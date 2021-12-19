using BC = BCrypt.Net.BCrypt;

namespace Infrastructure.Identity.Helpers
{
    public static class PasswordHelper
    {
        public static string CreatePasswordHash(string password)
        {
            return BC.HashPassword(password);
        }

        public static bool VerifyPassword(string hash, string password)
        {
            return BC.Verify(password, hash);
        }
    }
}
