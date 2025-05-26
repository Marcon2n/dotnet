using System.Security.Cryptography;

namespace DotNetProjectExample.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Mật khẩu không được để trống.");
            }

            byte[] salt;
            using (var rng = RandomNumberGenerator.Create())
            {
                salt = new byte[SaltSize];
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPasswordWithSalt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Mật khẩu không được để trống.");
            }
            if (string.IsNullOrEmpty(hashedPasswordWithSalt))
            {
                throw new ArgumentNullException(nameof(hashedPasswordWithSalt), "Chuỗi hash đã lưu không được để trống.");
            }

            byte[] hashBytes;
            try
            {
                hashBytes = Convert.FromBase64String(hashedPasswordWithSalt);
            }
            catch (FormatException)
            {
                return false;
            }

            if (hashBytes.Length != SaltSize + HashSize)
            {
                return false;
            }

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hashToVerify = pbkdf2.GetBytes(HashSize);

            bool hashesMatch = true;
            for (int i = 0; i < HashSize; i++)
            {
                if (hashToVerify[i] != hashBytes[i + SaltSize])
                {
                    hashesMatch = false;
                    break;
                }
            }
            return hashesMatch;
        }
    }
}