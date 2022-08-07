using Azure.Messaging.ServiceBus;
using DataContracts.Statistics;
using DataContracts.Submissions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using webapi.Services.Interfaces;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace Webapi.JudgingQueue
{
    public class ServiceBusSubmissionQueue : ISubmissionQueue
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusProcessor _processor;
        IOnlineJudgeService _judgingService;
        IStatisticsService _statService;
        IWaReportStore _waReportStore;
        ISolutionService _solutionService;

        public ServiceBusSubmissionQueue(
            IOptions<ServiceBusSettings> options,
            IStatisticsService statService,
            IOnlineJudgeService judgingService,
            IWaReportStore waReportStore,
            ISolutionService solutionService)
        {
            _statService = statService;
            _judgingService = judgingService;
            _waReportStore = waReportStore;
            _solutionService = solutionService;
            _client = new ServiceBusClient(options.Value.ConnectionString);
            _sender = _client.CreateSender(options.Value.SubmissionQueueName);
            _processor = _client.CreateProcessor(options.Value.SubmissionQueueName, new ServiceBusProcessorOptions());
            _processor.ProcessMessageAsync += DequeueSubmission;
            _processor.ProcessErrorAsync += ErrorHandler;
            Task.Run(() => _processor.StartProcessingAsync()).Wait();
        }

        public async Task EnqueueSubmission(SubmissionRequest request)
        {
            request.SubmissionId = Guid.NewGuid().ToString();

            SubmissionStatistics subStats = null;
            if (!request.IsSolution)
            {
                subStats = new SubmissionStatistics
                {
                    ProblemId = int.Parse(request.ProblemId),
                    SourceCode = request.SourceCode,
                    ProgrammingLanguage = request.ProgLanguage,
                    UserEmail = request.UserEmail,
                    SubmittedAt = DateTime.Now,
                    Verdict = (int)Verdict.InQueue
                };

                await _statService.AddSubmission(subStats);
            }


            SubmissionQueueMessage queueMessage = new()
            {
                Request = request,
                SubmissionStatistics = subStats
            };

            var message = new ServiceBusMessage(JsonSerializer.Serialize(queueMessage));
            message.MessageId = request.SubmissionId;
            await _sender.SendMessageAsync(message);
        }

        public async Task DequeueSubmission(ProcessMessageEventArgs args)
        {
            string message = args.Message.Body.ToString();
            SubmissionQueueMessage? queueMessage = JsonSerializer.Deserialize<SubmissionQueueMessage>(message);
            var response = await _judgingService.JudgeCode(queueMessage.Request, queueMessage.Request.IsSolution);
            if (queueMessage == null)
            {
                return;
            }

            if (queueMessage.Request.IsSolution)
            {
                await _solutionService.AddSolution(queueMessage.Request);
            }
            else
            {
                var subStats = queueMessage.SubmissionStatistics;
                //if (response.Verdict == Verdict.WrongAnswer)
                //{
                //    response.WaReport.SubmissionId = subStats.Id;
                //    _waReportStore.AddReport(response.WaReport);
                //}

                subStats.SubmittedAt = DateTime.Now;
                subStats.Verdict = (int)response.Verdict;
                await _statService.UpdateSubmission(subStats);
            }

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}