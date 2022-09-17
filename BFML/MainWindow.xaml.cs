using System;
using System.Diagnostics;
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
    }
}