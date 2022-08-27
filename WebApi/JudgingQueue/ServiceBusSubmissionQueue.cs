using Azure.Messaging.ServiceBus;
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
        ISubmissionsStore _subStore;
        ISolutionService _solutionService;

        public ServiceBusSubmissionQueue(
            IOptions<ServiceBusSettings> options,
            ISubmissionsStore subStore,
            IOnlineJudgeService judgingService,
            ISolutionService solutionService)
        {
            _subStore = subStore;
            _judgingService = judgingService;
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
            Submission? sub = null;

            bool isretry = false;

            if (!request.IsSolution && string.IsNullOrEmpty(request.SubmissionId))
            {
                sub = new Submission
                {
                    ProblemId = int.Parse(request.ProblemId),
                    SourceCode = request.SourceCode,
                    ProgrammingLanguage = request.ProgLanguage,
                    UserEmail = request.UserEmail,
                    SubmittedAt = DateTime.Now,
                    Verdict = (int)Verdict.InQueue
                };

                await _subStore.AddSubmission(sub);
            }
            else if (!string.IsNullOrEmpty(request.SubmissionId))
            {
                sub = await _subStore.GetSubmission(int.Parse(request.SubmissionId));
                if (sub != null && sub.Verdict != (int)Verdict.InQueue)
                {
                    // submission already processed, do nothing
                    return;
                }

                if (sub == null)
                {
                    sub = new Submission
                    {
                        ProblemId = int.Parse(request.ProblemId),
                        SourceCode = request.SourceCode,
                        ProgrammingLanguage = request.ProgLanguage,
                        UserEmail = request.UserEmail,
                        SubmittedAt = DateTime.Now,
                        Verdict = (int)Verdict.InQueue
                    };

                    await _subStore.AddSubmission(sub);
                }

                sub.IsRetried = true;
                await _subStore.UpdateSubmission(sub);
                isretry = true;
            }

            SubmissionQueueMessage queueMessage = new()
            {
                Request = request,
                Submission = sub,
                IsyRetry = isretry
            };

            var message = new ServiceBusMessage(JsonSerializer.Serialize(queueMessage));
            message.MessageId = sub?.Id.ToString() ?? Guid.NewGuid().ToString();
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
                var sub = queueMessage.Submission;
                sub.Verdict = (int)response.Verdict;
                sub.CompilerErrorMessage = response.CompilerErrorMessage;
                sub.RuntimeErrorMessage = response.RuntimeErrorMessage;
                sub.ActualOutput = response.ActualOutput;
                sub.ExpectedOutput = response.ExpectedOutput;
                sub.TestsPassed = response.TestsPassed;
                sub.TotalTests = response.TotalTests;
                sub.WrongTestInput = response.WrongTestInput;

                await _subStore.UpdateSubmission(sub);
            }

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}