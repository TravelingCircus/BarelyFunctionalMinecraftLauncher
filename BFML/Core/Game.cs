using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;

namespace BFML.Core;

public class Game
{
    public readonly MVersion VanillaVersion = new MVersion("1.16.5");
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;
    private readonly Vanilla _vanilla;
    private readonly Forge _forge;
    
    public Game()
    {
        _minecraftPath = new MinecraftPath();
        _launcher = new CMLauncher(_minecraftPath);
        _vanilla = new Vanilla(_minecraftPath, VanillaVersion);
        _forge = new Forge(_minecraftPath, VanillaVersion);
    }

    public Task Install()
    {
        return Task.CompletedTask;
    }
    
    public Task<bool> IsReadyToLaunch()
    {
        return Task.FromResult(true);
    }

    public Task DeleteAllFiles()
    {
        return Task.CompletedTask;
    }
    
    private Task InstallMinecraft()
    {
        return Task.CompletedTask;
    }
    
    private Task InstallForge()
    {
        return Task.CompletedTask;
    }
    
}