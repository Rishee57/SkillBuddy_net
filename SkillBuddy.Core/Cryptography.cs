using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;

namespace SkillBuddy.Core
{
    public class Cryptography
    {
        // === Static global key/salt (if you want global API) ===
        private static string ENCRYPTION_KEY = string.Empty;
        private static byte[] SALT = Array.Empty<byte>();

        // === Instance key/salt (preferred) ===
        public string EncKey { get; }
        public byte[] EncSalt { get; }

        /// <summary>
        /// Create a new Cryptography instance with a given key and salt.
        /// Also updates static ENCRYPTION_KEY + SALT for static API usage.
        /// </summary>
        public Cryptography(string encryptionKey, byte[] salt)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));
            if (salt == null || salt.Length == 0)
                throw new ArgumentNullException(nameof(salt));

            EncKey = encryptionKey;
            EncSalt = salt;

            // Optional: keep static API in sync
            ENCRYPTION_KEY = encryptionKey;
            SALT = salt;
        }

        #region Static API (uses global ENCRYPTION_KEY/SALT)
        public static string Encrypt(string inputText)
        {
            if (string.IsNullOrEmpty(inputText)) return string.Empty;
            return EncryptInternal(ENCRYPTION_KEY, SALT, inputText);
        }

        public static string Decrypt(string inputText)
        {
            if (string.IsNullOrEmpty(inputText)) return string.Empty;
            return DecryptInternal(ENCRYPTION_KEY, SALT, inputText);
        }
        #endregion

        #region Internal helpers
        internal static string EncryptInternal(string key, byte[] salt, string inputText)
        {
            byte[] plainText = Encoding.Unicode.GetBytes(inputText);

            using Aes aesAlg = Aes.Create();
            using var derive = new Rfc2898DeriveBytes(
                key,
                salt,
                100_000,
                HashAlgorithmName.SHA256
            );

            aesAlg.Key = derive.GetBytes(32);
            aesAlg.IV = derive.GetBytes(16);

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainText, 0, plainText.Length);
                cryptoStream.FlushFinalBlock();
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        internal static string DecryptInternal(string key, byte[] salt, string inputText)
        {
            byte[] encryptedData = Convert.FromBase64String(inputText.Replace(" ", "+"));

            using Aes aesAlg = Aes.Create();
            using var derive = new Rfc2898DeriveBytes(
                key,
                salt,
                100_000,
                HashAlgorithmName.SHA256
            );

            aesAlg.Key = derive.GetBytes(32);
            aesAlg.IV = derive.GetBytes(16);

            using var memoryStream = new MemoryStream(encryptedData);
            using var cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream, Encoding.Unicode);

            return reader.ReadToEnd();
        }
        #endregion
    }

    public static class CryptographyExtension
    {
        public static string Encrypt(this Cryptography cryptography, string inputText)
        {
            if (string.IsNullOrEmpty(inputText)) return string.Empty;
            return Cryptography.EncryptInternal(cryptography.EncKey, cryptography.EncSalt, inputText);
        }

        public static string Decrypt(this Cryptography cryptography, string inputText)
        {
            if (string.IsNullOrEmpty(inputText)) return string.Empty;
            return Cryptography.DecryptInternal(cryptography.EncKey, cryptography.EncSalt, inputText);
        }
    }
}
