using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class SaveEncryptionTests
    {
        const int KeySize = 256;
        const int BlockSize = 128;
        const int Iterations = 10000;
        const int SaltSize = 16;
        const int IVSize = 16;

        string Encrypt(string plainText, string key)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(IVSize);

            using var deriveBytes = new Rfc2898DeriveBytes(key, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] derivedKey = deriveBytes.GetBytes(KeySize / 8);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = derivedKey;
            aes.IV = iv;

            using var enc = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = enc.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[salt.Length + iv.Length + encrypted.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, salt.Length + iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        string Decrypt(string cipherText, string key)
        {
            byte[] allBytes = Convert.FromBase64String(cipherText);
            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IVSize];
            byte[] encrypted = new byte[allBytes.Length - SaltSize - IVSize];

            Buffer.BlockCopy(allBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(allBytes, SaltSize, iv, 0, IVSize);
            Buffer.BlockCopy(allBytes, SaltSize + IVSize, encrypted, 0, encrypted.Length);

            using var deriveBytes = new Rfc2898DeriveBytes(key, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] derivedKey = deriveBytes.GetBytes(KeySize / 8);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = derivedKey;
            aes.IV = iv;

            using var dec = aes.CreateDecryptor();
            byte[] decrypted = dec.TransformFinalBlock(encrypted, 0, encrypted.Length);
            return Encoding.UTF8.GetString(decrypted);
        }

        string ComputeHash(string data)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [Test]
        public void EncryptDecrypt_RoundTrip()
        {
            string key = "test-encryption-key-2026";
            string original = "{\"gold\":1500,\"gems\":50,\"level\":10}";
            string encrypted = Encrypt(original, key);
            string decrypted = Decrypt(encrypted, key);
            Assert.That(decrypted, Is.EqualTo(original));
        }

        [Test]
        public void Encrypted_DiffersFromPlaintext()
        {
            string key = "my-secret-key";
            string original = "Hello World";
            string encrypted = Encrypt(original, key);
            Assert.That(encrypted, Is.Not.EqualTo(original));
        }

        [Test]
        public void SameInput_ProducesDifferentCiphertext()
        {
            string key = "my-secret-key";
            string original = "same data";
            string encrypted1 = Encrypt(original, key);
            string encrypted2 = Encrypt(original, key);
            Assert.That(encrypted1, Is.Not.EqualTo(encrypted2)); // Random salt+IV
        }

        [Test]
        public void WrongKey_FailsDecryption()
        {
            string encrypted = Encrypt("secret data", "correct-key");
            Assert.Throws<CryptographicException>(() => Decrypt(encrypted, "wrong-key"));
        }

        [Test]
        public void InvalidCiphertext_ThrowsException()
        {
            Assert.Throws<FormatException>(() => Decrypt("not-base64!", "key"));
        }

        [Test]
        public void Hash_IsDeterministic()
        {
            string data = "player-save-data";
            string hash1 = ComputeHash(data);
            string hash2 = ComputeHash(data);
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void Hash_ChangesWithData()
        {
            string hash1 = ComputeHash("data1");
            string hash2 = ComputeHash("data2");
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void Hash_VerifiesIntegrity()
        {
            string data = "save-data-content";
            string hash = ComputeHash(data);
            Assert.That(ComputeHash(data), Is.EqualTo(hash));
            Assert.That(ComputeHash(data + "tampered"), Is.Not.EqualTo(hash));
        }

        [Test]
        public void LargePayload_EncryptsDecrypts()
        {
            string key = "large-payload-key";
            string large = new string('A', 10000);
            string encrypted = Encrypt(large, key);
            string decrypted = Decrypt(encrypted, key);
            Assert.That(decrypted, Is.EqualTo(large));
        }

        [Test]
        public void UnicodeContent_PreservedAfterEncryption()
        {
            string key = "unicode-key";
            string unicode = "こんにちは世界 🎮 العب";
            string encrypted = Encrypt(unicode, key);
            string decrypted = Decrypt(encrypted, key);
            Assert.That(decrypted, Is.EqualTo(unicode));
        }
    }
}
