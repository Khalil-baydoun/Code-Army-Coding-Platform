using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using DataContracts.Statistics;
using DataContracts.Submissions;
using Microsoft.Extensions.DependencyInjection;
using webapi.Services.Interfaces;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

public class JudgingQueue
{

    IOnlineJudgeService _judgingService;

    IStatisticsService _statService;
    IWaReportStore _waReportStore;
    IReportStore _reportStore;
    ISolutionService _solutionService;


    public JudgingQueue(IStatisticsService statService,
                        IOnlineJudgeService judgingService,
                        IWaReportStore waReportStore,
                        IReportStore report,
                        ISolutionService solutionService)
    {
        _statService = statService;
        _judgingService = judgingService;
        _waReportStore = waReportStore;
        _reportStore = report;
        _solutionService = solutionService;
    }

    private int cnt = 0;
    private Queue<Tuple<SubmissionRequest, bool, SubmissionStatistics>> _jobs = new Queue<Tuple<SubmissionRequest, bool, SubmissionStatistics>>();
    private bool _delegateQueuedOrRunning = false;

    public void Enqueue(SubmissionRequest request, bool isSolution, ClaimsPrincipal user)
    {
        lock (_jobs)
        {
            SubmissionStatistics subStats = null;
            if (!isSolution)
            {
                subStats = new SubmissionStatistics
                {
                    ProblemId = Int32.Parse(request.ProblemId),
                    SourceCode = request.SourceCode,
                    ProgrammingLanguage = (int)request.ProgLanguage,
                    UserEmail = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Email).Value,
                    SubmittedAt = DateTime.Now,
                    Verdict = (int)Verdict.InQueue
                };
                _statService.AddSubmission(subStats);
            }
            _jobs.Enqueue(new Tuple<SubmissionRequest, bool, SubmissionStatistics>(request, isSolution, subStats));
            if (!_delegateQueuedOrRunning)
            {
                _delegateQueuedOrRunning = true;
                ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
            }
        }
    }

    private void ProcessQueuedItems(object ignored)
    {
        while (true)
        {
            Tuple<SubmissionRequest, bool, SubmissionStatistics> item;
            lock (_jobs)
            {
                if (_jobs.Count == 0)
                {
                    _delegateQueuedOrRunning = false;
                    break;
                }
                item = _jobs.Dequeue();
            }
            try
            {
                Console.WriteLine("Judging Process #" + cnt.ToString());
                cnt++;
                SubmissionResponse response = _judgingService.JudgeCode(item.Item1, item.Item2);

                if (!item.Item2)
                {
                    response.Report.Id = item.Item3.Id;

                    _reportStore.addReport(response.Report);

                    if (response.Verdict == Verdict.WrongAnswer)
                    {
                        response.WaReport.Id = item.Item3.Id;
                        _waReportStore.AddReport(response.WaReport);
                    }

                    DateTime subTime = DateTime.Now;
                    var subStats = item.Item3;
                    subStats.MemoryTakenInKiloBytes = response.MemoryTakenInKiloBytes;
                    subStats.TimeTakenInMilliseconds = response.TimeTakenInMilliseconds;
                    subStats.Verdict = (int)response.Verdict;
                    _statService.UpdateSubmission(subStats);
                }
                else
                {
                    if (response.Verdict == Verdict.Accepted)
                    {
                        var id = _solutionService.AddSolution(item.Item1);
                    }
                }
            }
            catch
            {
                ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
                throw;
            }
        }
    }

    private SubmissionStatistics GetSubmissionStatistics(SubmissionRequest req, SubmissionResponse resp, ClaimsPrincipal User)
    {
        return new SubmissionStatistics
        {
            ProblemId = Int32.Parse(req.ProblemId),
            UserEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value,
            MemoryTakenInKiloBytes = resp.MemoryTakenInKiloBytes,
            TimeTakenInMilliseconds = resp.TimeTakenInMilliseconds,
            Verdict = (int)resp.Verdict,
            SourceCode = req.SourceCode,
            ProgrammingLanguage = (int)req.ProgLanguage
        };
    }
}