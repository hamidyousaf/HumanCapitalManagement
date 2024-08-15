using Domain.Entities;

namespace Application.Abstractions.Services;

public interface ITokenService
{
    public string RefreshToken(string token);
    public string GenerateToken(User user);
}
