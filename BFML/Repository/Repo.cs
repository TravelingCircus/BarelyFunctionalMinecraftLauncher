using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using BFML.Core;
using Common;
using Utils;
using Version = Utils.Version;

namespace BFML.Repository;

internal abstract class Repo
{
    internal LocalPrefs LocalPrefs { get; private set; }
    protected readonly RepoIO RepoIo;

    protected Repo(RepoIO repoIo)
    {
        RepoIo = repoIo;
    }

    public virtual async Task<bool> TryInit()
    {
        if (! ValidateRepository()) return false;
        
        Result<LocalPrefs> loadResult = await RepoIo.Configs.LoadLocalPrefs();
        if (loadResult.IsOk) LocalPrefs = loadResult.Value; 
        return loadResult.IsOk;
    }

    protected abstract bool ForgeFilter(Forge forge);

    internal async Task<Forge[]> LoadForgeVersions(Version vanillaVersion)
    {
        return (await RepoIo.Forge.LoadAllVersions())
            .Where(ForgeFilter)
            .Where(forge => forge.TargetVanillaVersion.ToString() == vanillaVersion.ToString())
            .ToArray();
    }

    internal async Task<Forge[]> LoadAllForgeVersions()
    {
        return (await RepoIo.Forge.LoadAllVersions())
            .Where(ForgeFilter)
            .ToArray();
    }

    internal async Task<bool> ValidateForgeInstalled(Forge forge, FileValidation validationMode)
    {
        bool wasInstalled = await RepoIo.Forge.IsInstalled(forge, validationMode);

        return await RepoIo.Forge.Install(forge);
    }

    protected abstract bool ModPackFilter(ModPack modPack);

    internal async Task<ModPack[]> LoadModPackList()
    {
        return (await RepoIo.ModPacks.LoadModPackList())
            .Where(ModPackFilter)
            .ToArray();
    }

    internal Task<Skin> LoadDefaultSkin() => RepoIo.Resources.LoadDefaultSkin();

    internal async Task<bool> SaveLocalPrefs(LocalPrefs localPrefs)
    {
        bool success = await RepoIo.Configs.SaveLocalPrefs(localPrefs);
        if (success) LocalPrefs = localPrefs;
        return success;
    }

    internal async Task<bool> ClearLocalPrefs()
    {
        bool success = await RepoIo.Configs.ClearLocalPrefs();
        if(success) LocalPrefs = new LocalPrefs();
        return success;
    }

    #region RepositoryValidation

    private bool ValidateRepository()
    { 
        XmlDocument structure = RepoIo.ReadRepoStructure();
        
        if (!ValidateRepositoryRecursive(structure.FirstChild, String.Empty)) return false;

        return true;
    }

    private bool ValidateRepositoryRecursive(XmlNode node, string parentPath)
    {
        foreach (XmlNode subNode in node)
        {
            if (subNode is not XmlElement subElement) continue;
            if (!ValidateRepositoryRecursive(subElement, parentPath + $"\\{node.LocalName}")) return false;
        }

        return ValidateRepositoryElement(node as XmlElement, parentPath);
    }

    private bool ValidateRepositoryElement(XmlElement element, string parentPath)
    {
        return element.ChildNodes.Count == 1 && element.ChildNodes[0] is not XmlElement
            ? ValidateFile(new FileInfo(RepoIo.Root.Parent + parentPath + $"\\{element.InnerText}.{element.LocalName.ToLower()}"))
            : ValidateDirectory(new DirectoryInfo(RepoIo.Root.Parent + parentPath + $"\\{element.LocalName}"));
    }

    private bool ValidateFile(FileInfo file)
    {
        if (!file.Exists)
        {
            if (file.Name == "LocalPrefs") RepoIo.Configs.ClearLocalPrefs();
        }
        return file.Exists;
    }

    private bool ValidateDirectory(DirectoryInfo directory)
    {
        if (!directory.Exists) directory.Create();
        return directory.Exists;
    }

    #endregion
}