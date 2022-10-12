using System.Windows;

namespace BFML.WPF
{
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }

        public void SetProgress(float value)
        {
            ProgressBar.Value = value * 100;
        }
    }
}
