using JwtAuthorization.Abstractions;
using JwtAuthorization.Models;

namespace JwtAuthorization.Repositories;

public class UserManagerRepository : IUserManagerRepository
{
    private static List<User> _storage = new()
    {
        new User()
        {
            UserName = "admin"
        }
    };
    public User FindByName(string username)
    {
        var user = _storage.Where(x => x.UserName == username).FirstOrDefault();

        if (user is not null)
        {
            return user;
        }

        return null;
    }

    public void Update(User user)
    {
        var index = _storage.FindIndex(x => x.UserName == user.UserName);
        _storage[index] = user;
    }
}