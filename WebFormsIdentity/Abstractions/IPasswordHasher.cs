namespace WebFormsIdentity.Abstractions
{
    public interface IPasswordHasher
    {
        (string hashedPassword, string salt) HashPassword(string password);
        bool VerifyPassword(string password, string storedHash, string storedSalt);
    }
}