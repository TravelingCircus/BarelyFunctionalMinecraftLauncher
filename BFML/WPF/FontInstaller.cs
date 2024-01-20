using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace BFML.WPF;

public static class FontInstaller
{
    public static void InstallFont(FileInfo file)
    {
        if (!file.Exists || file.Extension != ".ttf") throw new FileLoadException($"Font {file.FullName} doesn't exist or has wrong extension");

        InstalledFontCollection installedFontCollection = new InstalledFontCollection();
        if (installedFontCollection.Families.Any(family => family.Name == "Minecraft")) return;
        
        int _ = AddFontResource(file.FullName);
    }
    
    [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
    private static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
}