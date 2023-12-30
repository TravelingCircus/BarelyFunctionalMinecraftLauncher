namespace Common;

public static class Checksum
{
    public static uint FromDirectory(DirectoryInfo directory)
    {
        if (!directory.Exists) throw new DirectoryNotFoundException($"{directory}");
        FileInfo[] files = directory.GetFiles();
        uint[] hashes = new uint[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            hashes[i] = unchecked((uint)files[i].Length/(uint)files.Length);
        }

        return hashes.Aggregate<uint, uint>(53214, (current, t) => current + t);
    }
}