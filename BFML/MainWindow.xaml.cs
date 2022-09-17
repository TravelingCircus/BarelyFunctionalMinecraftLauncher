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
            LaunchMinecraft();
        }

        private async void LaunchMinecraft()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            MinecraftPath path = new MinecraftPath();
            CMLauncher launcher = new CMLauncher(path);

            launcher.FileChanged += (e) =>
            {
                Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
            };
            launcher.ProgressChanged += (s, e) =>
            {
                Console.WriteLine("{0}%", e.ProgressPercentage);
            };

            MVersionCollection versions = await launcher.GetAllVersionsAsync();
            foreach (MVersionMetadata v in versions)
            {
                Console.WriteLine(v.Name);
            }

            Process process = await launcher.CreateProcessAsync("1.16.5", new MLaunchOption
            {
                MaximumRamMb = 2048,
                Session = MSession.GetOfflineSession("hello123"),
            });

            process.Start();
        }
    }
}