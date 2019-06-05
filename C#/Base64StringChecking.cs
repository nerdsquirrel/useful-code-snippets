/// <summary>
/// Checks whether the provided string is in Base64 format.
/// </summary>
public static bool IsBase64String(this string s)
{
    s = s.Trim();
    return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
}