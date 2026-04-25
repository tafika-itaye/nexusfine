using NexusFine.Infrastructure.Auth;
using Xunit;

namespace NexusFine.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ProducesNonEmptyBase64()
    {
        var (hash, salt) = _hasher.Hash("StrongPass!2026");
        Assert.False(string.IsNullOrEmpty(hash));
        Assert.False(string.IsNullOrEmpty(salt));
    }

    [Fact]
    public void Verify_SamePassword_ReturnsTrue()
    {
        var (hash, salt) = _hasher.Hash("StrongPass!2026");
        Assert.True(_hasher.Verify("StrongPass!2026", hash, salt));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var (hash, salt) = _hasher.Hash("StrongPass!2026");
        Assert.False(_hasher.Verify("strongpass!2026", hash, salt));
    }

    [Fact]
    public void Verify_MalformedInput_ReturnsFalse()
    {
        Assert.False(_hasher.Verify("pw", "not-base64!!", "also-not-base64"));
    }

    [Fact]
    public void Hash_SamePasswordTwice_ProducesDifferentHashes()
    {
        var a = _hasher.Hash("SamePassword");
        var b = _hasher.Hash("SamePassword");
        Assert.NotEqual(a.Hash, b.Hash);
        Assert.NotEqual(a.Salt, b.Salt);
    }
}
