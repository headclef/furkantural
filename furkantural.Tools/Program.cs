// Temporary helper to encrypt appsettings values.
// Usage: dotnet run --project furkantural.Tools -- <masterKey>
// Delete this project after generating encrypted values.

using furkantural.Infrastructure.Security;

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run --project furkantural.Tools -- <masterKey>");
    return;
}

var masterKey = args[0];
var service = new EncryptionService(masterKey);

var secrets = new Dictionary<string, string>
{
    // ["Smtp:Host"] = "HOST",
    // ["Smtp:Password"] = "PASSWORD",
    // ["Smtp:SenderEmail"] = "SENDER_EMAIL",
    // ["Smtp:ListenerEmail"] = "LISTENER_EMAIL",
    // ["Turnstile:SecretKey"] = "SECRET_KEY",
    // ["ConnectionStrings:DefaultConnection"] = "CONNECTION_STRING",
};

Console.WriteLine("=== Encrypted Values ===\n");
foreach (var kvp in secrets)
{
    var encrypted = service.Encrypt(kvp.Value);
    Console.WriteLine($"[{kvp.Key}]");
    Console.WriteLine($"  Plain:     {kvp.Value}");
    Console.WriteLine($"  Encrypted: {encrypted}");
    Console.WriteLine();

    // Verify round-trip
    var decrypted = service.Decrypt(encrypted);
    if (decrypted != kvp.Value)
        Console.WriteLine($"  *** ROUND-TRIP FAILED for {kvp.Key}! ***");
}

Console.WriteLine("=== All values encrypted and verified successfully. ===");