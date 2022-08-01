using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Groups;
using DataContracts.Users;

namespace WebApi.Services.Interfaces
{
    public interface IUserService
    {
        Task AddUser(AddUserRequest user);

        User GetUser(string email);

        Task DeleteUser(string email);

        Task UpdateUser(AddUserRequest user);
        Task AddGroup(string groupName);
    }
}
