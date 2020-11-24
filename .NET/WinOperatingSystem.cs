public class WinOperatingSystem
{
    public WinOperatingSystem()
    { }

    /// <summary>
    /// Returns true if Windows 64-bit Enviroment else it will return
    /// false. 
    /// </summary>
    /// <returns></returns>
    public bool Is64() { return IntPtr.Size == 8; }

    /// <summary>
    /// Return true if Windows 7. 
    /// </summary>
    /// <returns></returns>
    public bool IsWindows7()
    {
        return Environment.OSVersion.Version.ToString() == "6.1.7600.0";
    }

    /// <summary>
    /// Return true if Windows XP. 
    /// </summary>
    /// <returns></returns>
    public bool IsWindowsXP()
    {
        return Environment.OSVersion.Version.ToString() == "5.1.2600.0";
    }

    /// <summary>
    /// Return true if Windows XP 64-bit.
    /// </summary>
    /// <returns></returns>
    public bool IsWindowsXP64()
    {
        return Environment.OSVersion.Version.ToString() == "5.2.3790.0";
    }

    /// <summary>
    /// Return true if Windows Vista.
    /// </summary>
    /// <returns></returns>
    public bool IsWindowsVista()
    {
        return Environment.OSVersion.Version.ToString() == "6.0.6000.0";
    }

    /// <summary>
    /// Return true if Windows Vista + SP2.
    /// </summary>
    /// <returns></returns>
    public bool IsWindowsVistaSP2()
    {
        return Environment.OSVersion.Version.ToString() == "6.0.6002.0";
    }
}