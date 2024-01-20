using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BFML.Core;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ConfigAdapter : RepoAdapter
{
    private FileInfo LocalPrefsFile => new FileInfo(AdapterDirectory.FullName + "\\LocalPrefs.xml");

    internal ConfigAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }

    internal Task<LocalPrefs> LoadLocalPrefs()
    {
        FileInfo prefsFile = LocalPrefsFile;
        if (!prefsFile.Exists)
        {
            LocalPrefs localPrefs = LocalPrefs.Default();
            SaveLocalPrefs(localPrefs);
            return Task.FromResult(localPrefs);
        }
        
        using FileStream fileStream = prefsFile.OpenRead();

        XmlSerializer serializer = new XmlSerializer(typeof(LocalPrefs));
        LocalPrefs loadedPrefs = serializer.Deserialize(fileStream) as LocalPrefs;
        return Task.FromResult(loadedPrefs);
    }

    internal Task<bool> SaveLocalPrefs(LocalPrefs prefs)
    {
        FileInfo prefsFile = LocalPrefsFile;
        if (prefsFile.Exists) prefsFile.Delete();

        Directory.CreateDirectory(prefsFile.Directory.FullName);
        using FileStream fileStream = prefsFile.Create();
        XmlSerializer xml = new XmlSerializer(typeof(LocalPrefs));
        xml.Serialize(fileStream, prefs);
        return Task.FromResult(true);
    }

    internal Task<bool> ClearLocalPrefs()
    {
        FileInfo prefsFile = LocalPrefsFile;
        if (prefsFile.Exists) prefsFile.Delete();
        return SaveLocalPrefs(new LocalPrefs());
    }
}