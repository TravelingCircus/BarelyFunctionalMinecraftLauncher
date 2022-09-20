using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using BFML._3D;
using FileClient;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;

namespace BFML.WPF;

public partial class MainWindow : Window
{
    private readonly SkinPreviewRenderer _skinPreviewRenderer;

    public MainWindow()
    {
        InitializeComponent();
        GLWpfControlSettings settings = new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 0
        };
        //OpenTkControl.Start(settings);
        _skinPreviewRenderer = new SkinPreviewRenderer();
        _skinPreviewRenderer.SetUp();
    }

    private void PlayButtonOnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
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

    private void InstallButtonOnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void SkinPreviewOnRender(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        await _skinPreviewRenderer.Render(obj).ConfigureAwait(false);
    }

    private void ServerButtonOnClick(object sender, RoutedEventArgs e)
    {
        BFMLFileClient.ConnectToServer();
    }
}