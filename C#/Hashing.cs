public class Hashing
{
    public static string GenerateSaltedHash(string targetText, string salt)
    {
        byte[] plainTextInByte = Encoding.UTF8.GetBytes(targetText);
        byte[] saltInByte = Encoding.UTF8.GetBytes(salt);
        string result = Convert.ToBase64String(GenerateSaltedHash(plainTextInByte, saltInByte)).Replace("/", string.Empty);
        return result;
    }

    private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
    {
        HashAlgorithm algorithm = new SHA256Managed();
        byte[] plainTextWithSaltBytes = plainText.Concat(salt).ToArray();
        return algorithm.ComputeHash(plainTextWithSaltBytes);
    }       
    
}