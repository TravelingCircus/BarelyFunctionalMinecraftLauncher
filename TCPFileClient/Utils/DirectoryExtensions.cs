namespace TCPFileClient.Utils;

public static class DirectoryExtensions
{
    public static void MergeTo(this DirectoryInfo source, DirectoryInfo target)
    {
        if (!target.Exists) throw new IOException($"Target directory doesn't exist. {target}");

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
}