using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BFML.Core;
using Utils;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ForgeAdapter : RepoAdapter
{
    public const string CentralizedReservedForge = "CentralizedForge";
    
    internal ForgeAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }

    internal Task<Forge[]> LoadForgeList()
    {
        List<Forge> forges = new List<Forge>();
        foreach (DirectoryInfo forgeDirectory in AdapterDirectory.GetDirectories())
        {
            Result<Forge> result = LoadForge(forgeDirectory);
            if (result.IsOk) forges.Add(result.Value);
        }
        return Task.FromResult(forges.ToArray());
    }

    internal Task<bool> IsInstalled(Forge forge)
    {
        throw new NotImplementedException();
        DirectoryInfo librariesDirectory = new DirectoryInfo(RepoIo.Configs.LoadLocalPrefs().Result.GameDirectory+"\\libraries");
    }

    internal Task<bool> Install(Forge forge)
    {
        throw new NotImplementedException();
    }

    private Result<Forge> LoadForge(DirectoryInfo directory)
    {
        try
        {
            FileInfo manifestFile = new FileInfo(directory.FullName + "\\Manifest.xml");
            if (!manifestFile.Exists) return Result<Forge>.Err(new InvalidDataException("Manifest file does not exist."));

            using FileStream manifestFileStream = File.OpenRead(manifestFile.FullName);
            Forge forge = new XmlSerializer(typeof(Forge)).Deserialize(manifestFileStream) as Forge;
            return Result<Forge>.Ok(forge);
        }
        catch (Exception e)
        {
            return Result<Forge>.Err(e);
        }
    }
    
    private DirectoryInfo FindForgeDirectory()
    {
        throw new NotImplementedException();
    } 
}