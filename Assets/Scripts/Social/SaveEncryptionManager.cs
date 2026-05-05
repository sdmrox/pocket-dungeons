using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PocketDungeons.Social
{
    public class SaveEncryptionManager : MonoBehaviour
    {
        public static SaveEncryptionManager Instance { get; private set; }

        private const int KeySize = 256;
        private const int BlockSize = 128;
        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int IVSize = 16;

        private string _encryptionKey;

        public bool IsInitialized => !string.IsNullOrEmpty(_encryptionKey);

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Encryption key cannot be null or empty");

            _encryptionKey = key;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plain text cannot be null or empty");
            if (!IsInitialized)
                throw new InvalidOperationException("Encryption not initialized");

            byte[] salt = GenerateRandomBytes(SaltSize);
            byte[] iv = GenerateRandomBytes(IVSize);

            using var deriveBytes = new Rfc2898DeriveBytes(
                _encryptionKey, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(KeySize / 8);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[salt.Length + iv.Length + encrypted.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, salt.Length + iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Cipher text cannot be null or empty");
            if (!IsInitialized)
                throw new InvalidOperationException("Encryption not initialized");

            byte[] allBytes = Convert.FromBase64String(cipherText);

            if (allBytes.Length < SaltSize + IVSize + 1)
                throw new CryptographicException("Invalid cipher text");

            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IVSize];
            byte[] encrypted = new byte[allBytes.Length - SaltSize - IVSize];

            Buffer.BlockCopy(allBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(allBytes, SaltSize, iv, 0, IVSize);
            Buffer.BlockCopy(allBytes, SaltSize + IVSize, encrypted, 0, encrypted.Length);

            using var deriveBytes = new Rfc2898DeriveBytes(
                _encryptionKey, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(KeySize / 8);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        public bool TryDecrypt(string cipherText, out string plainText)
        {
            try
            {
                plainText = Decrypt(cipherText);
                return true;
            }
            catch
            {
                plainText = null;
                return false;
            }
        }

        public string ComputeHash(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Data cannot be null or empty");

            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyHash(string data, string expectedHash)
        {
            string actualHash = ComputeHash(data);
            return string.Equals(actualHash, expectedHash, StringComparison.Ordinal);
        }

        private static byte[] GenerateRandomBytes(int size)
        {
            byte[] bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return bytes;
        }
    }
}
