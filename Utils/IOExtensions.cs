namespace Utils;

public static class IOExtensions
{
    public static bool CopyTo(this DirectoryInfo source, DirectoryInfo target, bool recursive, bool clearTarget = false)
    {
        if (!source.Exists) return false;
        if (source == target) throw new InvalidOperationException("Can't copy to source directory.");
        
        DirectoryInfo[] dirs = source.GetDirectories();
        if (target.Exists && clearTarget) Directory.Delete(target.FullName, true);
        target.Create();
        
        foreach (FileInfo file in source.GetFiles())
        {
            string targetFilePath = Path.Combine(target.FullName, file.Name);
            file.CopyTo(targetFilePath);
        }
        
        if (!recursive) return true;

        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(target.FullName, subDir.Name);
            CopyTo(subDir, new DirectoryInfo(newDestinationDir), true);
        }
        
        return true;
    }
    
    public static bool IsSubset(this DirectoryInfo self, DirectoryInfo other, SearchOption searchOption = SearchOption.AllDirectories)
    {
        if (!self.Exists || !other.Exists) return false;
        
        FileInfo[] sourceFiles = self.GetFiles("*", searchOption);
        DirectoryInfo[] sourceDirs = self.GetDirectories("*", searchOption);

        foreach (FileInfo file in sourceFiles)
        {
            string relativePath = GetRelativePath(file.FullName, self.FullName);
            string targetFilePath = Path.Combine(other.FullName, relativePath);

            if (!File.Exists(targetFilePath))
            {
                return false;
            }
        }

        foreach (DirectoryInfo dir in sourceDirs)
        {
            string relativePath = GetRelativePath(dir.FullName, self.FullName);
            string targetDirPath = Path.Combine(other.FullName, relativePath);

            if (!Directory.Exists(targetDirPath))
            {
                return false;
            }
        }

        return true;
    }
    
    private static string GetRelativePath(string fullPath, string basePath)
    {
        return Path.GetRelativePath(basePath, fullPath);
    }
}