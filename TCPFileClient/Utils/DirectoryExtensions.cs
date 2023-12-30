namespace FileClient.Utils;

public static class DirectoryExtensions
{
    public static void MergeTo(this DirectoryInfo source, DirectoryInfo target)
    {
        if (!target.Exists) Directory.CreateDirectory(target.FullName);

        foreach (FileInfo file in source.GetFiles())
        {
            file.MoveTo(target.FullName + $"\\{file.Name}", true);
        }

        foreach (DirectoryInfo directory in source.GetDirectories())
        {
            string destination = target.FullName + $"\\{directory.Name}";
            DirectoryInfo collision = new DirectoryInfo(destination);
            if (collision.Exists)
            {
                directory.MergeTo(collision);
                continue;
            }
            directory.MoveTo(destination);
        }
    }

    public static int RoughSize(this DirectoryInfo directory)
    {
        FileInfo[] fis = directory.GetFiles();
        int size = fis.Sum(file => (int) file.Length);
        DirectoryInfo[] directories = directory.GetDirectories();
        size += directories.Sum(innerDirectory => innerDirectory.RoughSize());
        return size;  
    }
}