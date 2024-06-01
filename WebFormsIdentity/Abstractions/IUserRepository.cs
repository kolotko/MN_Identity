namespace WebFormsIdentity.Abstractions
{
    public interface IUserRepository
    {
        void CreateUser(string username, string hashedPassword, string salt);
        (string hashedPassword, string salt) GetUserPassHashAandSalt(string username);
    }
}