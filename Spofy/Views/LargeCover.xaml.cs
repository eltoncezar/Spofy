using System.Windows;
using System.Windows.Controls;

namespace Spofy.Views
{
    /// <summary>
    /// Interaction logic for LargeCover.xaml
    /// </summary>
    public partial class LargeCover : UserControl
    {
        public LargeCover()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            buttonsBar.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            buttonsBar.Visibility = Visibility.Hidden;
        }
    }
}
