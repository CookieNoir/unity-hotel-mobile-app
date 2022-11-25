using System;
using System.Security.Cryptography;

public class SecurityHelper
{
    private const int _SALT_LENGTH = 32;
    private const int _ITERATIONS = 100;
    private const int _HASH_LENGTH = 64;

    public static string GenerateSalt()
    {
        var saltBytes = new byte[_SALT_LENGTH];

        using (var provider = new RNGCryptoServiceProvider())
        {
            provider.GetNonZeroBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, _ITERATIONS))
        {
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(_HASH_LENGTH));
        }
    }
}