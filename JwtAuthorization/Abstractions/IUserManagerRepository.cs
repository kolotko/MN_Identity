using JwtAuthorization.Models;

namespace JwtAuthorization.Abstractions;

public interface IUserManagerRepository
{
    User FindByName(string username);
    void Update(User user);
}