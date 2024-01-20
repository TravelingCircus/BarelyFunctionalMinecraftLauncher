using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
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

    protected abstract bool ForgeFilter(Forge forge);

    internal async Task<Forge[]> LoadForgeVersions(Version vanillaVersion)
    {
        return (await RepoIo.Forge.LoadAllVersions())
            .Where(ForgeFilter)
            .Where(forge => forge.TargetVanillaVersion == vanillaVersion)
            .ToArray();
    }

    internal async Task<Forge[]> LoadAllForgeVersions()
    {
        return (await RepoIo.Forge.LoadAllVersions())
            .Where(ForgeFilter)
            .ToArray();
    }

    internal async Task<Result<bool>> InstallForge(Forge forge, FileValidation validationMode)
    {
        if(await RepoIo.Forge.IsInstalled(forge, validationMode)) return Result<bool>.Ok(true);
        
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

    public Task<Result<bool>> InstallModPack(ModPack modPack, FileValidation validationMode)
    {
        return Task.FromResult(Result<bool>.Ok(true));
    }

    public Task<Result<Forge>> AddForgeWithDialogue()
    {
        Result<FileInfo> fileSelection = PlayFileSelectionDialogue("Forge Package Zip|*.zip");
        if (!fileSelection.IsOk) return Task.FromResult(Result<Forge>.Err(fileSelection.Error));
        
        FileInfo archiveFile = fileSelection.Value;
        if (archiveFile.Extension != ".zip") return Task.FromResult(Result<Forge>.Err(new IOException("Expected forge version archive.")));

        return RepoIo.Forge.AddVersion(archiveFile);
    }

    public Task<Result<bool>> RemoveForge(Forge forgeToRemove)
    {
        return RepoIo.Forge.DeleteVersion(forgeToRemove);
    }

    /// <param name="pattern">example: "Forge Package Zip|*.zip"</param>
    private Result<FileInfo> PlayFileSelectionDialogue(string pattern)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = "c:\\";
        openFileDialog.Filter = pattern;

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