using furkantural.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
namespace furkantural.Infrastructure.Configuration;

/// <summary>
/// Extension to transparently decrypt all encrypted values in configuration at startup.
/// Encrypted values are identified by the $e$ prefix and decrypted using the master key
/// from the FT_ENCRYPTION_KEY environment variable.
/// </summary>
public static class ConfigurationDecryptionExtensions
{
    private const string MasterKeyEnvVar = "FT_ENCRYPTION_KEY";

    public static IConfigurationBuilder DecryptEncryptedValues(this IConfigurationBuilder builder)
    {
        // Build a temporary config to read current values
        var tempConfig = builder.Build();

        var masterKey = Environment.GetEnvironmentVariable(MasterKeyEnvVar);
        if (string.IsNullOrWhiteSpace(masterKey))
        {
            // No master key set — skip decryption (allows running without encryption in dev)
            return builder;
        }

        var encryptionService = new EncryptionService(masterKey);
        var decryptedValues = new Dictionary<string, string?>();

        foreach (var kvp in tempConfig.AsEnumerable())
        {
            if (kvp.Value is not null && encryptionService.IsEncrypted(kvp.Value))
            {
                decryptedValues[kvp.Key] = encryptionService.Decrypt(kvp.Value);
            }
        }

        if (decryptedValues.Count > 0)
        {
            builder.AddInMemoryCollection(decryptedValues);
        }

        return builder;
    }
}