using System.Windows;
using System.Windows.Controls;
using Common.Misc;
using XamlRadialProgressBar;

namespace BFML.WPF;

internal sealed class LoadingScreen
{
    private readonly Border _loading;
    private readonly RadialProgressBar _progressBar;
    private readonly TextBlock _progressText;
    private CompositeProgress _progress;

    public LoadingScreen(Border loading, RadialProgressBar progressBar, TextBlock progressText)
    {
        _loading = loading;
        _progressBar = progressBar;
        _progressText = progressText;
    }

    public CompositeProgress Show()
    {
        _loading.Visibility = Visibility.Visible;
        _progressBar.Value = 0f;
        _progressText.Text = "0";
        _progress = new CompositeProgress();
        _progress.Changed += OnProgressChange;
        return _progress;
    }

    public void Hide()
    {
        _loading.Visibility = Visibility.Collapsed;
        _progress.Changed -= OnProgressChange;
    }

    private void OnProgressChange(float progressValue)
    {
        _progressBar.Value = progressValue * 100f;
        _progressText.Text = $"{(int)(progressValue * 100f)}";
    }
}