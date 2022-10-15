using System.Windows;
using Common.Misc;

namespace BFML.WPF
{
    public partial class LoadingWindow : Window
    {
        public CompositeProgress Progress;

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
