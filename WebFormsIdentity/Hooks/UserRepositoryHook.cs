using WebFormsIdentity.Abstractions;
using WebFormsIdentity.Repositories;

namespace WebFormsIdentity.Hooks
{
    public static class UserRepositoryHook
    {
        public static IUserRepository Repository { get; set; }
    }
}