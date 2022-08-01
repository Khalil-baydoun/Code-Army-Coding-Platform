using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Forbiddens;


namespace WebApi.Store.Interfaces
{
    public interface IForbiddensStore
    {
        List<Forbiddens> GetForbiddensOfProblem(string problemId);

        Task AddForbiddens(string problemId, Forbiddens forbiddnes);
    }
}