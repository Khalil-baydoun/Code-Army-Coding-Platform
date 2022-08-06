using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.ProblemSets;
using DataContracts.Users;

namespace WebApi.Store.Interfaces
{
    public interface IUserStore
    {
        Task AddUser(AddUserRequest user);

        User GetUser(string email);

        Task DeleteUser(string email);

        Task UpdateUser(AddUserRequest user);
    }
}