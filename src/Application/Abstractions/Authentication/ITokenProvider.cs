using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    AccessToken  Create(User user);
    
    string GenerateRandomToken();
}
