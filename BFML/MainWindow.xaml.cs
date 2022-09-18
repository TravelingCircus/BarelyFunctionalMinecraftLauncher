using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;

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

            /*launcher.VersionLoader = new LocalVersionLoader(launcher.MinecraftPath);
            launcher.FileDownloader = null;*/
            
            launcher.FileChanged += (e) =>
            {
                Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
            };
            launcher.ProgressChanged += (_, e) =>
            {
                Console.WriteLine("{0}%", e.ProgressPercentage);
            };

            MVersionCollection versions = await launcher.GetAllVersionsAsync();
            foreach (MVersionMetadata v in versions)
            {
                Console.WriteLine(v.Name);
            }

            Process process = await launcher.CreateProcessAsync("1.7.10", new MLaunchOption
            {
                MaximumRamMb = 8192,
                Session = MSession.GetOfflineSession("hello123"),
            });

            process.Start();
        }

        private async void DownloadButtonOnClick(object sender, RoutedEventArgs e)
        {
            await DownloadSomethingViaHttpClient();
        }

       private async Task DownloadSomethingViaHttpClient()
        {
            Uri url = new Uri("https://upload.wikimedia.org/wikipedia/commons/4/47/PNG_transparency_demonstration_1.png");
            string savePath = @"D:\Home\Desktope\TestDownload\PNG_transparency_demonstration_1.png";
            
            using HttpClient client = new HttpClient();
            using Stream stream = await client.GetStreamAsync(url);
            using FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            await stream.CopyToAsync(fileStream);
        }

       private void InstallButtonOnClick(object sender, RoutedEventArgs e)
       {
           throw new NotImplementedException();
       }
    }
}