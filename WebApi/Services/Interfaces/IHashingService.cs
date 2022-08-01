
namespace WebApi.Services.Interfaces
{
    public interface IHashingService
    {
        string Hash(string value, string salt);
    }
}
