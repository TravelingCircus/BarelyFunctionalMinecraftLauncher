namespace FileClient.Utils;

public static class DirectoryExtensions
{
    public static DirectoryInfo CopyTo(this DirectoryInfo sourceDir, string destinationPath, bool overwrite = false)
    {
        var sourcePath = sourceDir.FullName;

        var destination = new DirectoryInfo(destinationPath);

        destination.Create();

        foreach (var sourceSubDirPath in Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(sourceSubDirPath.Replace(sourcePath, destinationPath));

        foreach (var file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
            File.Copy(file, file.Replace(sourcePath, destinationPath), overwrite);

        return destination;
    }
}