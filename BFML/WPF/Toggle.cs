using System.Windows;
using System.Windows.Controls.Primitives;

namespace BFML.WPF
{
    public class Toggle : ToggleButton
    {
        static Toggle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toggle), new FrameworkPropertyMetadata(typeof(Toggle)));
        }
    }
}
