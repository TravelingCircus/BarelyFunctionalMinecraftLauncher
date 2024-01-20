using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BFML.Core;
using Utils;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ModPackAdapter : RepoAdapter
{
    public const string CentralizedReservedModPack = "CentralizedModpack";
    
    internal ModPackAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }
    
    internal Task<ModPack[]> LoadModPackList() => Task.FromResult(EnumerateVersions().ToArray());

    internal IEnumerable<ModPack> EnumerateVersions() => EnumerateVersionsInternal().Select(entry => entry.ModPack);

    internal Task<bool> IsInstalled(ModPack modPack, FileValidation validationMode)
    {
        Result<DirectoryInfo> modsResult = FindMods(modPack);
        if (!modsResult.IsOk) return Task.FromResult(false);

        DirectoryInfo mods = modsResult.Value;
        DirectoryInfo gameRoot = RepoIo.Configs.LoadLocalPrefs().Result.GameDirectory;
        SearchOption searchOption = validationMode > FileValidation.Heuristic
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;
        
        return Task.FromResult(mods.IsSubset(new DirectoryInfo(gameRoot + "\\mods"), searchOption));
    }

    internal Task<bool> Install(ModPack modPack)
    {
        Result<DirectoryInfo> modsResult = FindMods(modPack);
        if (!modsResult.IsOk) return Task.FromResult(false);

        DirectoryInfo mods = modsResult.Value;
        DirectoryInfo gameRoot = RepoIo.Configs.LoadLocalPrefs().Result.GameDirectory;
        DirectoryInfo gameMods = new DirectoryInfo(gameRoot + "\\mods");
        
        if (gameMods.Exists) gameMods.Delete(true);
        
        return Task.FromResult(mods.CopyTo(gameMods, true));
    }

    public Task<Result<ModPack>> AddVersion(FileInfo archiveFile)
    {
        if (!archiveFile.Exists || archiveFile.Extension != ".zip")
        {
            return Task.FromResult(Result<ModPack>.Err(new IOException("Expected mod pack archive.")));
        }
        
        using ZipArchive zipArchive = new ZipArchive(archiveFile.OpenRead());
        DirectoryInfo temp = new DirectoryInfo(RepoIo.Temp + $"\\ModPacks\\{archiveFile.Name}");

        try
        {
            zipArchive.ExtractToDirectory(temp.FullName, true);

            Result<ModPack> sourceFileValidation = ValidateModPackDirectory(temp);
            if (!sourceFileValidation.IsOk) throw sourceFileValidation.Error;

            DirectoryInfo target = new DirectoryInfo(AdapterDirectory + $"\\{sourceFileValidation.Value.Name}");
            temp.CopyTo(target, true, true);

            return Task.FromResult(ValidateModPackDirectory(target));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<ModPack>.Err(e));
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }

    public Task<Result<bool>> DeleteVersion(ModPack modPackToRemove)
    {
        Result<DirectoryInfo> filesSearch = FindMods(modPackToRemove);
        if (!filesSearch.IsOk) return Task.FromResult(Result<bool>.Err(filesSearch.Error));

        try
        {
            Directory.Delete(filesSearch.Value.Parent!.FullName, true);
            return Task.FromResult(Result<bool>.Ok(true));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<bool>.Err(e));
        }
    }

    private Result<DirectoryInfo> FindMods(ModPack modPack)
    {
        DirectoryInfo modsDirectory = EnumerateVersionsInternal()
            .FirstOrDefault(entry => entry.ModPack.Name == modPack.Name).Mods;
        if (modsDirectory is null || !modsDirectory.Exists)
        {
            return Result<DirectoryInfo>
                .Err(new Exception($"ModPack:{modPack.Name} not found, or mods directory does not exist."));
        }

        if (!modsDirectory.Exists)
        {
            return Result<DirectoryInfo>
                .Err(new Exception($"ModPack {modPack.Name} does not have a mods directory."));
        }
        
        return Result<DirectoryInfo>.Ok(modsDirectory);
    }

    private static Result<ModPack> ValidateModPackDirectory(DirectoryInfo modPackDirectory)
    {
        Result<ModPack> manifestLoading = LoadModPackDescription(modPackDirectory);
        if (!manifestLoading.IsOk) return manifestLoading;

        ModPack manifest = manifestLoading.Value;
        DirectoryInfo modsDirectory = new DirectoryInfo(modPackDirectory + "\\Mods");

        return modsDirectory.Exists
            ? Result<ModPack>.Ok(manifest)
            : Result<ModPack>.Err(new IOException($"ModPack {manifest.Name} does not have a mods directory."));
    }

    private IEnumerable<(ModPack ModPack, DirectoryInfo Mods)> EnumerateVersionsInternal()
    {
        return AdapterDirectory.GetDirectories()
            .Select(modPackDirectory => new { modPackDirectory, modPack = LoadModPackDescription(modPackDirectory) })
            .Where(tuple => tuple.modPack.IsOk)
            .Select(tuple => (tuple.modPack.Value, new DirectoryInfo(tuple.modPackDirectory + "\\Mods")));
    }

    private static Result<ModPack> LoadModPackDescription(DirectoryInfo directory)
    {
        try
        {
            FileInfo manifestFile = new FileInfo(directory.FullName + "\\Manifest.xml");
            if (!manifestFile.Exists) return Result<ModPack>.Err(new InvalidDataException("Manifest file does not exist."));

            using FileStream manifestFileStream = File.OpenRead(manifestFile.FullName);
            ModPack modPack = new XmlSerializer(typeof(ModPack)).Deserialize(manifestFileStream) as ModPack;
            return Result<ModPack>.Ok(modPack);
        }
        catch (Exception e)
        {
            return Result<ModPack>.Err(e);
        }
    }
}