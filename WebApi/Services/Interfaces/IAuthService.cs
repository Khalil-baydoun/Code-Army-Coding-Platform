using System.Threading.Tasks;
using DataContracts.Authentication;

namespace WebApi.Services.Interfaces
{
    public interface IAuthenticationService
    {
        AuthenticationResult AuthenticateLogin(string email, string password);
    }
}
