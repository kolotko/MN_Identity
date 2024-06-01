using System;
using WebFormsIdentity.Abstractions;
using Npgsql;

namespace WebFormsIdentity.Repositories
{
    public class UserRepository : IUserRepository
    {
        public void CreateUser(string username, string hashedPassword, string salt)
        {
            
            using (var connection = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO WebForms.Users (Username, PasswordHash, Salt) VALUES (@Username, @PasswordHash, @Salt)";
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.ExecuteNonQuery();
                }
            }
        }

        public (string hashedPassword, string salt) GetUserPassHashAandSalt(string username)
        {
            using (var connection = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT PasswordHash, Salt FROM WebForms.Users WHERE Username = @Username";
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashedPassword = reader["PasswordHash"].ToString();
                            string salt = reader["Salt"].ToString();
                            return (hashedPassword, salt);
                        }
                        
                        throw new Exception("User not found.");
                    }
                }
            }
        }
    }
}