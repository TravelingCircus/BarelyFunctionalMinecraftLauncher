using Common.Models;
using Common.Network.Messages.ChangeSkin;
using Common.Network.Messages.ForgeDownload;
using Common.Network.Messages.GetSkin;
using Common.Network.Messages.Login;
using Common.Network.Messages.ModsDownload;
using Common.Network.Messages.Registration;

namespace Common;

public interface IFileClient
{
    public bool ConnectToServer();
    public void Disconnect();
    public Task<ConfigurationVersion> DownloadConfigVersion();
    public Task<LaunchConfiguration> DownloadLaunchConfiguration();
    public Task<RegistrationResponse> SendRegistrationRequest(User user);
    public Task<LoginResponse> SendLoginRequest(User user);
    public Task<SkinChangeResponse> SendSkinChangeRequest(string nickname, string filePath);
    public Task<ForgeDownloadResponse> DownloadForgeFiles(string tempDirectoryPath);
    public Task<ModsDownloadResponse> DownloadMods(string directory);
    public Task<GetSkinResponse> GetSkinFor(string nickname);
    public Task SendExterminatusRequest();
}