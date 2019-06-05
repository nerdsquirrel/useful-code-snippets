using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace InsuranceRight.Web.Core.Helpers
{
    /// <summary>
    /// Provides helper methods for data encryption.
    /// </summary>
    internal static class EncryptionHelper
    {
        /// <summary>
        /// Encrypts the provided data with specified in the template algorithm.
        /// </summary>
        /// <typeparam name="T">Type of the symmetric algorithm.</typeparam>
        /// <param name="value">Text to be encrypted.</param>
        /// <param name="password">Password for encryption.</param>
        /// <param name="salt">Salt for encryption.</param>
        /// <returns>Encrypted data in Base64 format.</returns>
        internal static string Encrypt<T>(string value, string password, string salt)
             where T : SymmetricAlgorithm, new()
        {
            using (DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt)))
            {
                using (SymmetricAlgorithm algorithm = new T())
                {
                    byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
                    byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

                    ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

                    using (MemoryStream buffer = new MemoryStream())
                    {
                        using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                            {
                                writer.Write(value);
                            }
                        }

                        return Convert.ToBase64String(buffer.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts the provided text with specified in the template algorithm.
        /// </summary>
        /// <typeparam name="T">Type of the symmetric algorithm.</typeparam>
        /// <param name="text">Text to be decrypted.</param>
        /// <param name="password">Password for encryption.</param>
        /// <param name="salt">Salt for encryption.</param>
        /// <returns>Decrypted data.</returns>
        internal static string Decrypt<T>(string text, string password, string salt)
          where T : SymmetricAlgorithm, new()
        {
            using (DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt)))
            {
                using (SymmetricAlgorithm algorithm = new T())
                {
                    byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
                    byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

                    ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

                    using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
                    {
                        using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }        
    }

    /// <summary>
    /// Provides methods for encryping data.
    /// </summary>
    public static class EncryptionManager
    {
        #region Members

        private static readonly string CryptingKey;
        private static readonly string CryptingSalt;

        #endregion Members

        #region Constructors

        static EncryptionManager()
        {
            CryptingKey = ConfigurationManager.AppSettings["EncryptionKey"];
            CryptingSalt = ConfigurationManager.AppSettings["EncryptionSalt"];
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Encrypts the provided data with AES algorithm.
        /// </summary>
        /// <param name="data">The string data to be encrypted.</param>
        /// <returns>Encrypted data in Base64 format.</returns>
        public static string Encrypt(string data)
        {
            return EncryptionHelper.Encrypt<AesManaged>(data, CryptingKey, CryptingSalt);
        }

        /// <summary>
        /// Decrypts the provided data with AES algorithm.
        /// </summary>
        /// <param name="encryptData">The encrypted data.</param>
        /// <returns>String representing the decrypted data.</returns>
        public static string Decrypt(string encryptData)
        {
            return EncryptionHelper.Decrypt<AesManaged>(encryptData, CryptingKey, CryptingSalt);
        }        

        #endregion Public Methods
    }
}