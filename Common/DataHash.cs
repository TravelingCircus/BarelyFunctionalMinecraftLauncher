namespace Common;

public static class DataHash
{
    public static int ReadHashFromFiles(string path)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles();
        int hashCode = 0;
        foreach(FileInfo file in files)
        {
            hashCode += file.Name.GetHashCode();
            hashCode /=  2;
        }

        return hashCode;
    }
}