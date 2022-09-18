namespace FileClient.Utils;

public static class DirectoryExtensions
{
    public static DirectoryInfo CopyTo(this DirectoryInfo sourceDir, string destinationPath, bool overwrite = false)
    {
        string sourcePath = sourceDir.FullName;

        DirectoryInfo destination = new DirectoryInfo(destinationPath);

        destination.Create();

        foreach (string sourceSubDirPath in Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(sourceSubDirPath.Replace(sourcePath, destinationPath));

        foreach (string file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
            File.Copy(file, file.Replace(sourcePath, destinationPath), overwrite);

        return destination;
    }
}