using System.Threading.Tasks;
using CmlLib.Core;

namespace BFML.Core;

public class Game
{
    public readonly string VanillaVersion = "1.16.5";
    private readonly CMLauncher _launcher;
    private readonly MinecraftPath _minecraftPath;
    
    public Game()
    {
        _minecraftPath = new MinecraftPath();
        _launcher = new CMLauncher(_minecraftPath);
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