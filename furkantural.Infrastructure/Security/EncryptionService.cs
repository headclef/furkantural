using furkantural.Application.Services.Abstract;
using System.Security.Cryptography;
using System.Text;
namespace furkantural.Infrastructure.Security;

/// <summary>
/// AES-256-CBC encryption with per-value random IV, PBKDF2 key derivation, and HMAC-SHA256 integrity.
/// Master key must be provided externally (environment variable) — never stored in config files.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const string Prefix = "$e$";
    private const int KeySize = 32;   // AES-256
    private const int IvSize = 16;    // AES block size
    private const int HmacSize = 32;  // SHA-256
    private const int Iterations = 150_000;

    // App-specific derivation salt — unique to this project, different from crypt project
    private static readonly byte[] DerivationSalt = Encoding.UTF8.GetBytes("ft-portfolio-kdf-2026-v2");

    private readonly byte[] _aesKey;
    private readonly byte[] _hmacKey;

    public EncryptionService(string masterKey)
    {
        if (string.IsNullOrWhiteSpace(masterKey))
            throw new ArgumentException("Encryption master key must not be empty.", nameof(masterKey));

        // Derive two independent keys from the master key using PBKDF2-SHA512
        using var kdf = new Rfc2898DeriveBytes(
            Encoding.UTF8.GetBytes(masterKey),
            DerivationSalt,
            Iterations,
            HashAlgorithmName.SHA512);

        _aesKey = kdf.GetBytes(KeySize);   // 32 bytes for AES-256
        _hmacKey = kdf.GetBytes(KeySize);  // 32 bytes for HMAC-SHA256
    }

    public string Encrypt(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] cipherBytes;
        using (var encryptor = aes.CreateEncryptor())
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(plainText);
            }
            cipherBytes = ms.ToArray();
        }

        // Build payload: IV(16) + Ciphertext(N)
        var payload = new byte[IvSize + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, payload, 0, IvSize);
        Buffer.BlockCopy(cipherBytes, 0, payload, IvSize, cipherBytes.Length);

        // Compute HMAC over IV + Ciphertext
        byte[] mac;
        using (var hmac = new HMACSHA256(_hmacKey))
        {
            mac = hmac.ComputeHash(payload);
        }

        // Final blob: IV(16) + Ciphertext(N) + HMAC(32)
        var result = new byte[payload.Length + HmacSize];
        Buffer.BlockCopy(payload, 0, result, 0, payload.Length);
        Buffer.BlockCopy(mac, 0, result, payload.Length, HmacSize);

        return Prefix + Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        if (!IsEncrypted(cipherText))
            throw new ArgumentException("Value is not in the expected encrypted format.");

        var blob = Convert.FromBase64String(cipherText[Prefix.Length..]);

        if (blob.Length < IvSize + HmacSize + 1)
            throw new CryptographicException("Encrypted data is too short to be valid.");

        // Extract IV, ciphertext, and HMAC
        var iv = new byte[IvSize];
        var encrypted = new byte[blob.Length - IvSize - HmacSize];
        var storedMac = new byte[HmacSize];

        Buffer.BlockCopy(blob, 0, iv, 0, IvSize);
        Buffer.BlockCopy(blob, IvSize, encrypted, 0, encrypted.Length);
        Buffer.BlockCopy(blob, blob.Length - HmacSize, storedMac, 0, HmacSize);

        // Verify HMAC integrity (IV + Ciphertext)
        var payloadForMac = new byte[IvSize + encrypted.Length];
        Buffer.BlockCopy(iv, 0, payloadForMac, 0, IvSize);
        Buffer.BlockCopy(encrypted, 0, payloadForMac, IvSize, encrypted.Length);

        byte[] expectedMac;
        using (var hmac = new HMACSHA256(_hmacKey))
        {
            expectedMac = hmac.ComputeHash(payloadForMac);
        }

        if (!CryptographicOperations.FixedTimeEquals(storedMac, expectedMac))
            throw new CryptographicException("HMAC verification failed. Data may have been tampered with.");

        // Decrypt
        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encrypted);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }

    public bool IsEncrypted(string value) =>
        !string.IsNullOrEmpty(value) && value.StartsWith(Prefix);
}