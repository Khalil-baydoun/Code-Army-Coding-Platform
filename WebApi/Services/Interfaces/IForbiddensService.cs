using System.Threading.Tasks;
using DataContracts.Forbiddens;
using System.Collections.Generic;

namespace WebApi.Services.Interfaces
{
    public interface IForbiddensService
    {
        List<Forbiddens> GetForbiddensOfProblem(string problemId);

        Task AddForbiddens(string problemId, Forbiddens forbiddnes);
    }
}