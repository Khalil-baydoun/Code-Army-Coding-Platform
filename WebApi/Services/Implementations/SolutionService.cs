
using System.Threading.Tasks;
using DataContracts.Submissions;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

class SolutionService : ISolutionService
{
    ISolutionStore _solutionStore;

    public SolutionService(ISolutionStore solutionStore)
    {
        _solutionStore = solutionStore;
    }

    public async Task AddSolution(SubmissionRequest submissionRequest)
    {
        await _solutionStore.AddSolution(submissionRequest);
    }

    public async Task<string> GetSolution(string problemId, string progLang)
    {
        var solution = await _solutionStore.GetSolution(problemId, progLang);
        return solution;
    }
}