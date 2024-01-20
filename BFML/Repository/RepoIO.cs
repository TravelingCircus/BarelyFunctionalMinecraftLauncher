using System;
using System.IO;
using System.Xml;
using Common.Misc;
using BFML.Repository.RepoIOAdapters;
using Utils;

namespace BFML.Repository;

internal sealed class RepoIO
{
    public DirectoryInfo Root => _repoInfo;
    public DirectoryInfo Temp => new DirectoryInfo(_repoInfo.FullName + "\\Temp");
    private DirectoryInfo ForgeDirectory => new DirectoryInfo(_repoInfo.FullName + "\\Forge");
    private DirectoryInfo ConfigsDirectory => new DirectoryInfo(_repoInfo.FullName + "\\Configs");
    private DirectoryInfo ModPacksDirectory => new DirectoryInfo(_repoInfo.FullName + "\\ModPacks");
    private DirectoryInfo ResourcesDirectory => new DirectoryInfo(_repoInfo.FullName + "\\Resources");
    
    internal readonly ForgeAdapter Forge;
    internal readonly ConfigAdapter Configs;
    internal readonly ModPackAdapter ModPacks;
    internal readonly ResourceAdapter Resources;
    private readonly DirectoryInfo _repoInfo;

    internal RepoIO(DirectoryInfo repoInfo)
    {
        _repoInfo = repoInfo;
        Forge = new ForgeAdapter(ForgeDirectory, this);
        Configs = new ConfigAdapter(ConfigsDirectory, this);
        ModPacks = new ModPackAdapter(ModPacksDirectory, this);
        Resources = new ResourceAdapter(ResourcesDirectory, this);
    }

    internal XmlDocument ReadRepoStructure()
    {
        XmlDocument document = new XmlDocument();
        document.Load(_repoInfo.Parent + "\\Repo.xml");
        return document;
    }
    
    #region RepositoryValidation

    internal bool ValidateRepository()
    { 
        XmlDocument structure = ReadRepoStructure();
        
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
            ? ValidateFile(new FileInfo(Root.Parent + parentPath + $"\\{element.InnerText}.{element.LocalName.ToLower()}"))
            : ValidateDirectory(new DirectoryInfo(Root.Parent + parentPath + $"\\{element.LocalName}"));
    }

    private bool ValidateFile(FileInfo file)
    {
        if (!file.Exists)
        {
            if (file.Name == "LocalPrefs.xml") Configs.ClearLocalPrefs();
        }
        return new FileInfo(file.FullName).Exists;
    }

    private bool ValidateDirectory(DirectoryInfo directory)
    {
        if (!directory.Exists) directory.Create();
        return directory.Exists;
    }

    #endregion
}