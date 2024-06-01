using WebFormsIdentity.Abstractions;

namespace WebFormsIdentity.Hooks
{
    public class PasswordHasherHook
    {
        public static IPasswordHasher Hasher { get; set; }
    }
}