using FluentAssertions;
using furkantural.Infrastructure.Security;
using System.Security.Cryptography;
namespace furkantural.Infrastructure.Tests.Security;

public class EncryptionServiceTests
{
    private const string MasterKey = "TestMasterKey-2026-UnitTests-SecureX9!";
    private readonly EncryptionService _service = new(MasterKey);

    #region Encrypt

    [Fact]
    public void Encrypt_ShouldReturnPrefixedString()
    {
        var result = _service.Encrypt("hello");

        result.Should().StartWith("$e$");
    }

    [Fact]
    public void Encrypt_SameValueTwice_ShouldProduceDifferentCiphertext()
    {
        var result1 = _service.Encrypt("same-value");
        var result2 = _service.Encrypt("same-value");

        result1.Should().NotBe(result2, "random IV makes each encryption unique");
    }

    [Fact]
    public void Encrypt_EmptyString_ShouldSucceed()
    {
        var result = _service.Encrypt(string.Empty);

        result.Should().StartWith("$e$");
    }

    [Fact]
    public void Encrypt_NullInput_ShouldThrow()
    {
        var act = () => _service.Encrypt(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Decrypt

    [Fact]
    public void Decrypt_ShouldReturnOriginalText()
    {
        var original = "S3s!U3ThUkP.UGS";
        var encrypted = _service.Encrypt(original);

        var decrypted = _service.Decrypt(encrypted);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void Decrypt_LongText_ShouldRoundTrip()
    {
        var original = "YOUR_SUPER_SECRET_DATABASE_CONENCTION_STRING";
        var encrypted = _service.Encrypt(original);

        var decrypted = _service.Decrypt(encrypted);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void Decrypt_UnicodeText_ShouldRoundTrip()
    {
        var original = "Türkçe karakter: ğüşöçİ 🚀";
        var encrypted = _service.Encrypt(original);

        var decrypted = _service.Decrypt(encrypted);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void Decrypt_WithoutPrefix_ShouldThrow()
    {
        var act = () => _service.Decrypt("not-encrypted-value");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Decrypt_TamperedCiphertext_ShouldThrowCryptographic()
    {
        var encrypted = _service.Encrypt("secret");
        // Tamper with the base64 content (flip a character in the middle)
        var chars = encrypted.ToCharArray();
        var idx = encrypted.Length / 2;
        chars[idx] = chars[idx] == 'A' ? 'B' : 'A';
        var tampered = new string(chars);

        var act = () => _service.Decrypt(tampered);

        act.Should().Throw<Exception>(); // CryptographicException or FormatException
    }

    [Fact]
    public void Decrypt_WrongMasterKey_ShouldThrow()
    {
        var encrypted = _service.Encrypt("secret");
        var wrongKeyService = new EncryptionService("WrongKey-Completely-Different-2026");

        var act = () => wrongKeyService.Decrypt(encrypted);

        act.Should().Throw<CryptographicException>("HMAC verification should fail with wrong key");
    }

    #endregion

    #region IsEncrypted

    [Fact]
    public void IsEncrypted_WithPrefix_ShouldReturnTrue()
    {
        _service.IsEncrypted("$e$somebase64data").Should().BeTrue();
    }

    [Fact]
    public void IsEncrypted_WithoutPrefix_ShouldReturnFalse()
    {
        _service.IsEncrypted("plaintext").Should().BeFalse();
    }

    [Fact]
    public void IsEncrypted_NullOrEmpty_ShouldReturnFalse()
    {
        _service.IsEncrypted(string.Empty).Should().BeFalse();
        _service.IsEncrypted(null!).Should().BeFalse();
    }

    #endregion

    #region Constructor

    [Fact]
    public void Constructor_EmptyMasterKey_ShouldThrow()
    {
        var act = () => new EncryptionService(string.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WhitespaceMasterKey_ShouldThrow()
    {
        var act = () => new EncryptionService("   ");

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region DifferentKeysProduceDifferentOutput

    [Fact]
    public void DifferentMasterKeys_ShouldNotDecryptEachOthersData()
    {
        var service1 = new EncryptionService("MasterKeyAlpha-2026");
        var service2 = new EncryptionService("MasterKeyBeta-2026");

        var encrypted = service1.Encrypt("confidential");

        var act = () => service2.Decrypt(encrypted);
        act.Should().Throw<CryptographicException>();
    }

    #endregion
}