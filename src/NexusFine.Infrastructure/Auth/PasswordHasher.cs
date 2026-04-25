using System.Security.Cryptography;

namespace NexusFine.Infrastructure.Auth;

/// <summary>
/// PBKDF2-SHA256, 210,000 iterations, 16-byte salt, 32-byte hash.
/// Output is Base64. Constant-time comparison on verify.
/// </summary>
public class PasswordHasher
{
    private const int SaltSize     = 16;
    private const int HashSize     = 32;
    private const int Iterations   = 210_000;
    private static readonly HashAlgorithmName Algo = HashAlgorithmName.SHA256;

    public (string Hash, string Salt) Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algo, HashSize);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool Verify(string password, string hash, string salt)
    {
        if (string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(hash)     ||
            string.IsNullOrEmpty(salt))
            return false;

        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            var expected  = Convert.FromBase64String(hash);
            var actual    = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, Algo, HashSize);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
