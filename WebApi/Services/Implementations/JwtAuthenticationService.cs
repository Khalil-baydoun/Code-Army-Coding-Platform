using DataContracts.Authentication;
using DataContracts.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Exceptions;
using WebApi.Services.Implementations.Settings;
using WebApi.Services.Interfaces;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly IOptions<JwtSettings> _options;
    private readonly IUserService _userService;
    private readonly IHashingService _hashingService;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtAuthenticationService(IOptions<JwtSettings> options, IUserService userService, TokenValidationParameters tokenValidationParameters, IHashingService hashingService)
    {
        _options = options;
        _userService = userService;
        _tokenValidationParameters = tokenValidationParameters;
        _hashingService = hashingService;
    }

    public AuthenticationResult AuthenticateLogin(string username, string password)
    {
        try
        {
            var user = _userService.GetUser(username);
            VerifyPasswordOrThrow(user.Password, password, user.Salt);
            return GenerateAuthenticationResult(user);
        }
        catch (StorageErrorException)
        {
            throw new BadRequestException("Email or Password are invalid");
        }
    }

    private AuthenticationResult GenerateAuthenticationResult(User user)
    {
        var accessToken = CreateAccessToken(user, _options.Value.AccessTokenExpiryTimeInSeconds);
        return new AuthenticationResult
        {
            Token = accessToken,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = (Role)user.Role
        };
    }

    private string CreateAccessToken(User user, long accessTokenExpiryTimeInSeconds)
    {
        var key = Encoding.ASCII.GetBytes(_options.Value.Secret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddSeconds(accessTokenExpiryTimeInSeconds),
            SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _options.Value.Issuer,
            Audience = _options.Value.Audience,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private void VerifyPasswordOrThrow(string storedPasswordHash, string sentPassword, string salt)
    {
        string sendPasswordHash = _hashingService.Hash(sentPassword, salt);
        if (!sendPasswordHash.Equals(storedPasswordHash))
        {
            throw new BadRequestException("Email or Password are invalid");
        }
    }
}


