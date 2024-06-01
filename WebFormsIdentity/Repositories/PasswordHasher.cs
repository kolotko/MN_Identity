using System;
using System.Security.Cryptography;
using WebFormsIdentity.Abstractions;

namespace WebFormsIdentity.Repositories
{
    public class PasswordHasher : IPasswordHasher
    {
        public (string hashedPassword, string salt) HashPassword(string password)
        {
            // Generate a salt
            byte[] saltBytes;
            new RNGCryptoServiceProvider().GetBytes(saltBytes = new byte[16]);
            string salt = Convert.ToBase64String(saltBytes);

            // Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[20];
            Array.Copy(hash, 0, hashBytes, 0, 20);

            // Turn the combined hash into a string
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return (hashedPassword, salt);
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            // Extract the bytes
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the results
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}