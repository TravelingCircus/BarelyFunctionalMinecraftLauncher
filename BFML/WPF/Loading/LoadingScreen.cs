using System.Windows;
using System.Windows.Controls;
using XamlRadialProgressBar;

namespace BFML.WPF.Loading;

internal sealed class LoadingScreen
{
    private ProgressTracker[] _trackers;
    private readonly Border _loading;
    private readonly TextBlock _progressText;
    private readonly RadialProgressBar _progressBar;
    private readonly ItemsControl _progressTrackersList;
    private readonly TextBlock _progressTrackerProgressText;

    public LoadingScreen(
        Border loading, RadialProgressBar progressBar, TextBlock progressText, 
        TextBlock progressTrackerProgressText, ItemsControl progressTrackerList)
    {
        _loading = loading;
        _progressBar = progressBar;
        _progressText = progressText;
        _progressTrackersList = progressTrackerList;
        _progressTrackerProgressText = progressTrackerProgressText;
    }

    public void Show(ProgressTracker[] trackers)
    {
        _loading.Visibility = Visibility.Visible;
        _progressBar.Value = 0f;
        _progressText.Text = "0";
        _trackers = trackers;
        _progressTrackersList.ItemsSource = trackers;
        foreach (ProgressTracker tracker in trackers)
        {
            tracker.Changed += OnProgressTrackerChange;
        }
    }

    public void Hide()
    {
        _loading.Visibility = Visibility.Collapsed;
    }

    private void OnProgressTrackerChange(float progressValue)
    {
        _progressBar.Value = progressValue * 100f;
        _progressText.Text = $"{(int)(progressValue * 100f)}";
    }
}