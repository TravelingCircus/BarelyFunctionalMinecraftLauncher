using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using BFML._3D;
using BFML.Core;
using Common;
using Microsoft.Win32;
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
        Result<LocalPrefs> loadResult = await RepoIo.Configs.LoadLocalPrefs();
        if (loadResult.IsOk) LocalPrefs = loadResult.Value; 
        return loadResult.IsOk;
    }

    public bool Validate() => RepoIo.ValidateRepository();

#region Forge
    protected abstract bool ForgeFilter(Forge forge);

    internal async Task<Forge[]> LoadForgeVersions(Version vanillaVersion)
    {
        return (await RepoIo.Forge.LoadAllVersions())
            .Where(ForgeFilter)
            .Where(forge => forge.TargetVanillaVersion == vanillaVersion)
            .ToArray();
    }

    internal async Task<Forge[]> LoadForgeVersions()
    {
        return (await RepoIo.Forge.LoadAllVersions())
            .Where(ForgeFilter)
            .ToArray();
    }

    internal async Task<Result<bool>> InstallForge(Forge forge, FileValidation validationMode)
    {
        return await RepoIo.Forge.IsInstalled(forge, validationMode) switch
        {
            true => Result<bool>.Ok(true),
            false => await RepoIo.Forge.Install(forge)
        };
    }
    
    internal Task<Result<Forge>> AddForgeWithDialogue()
    {
        Result<FileInfo> fileSelection = PlayFileSelectionDialogue("Forge Package Zip|*.zip");
        if (!fileSelection.IsOk) return Task.FromResult(Result<Forge>.Err(fileSelection.Error));
        
        FileInfo archiveFile = fileSelection.Value;
        return archiveFile.Extension == ".zip" 
            ? RepoIo.Forge.AddVersion(archiveFile) 
            : Task.FromResult(Result<Forge>.Err(new IOException("Expected forge version archive.")));
    }

    internal Task<Result<bool>> RemoveForge(Forge forgeToRemove) 
    {
        return RepoIo.Forge.DeleteVersion(forgeToRemove);
    }
#endregion

#region ModPacks
    protected abstract bool ModPackFilter(ModPack modPack);

    internal async Task<ModPack[]> LoadModPacks(Forge forge)
        {
            return (await RepoIo.ModPacks.LoadModPackList())
                .Where(ModPackFilter)
                .Where(modPack => modPack.VanillaVersion == forge.TargetVanillaVersion
                                  && modPack.ForgeVersion == forge.SubVersion)
                .ToArray();
        }

    internal async Task<ModPack[]> LoadModPacks()
    {
        return (await RepoIo.ModPacks.LoadModPackList())
            .Where(ModPackFilter)
            .ToArray();
    }
    
    internal async Task<Result<bool>> InstallModPack(ModPack modPack, FileValidation validationMode) 
    {
        return await RepoIo.ModPacks.IsInstalled(modPack, validationMode) switch
        {
            true => Result<bool>.Ok(true),
            false => await RepoIo.ModPacks.Install(modPack)
        };
    }
    
    internal Task<Result<ModPack>> AddModPackWithDialogue() 
    {
        Result<FileInfo> fileSelection = PlayFileSelectionDialogue("ModPack Package Zip|*.zip");
        if (!fileSelection.IsOk) return Task.FromResult(Result<ModPack>.Err(fileSelection.Error));
        
        FileInfo archiveFile = fileSelection.Value;
        return archiveFile.Extension == ".zip" 
            ? RepoIo.ModPacks.AddVersion(archiveFile) 
            : Task.FromResult(Result<ModPack>.Err(new IOException("Expected mod pack version archive.")));
    }
    
    internal Task<Result<bool>> RemoveModPack(ModPack modPack)
    {
        return RepoIo.ModPacks.DeleteVersion(modPack);
    }
#endregion

    internal Task<Skin> LoadDefaultSkin() => RepoIo.Resources.LoadDefaultSkin();
    
    public Task<Texture> LoadShadowTexture()
    {
        return RepoIo.Resources.LoadShadowImage();
    }

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

    /// <param name="pattern">example: "Forge Package Zip|*.zip"</param>
    private static Result<FileInfo> PlayFileSelectionDialogue(string pattern)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            InitialDirectory = "c:\\",
            Filter = pattern
        };

        try
        {
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                FileInfo file = new FileInfo(openFileDialog.FileName);
                return file.Exists ? Result<FileInfo>.Ok(file) : Result<FileInfo>.Err(new IOException("Selected file doesn't exist."));
            }
        }
        catch (Exception e)
        {
            return Result<FileInfo>.Err(e);
        }

        return Result<FileInfo>.Err(new Exception("File selection dialogue was cancelled."));
    }
}