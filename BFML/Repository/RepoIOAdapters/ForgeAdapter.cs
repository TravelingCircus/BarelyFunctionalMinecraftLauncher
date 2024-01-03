﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BFML.Core;
using Utils;

namespace BFML.Repository.RepoIOAdapters;

internal sealed class ForgeAdapter : RepoAdapter
{
    public const string CentralizedReservedForge = "CentralizedForge";
    
    internal ForgeAdapter(DirectoryInfo directory, RepoIO repoIo) : base(directory, repoIo) { }

    internal Task<Forge[]> LoadAllVersions() => Task.FromResult(EnumerateVersions().ToArray());

    internal IEnumerable<Forge> EnumerateVersions() => EnumerateVersionsInternal().Select(entry => entry.Forge);

    internal Task<bool> IsInstalled(Forge forge, FileValidation validationMode)
    {
        Result<(DirectoryInfo Version, DirectoryInfo Libs)> forgeFilesResult = FindForgeFiles(forge);
        if (!forgeFilesResult.IsOk) return Task.FromResult(false);

        (DirectoryInfo Version, DirectoryInfo Libs) forgeFiles = forgeFilesResult.Value;

        DirectoryInfo gameRoot = RepoIo.Configs.LoadLocalPrefs().Result.GameDirectory;

        SearchOption searchOption = validationMode > FileValidation.Heuristic
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;
        
        bool libs = forgeFiles.Libs.IsSubset(new DirectoryInfo(gameRoot + "\\libraries"), searchOption);
        bool version = forgeFiles.Version.IsSubset(new DirectoryInfo(gameRoot + $"\\versions\\{forge.Name}"), searchOption);

        return Task.FromResult(libs && version);
    }

    internal Task<bool> Install(Forge forge)
    {
        Result<(DirectoryInfo Version, DirectoryInfo Libs)> forgeFilesResult = FindForgeFiles(forge);
        if (!forgeFilesResult.IsOk) return Task.FromResult(false);

        (DirectoryInfo Version, DirectoryInfo Libs) forgeFiles = forgeFilesResult.Value;
        
        DirectoryInfo gameRoot = RepoIo.Configs.LoadLocalPrefs().Result.GameDirectory;

        DirectoryInfo gameLibs = new DirectoryInfo(gameRoot + "\\libraries");
        if (gameLibs.Exists) gameLibs.Delete(true);
        if (!forgeFiles.Libs.CopyTo(gameLibs, true)) return Task.FromResult(false);

        DirectoryInfo gameVersion = new DirectoryInfo(gameRoot + $"\\versions\\{forge.Name}");
        if (gameVersion.Exists) gameVersion.Delete(true);
        if (!forgeFiles.Version.CopyTo(gameVersion, true)) return Task.FromResult(false);
        
        return Task.FromResult(true);
    }

    private Result<Forge> LoadForgeDescription(DirectoryInfo directory)
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

    private Result<(DirectoryInfo Version, DirectoryInfo Libs)> FindForgeFiles(Forge forge)
    {
        DirectoryInfo forgeFiles = EnumerateVersionsInternal()
            .FirstOrDefault(entry => entry.Forge.Name == forge.Name).ForgeFiles;
        if (forgeFiles is null || !forgeFiles.Exists)
        {
            return Result<(DirectoryInfo Forge, DirectoryInfo Libs)>
                .Err(new Exception($"Forge version:{forge.Name} not found, or files directory does not exist."));
        }

        DirectoryInfo versionDirectory = new DirectoryInfo(forgeFiles + $"\\{forge.Name}");
        DirectoryInfo librariesDirectory = new DirectoryInfo(forgeFiles + "\\libraries");

        if (!versionDirectory.Exists)
        {
            return Result<(DirectoryInfo Version, DirectoryInfo Libs)>
                .Err(new Exception($"Forge version {forge.Name} does not have a version directory."));
        }
        
        if (!librariesDirectory.Exists)
        {
            return Result<(DirectoryInfo Version, DirectoryInfo Libs)>
                .Err(new Exception($"Forge version {forge.Name} does not have a `libraries` directory."));
        }
        
        return Result<(DirectoryInfo Version, DirectoryInfo Libs)>.Ok((versionDirectory, librariesDirectory));
    }
    
    private IEnumerable<(Forge Forge, DirectoryInfo ForgeFiles)> EnumerateVersionsInternal()
    {
        foreach (DirectoryInfo forgeDirectory in AdapterDirectory.GetDirectories())
        {
            Result<Forge> result = LoadForgeDescription(forgeDirectory);
            if (result.IsOk) yield return (result.Value, new DirectoryInfo(forgeDirectory+"\\ForgeFiles"));
        }
    }
}