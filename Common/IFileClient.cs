using Common.Models;

namespace Common;

public interface IFileClient
{
    public Task<ConfigurationVersion> DownloadConfigVersion();
    public Task<LaunchConfiguration> DownloadLaunchConfiguration();
}