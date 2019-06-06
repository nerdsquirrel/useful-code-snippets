public class AESEncryption
{        
    public static string Encrypt(string inputText, string password)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        byte[] plainText = Encoding.Unicode.GetBytes(inputText);

        // We are using salt to make it harder to guess our key
        // using a dictionary attack.

        byte[] salt = Encoding.UTF8.GetBytes(password.Length.ToString());

        // The (Secret Key) will be generated from the specified 

        // password and salt.

        PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);

        // Create a encryptor from the existing SecretKey bytes.

        // We use 32 bytes for the secret key 

        // (the default Rijndael key length is 256 bit = 32 bytes) and

        // then 16 bytes for the IV (initialization vector),

        // (the default Rijndael IV length is 128 bit = 16 bytes)

        ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

        // Create a MemoryStream that is going to hold the encrypted bytes 

        MemoryStream memoryStream = new MemoryStream();

        // Create a CryptoStream through which we are going to be processing our data. 

        // CryptoStreamMode.Write means that we are going to be writing data 

        // to the stream and the output will be written in the MemoryStream

        // we have provided. (always use write mode for encryption)

        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        // Start the encryption process.

        cryptoStream.Write(plainText, 0, plainText.Length);

        // Finish encrypting.

        cryptoStream.FlushFinalBlock();

        // Convert our encrypted data from a memoryStream into a byte array.

        byte[] cipherBytes = memoryStream.ToArray();

        // Close both streams.

        memoryStream.Close();

        cryptoStream.Close();

        // Convert encrypted data into a base64-encoded string.

        // A common mistake would be to use an Encoding class for that. 

        // It does not work, because not all byte values can be

        // represented by characters. We are going to be using Base64 encoding

        // That is designed exactly for what we are trying to do. 

        string encryptedData = Convert.ToBase64String(cipherBytes);

        // Return encrypted string.

        return encryptedData;

    }

    public static string Decrypt(string inputText, string password)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        byte[] encryptedData = Convert.FromBase64String(inputText);

        byte[] salt = Encoding.UTF8.GetBytes(password.Length.ToString());

        PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
        // Create a decryptor from the existing SecretKey bytes.

        ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

        MemoryStream memoryStream = new MemoryStream(encryptedData);

        // Create a CryptoStream. (always use Read mode for decryption).

        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        // Since at this point we don't know what the size of decrypted data

        // will be, allocate the buffer long enough to hold EncryptedData;

        // DecryptedData is never longer than EncryptedData.

        byte[] plainText = new byte[encryptedData.Length];
        // Start decrypting.

        int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
        memoryStream.Close();

        cryptoStream.Close();
        // Convert decrypted data into a string. 

        string decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
        // Return decrypted string.   

        return decryptedData;
    }

}
