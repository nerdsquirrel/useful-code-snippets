public class RandomNumberGenerator
{
    public static string GenerateRandomNumberByLength(int length)
    {
        return Guid.NewGuid().ToString().Substring(0, length).ToUpper();
    }
}