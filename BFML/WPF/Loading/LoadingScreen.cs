using System.Windows;
using System.Windows.Controls;
using XamlRadialProgressBar;

namespace BFML.WPF.Loading;

internal sealed class LoadingScreen
{
    private readonly Border _loading;
    private readonly RadialProgressBar _progressBar;
    private readonly TextBlock _progressText;
    private CompositeProgressTracker _progressTracker;

    public LoadingScreen(Border loading, RadialProgressBar progressBar, TextBlock progressText)
    {
        _loading = loading;
        _progressBar = progressBar;
        _progressText = progressText;
    }

    public CompositeProgressTracker Show()
    {
        _loading.Visibility = Visibility.Visible;
        _progressBar.Value = 0f;
        _progressText.Text = "0";
        _progressTracker = new CompositeProgressTracker();
        _progressTracker.Changed += OnProgressTrackerChange;
        return _progressTracker;
    }

    public void Hide()
    {
        _loading.Visibility = Visibility.Collapsed;
        _progressTracker.Changed -= OnProgressTrackerChange;
    }

    private void OnProgressTrackerChange(float progressValue)
    {
        _progressBar.Value = progressValue * 100f;
        _progressText.Text = $"{(int)(progressValue * 100f)}";
    }
}