using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using FileClient;
using FileClient.Utils;

namespace BFML
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PlayButtonOnClick(object sender, RoutedEventArgs e)
        {
            ShitLabel.Content = "Loading";
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            MinecraftPath path = new MinecraftPath();
            CMLauncher launcher = new CMLauncher(path);

            MVersionCollection versions = await launcher.GetAllVersionsAsync();

            Process process = await launcher.CreateProcessAsync(versions[1].Name, new MLaunchOption
            {
                MaximumRamMb = 8192,
                Session = MSession.GetOfflineSession("hello123")
            });

            process.Start();
        }

        private async void DownloadButtonOnClick(object sender, RoutedEventArgs e)
        {
            await DownloadSomethingViaHttpClient();
        }

        private async Task DownloadSomethingViaHttpClient()
        {
            Uri url = new Uri(
                "https://upload.wikimedia.org/wikipedia/commons/4/47/PNG_transparency_demonstration_1.png");
            string savePath = @"C:\Users\maksy\Desktope\TestDownload\PNG_transparency_demonstration_1.png";

            using HttpClient client = new HttpClient();
            await using Stream stream = await client.GetStreamAsync(url);
            await using FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            await stream.CopyToAsync(fileStream);
        }

        private async void InstallButtonOnClick(object sender, RoutedEventArgs e)
        {
            MinecraftPath path = new MinecraftPath();
            CMLauncher launcher = new CMLauncher(path);

            await launcher.CheckAndDownloadAsync(await launcher.GetVersionAsync("1.16.5"));

            await InstallForge();
        }

        private async Task InstallForge()
        {
            MinecraftPath minecraftPath = new MinecraftPath();
            using TempDirectory tempDirectory = new TempDirectory();
            BFMLFileClient fileClient = BFMLFileClient.ConnectToServer();

            await fileClient.DownloadForgeFiles(tempDirectory.Info.FullName).ConfigureAwait(false);
            foreach (DirectoryInfo directory in tempDirectory.Info.GetDirectories())
            {
                if (directory.Name == "libraries")
                {
                    directory.CopyTo(minecraftPath.Library, true);
                }
                else if (directory.Name.Contains("-forge-"))
                {
                    //Directory.Move(directory.FullName, );
                }
                else throw new InvalidDataException($"Unexpected directory: {directory.Name}");
            }
        }
    }
}