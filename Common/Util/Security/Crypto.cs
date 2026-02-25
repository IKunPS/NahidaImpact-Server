using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NahidaImpact.Data.Models.Sdk;

namespace NahidaImpact.Util.Security;

public class Crypto
{
    private static readonly Random SecureRandom = new();
    private static readonly Logger _logger = new("Crypto");
    public static byte[] DISPATCH_KEY { get; private set; } = Array.Empty<byte>();
    public static byte[] DISPATCH_SEED { get; private set; } = Array.Empty<byte>();
    public static byte[] ENCRYPT_KEY { get; private set; } = Array.Empty<byte>();
    public static byte[] ENCRYPT_SEED_BUFFER { get; private set; } = Array.Empty<byte>();
    
    public static RSA? CUR_SIGNING_KEY { get; private set; }
    public static RSA? SDK_PATCH_KEY { get; private set; }
    
    public static Dictionary<int, RSA> EncryptionKeys { get; } = new();
    
    public static void LoadKeys()
    {
        try
        {
            // Load scheduling key
            DISPATCH_KEY = File.ReadAllBytes("Config/security/dispatchKey.bin");
            DISPATCH_SEED = File.ReadAllBytes("Config/security/dispatchSeed.bin");
            
            // Load encryption key
            ENCRYPT_KEY = File.ReadAllBytes("Config/security/secretKey.bin");
            ENCRYPT_SEED_BUFFER = File.ReadAllBytes("Config/security/secretKeyBuffer.bin");
            
            // Load signature private key
            var signingKeyBytes = File.ReadAllBytes("Config/security/SigningKey.der");
            CUR_SIGNING_KEY = RSA.Create();
            CUR_SIGNING_KEY.ImportPkcs8PrivateKey(signingKeyBytes, out _);
            
            // Load sdk private key
            var sdkBytes = File.ReadAllBytes("Config/security/sdk_private_key.der");
            SDK_PATCH_KEY = RSA.Create();
            SDK_PATCH_KEY.ImportPkcs8PrivateKey(sdkBytes, out _);
            
            
            // Load the game public key
            var gameKeysDir = "Config/security/game_keys";
            if (Directory.Exists(gameKeysDir))
            {
                var pattern = new Regex(@"([0-9]*)_Pub\.der");
                
                foreach (var file in Directory.GetFiles(gameKeysDir, "*_Pub.der"))
                {
                    var fileName = Path.GetFileName(file);
                    var match = pattern.Match(fileName);
                    
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int keyId))
                    {
                        var keyBytes = File.ReadAllBytes(file);
                        var rsa = RSA.Create();
                        rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
                        EncryptionKeys[keyId] = rsa;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An error occurred while loading keys: {ex.Message}");
        }
    }
    
    public static byte[] Xor(string data, byte[] key)
    {
        byte[] result = Encoding.UTF8.GetBytes(data);
        Xor(result, key);

        return result;
    }

    public static void Xor(byte[] packet, byte[] key)
    {
        try {
            for (int i = 0; i < packet.Length; i++) {
                packet[i] ^= key[i % key.Length];
            }
        } catch (Exception e) {
            _logger.Error("Crypto error.", e);
        }
    }
    
    // Simple way to create a unique session key
    public static string CreateSessionKey(string accountUid)
    {
        var random = new byte[32];
        SecureRandom.NextBytes(random);

        var temp = accountUid + "." + DateTime.Now.Ticks + "." + SecureRandom;

        try
        {
            var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(temp));
            return Convert.ToBase64String(bytes);
        }
        catch
        {
            var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(temp));
            return Convert.ToBase64String(bytes);
        }
    }
    
    public static QueryCurRegionRspJson EncryptAndSignRegionData(byte[] regionInfo, string keyId)
    {
        if (string.IsNullOrEmpty(keyId))
            throw new ArgumentException("Key ID was not set", nameof(keyId));
        if (!int.TryParse(keyId, out int id))
            throw new ArgumentException("Invalid Key ID format", nameof(keyId));
        if (!EncryptionKeys.TryGetValue(id, out var publicKey))
            throw new KeyNotFoundException($"No encryption key found for ID: {keyId}");
        if (CUR_SIGNING_KEY == null)
            throw new InvalidOperationException("Signing key has not been initialized");
        
        // 分块加密
        const int chunkSize = 245; // 256 - 11
        int dataLength = regionInfo.Length;
        int numChunks = (int)Math.Ceiling(dataLength / (double)chunkSize);
        
        using var encryptedStream = new MemoryStream();
        for (int i = 0; i < numChunks; i++)
        {
            int offset = i * chunkSize;
            int length = Math.Min(chunkSize, dataLength - offset);
            var chunk = regionInfo.AsSpan(offset, length);
            
            byte[] encryptedChunk = publicKey.Encrypt(
                chunk.ToArray(), RSAEncryptionPadding.Pkcs1);
            
            encryptedStream.Write(encryptedChunk);
        }
        
        // 创建签名
        byte[] signature = CUR_SIGNING_KEY.SignData(
            regionInfo, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        return new QueryCurRegionRspJson
        {
            Content = Convert.ToBase64String(encryptedStream.ToArray()),
            Sign = Convert.ToBase64String(signature)
        };
    }
    
    public static RSA GetDispatchEncryptionKey(int key)
    {
        return EncryptionKeys[key];
    }
}