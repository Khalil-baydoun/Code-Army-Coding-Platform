using System.Threading.Tasks;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;
using DataContracts.Forbiddens;
using System.Collections.Generic;
using WebApi.Exceptions;



namespace WebApi.Services.Implementations
{
    public class ForbiddensService : IForbiddensService
    {

        IForbiddensStore _forbiddensStore;

        public ForbiddensService(IForbiddensStore forbiddensStore)
        {
            _forbiddensStore = forbiddensStore;
        }
        public async Task AddForbiddens(string problemId, Forbiddens forbiddnes)
        {
            if (!(!string.IsNullOrEmpty(forbiddnes?.ProblemId) && forbiddnes != null))
            {
                throw new BadRequestException("Problem Id is empty or missing");
            }
            await _forbiddensStore.AddForbiddens(problemId, forbiddnes);
        }

        public List<Forbiddens> GetForbiddensOfProblem(string problemId)
        {
            if (string.IsNullOrWhiteSpace(problemId))
            {
                throw new BadRequestException("Problem Id is empty or missing");
            }
            return _forbiddensStore.GetForbiddensOfProblem(problemId);
        }
    }
}