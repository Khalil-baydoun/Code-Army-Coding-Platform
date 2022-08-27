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
        IStatisticsStore _statStore;
        IWaReportStore _waReportStore;
        ISolutionService _solutionService;

        public ServiceBusSubmissionQueue(
            IOptions<ServiceBusSettings> options,
            IStatisticsStore statStore,
            IOnlineJudgeService judgingService,
            IWaReportStore waReportStore,
            ISolutionService solutionService)
        {
            _statStore = statStore;
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
            SubmissionStatistics? subStats = null;

            bool isretry = false;

            if (!request.IsSolution && string.IsNullOrEmpty(request.SubmissionId))
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

                await _statStore.AddSubmission(subStats);
            }
            else if(!string.IsNullOrEmpty(request.SubmissionId))
            {
                subStats = await _statStore.GetSubmission(int.Parse(request.SubmissionId));
                if(subStats !=null && subStats.Verdict != (int)Verdict.InQueue)
                {
                    // submission already processed, do nothing
                    return;
                }

                if(subStats == null)
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

                    await _statStore.AddSubmission(subStats);
                }

                subStats.IsRetried = true;
                await _statStore.UpdateSubmission(subStats);
                isretry = true;
            }

            SubmissionQueueMessage queueMessage = new()
            {
                Request = request,
                SubmissionStatistics = subStats,
                IsyRetry = isretry
            };

            var message = new ServiceBusMessage(JsonSerializer.Serialize(queueMessage));
            message.MessageId = subStats?.Id.ToString()?? Guid.NewGuid().ToString();
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

                subStats.Verdict = (int)response.Verdict;
                await _statStore.UpdateSubmission(subStats);
            }

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}