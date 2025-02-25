using BCrypt.Net;

namespace GrubPix.Application.Utils
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashes a plain text password using BCrypt.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <returns>Hashed password</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a plain text password against a hashed password.
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <param name="hashedPassword">The stored hashed password</param>
        /// <returns>True if the password matches, otherwise false.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
