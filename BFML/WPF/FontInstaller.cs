using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace BFML.WPF;

public static class FontInstaller
{
    public static void InstallFont(FileInfo file)
    {
        if (!file.Exists || file.Extension != ".ttf") throw new FileLoadException($"Font {file.FullName} doesn't exist or has wrong extension");
        
        int result = AddFontResource(file.FullName);
        
        int error = Marshal.GetLastWin32Error();
        if (error != 0) throw new Win32Exception(error);
    }
    
    [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
    private static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
        string lpFileName);
}