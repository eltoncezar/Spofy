using System.Windows;
using System.Windows.Controls;

namespace Spofy.Views
{
    /// <summary>
    /// Interaction logic for SmallCover.xaml
    /// </summary>
    public partial class SmallCover : UserControl
    {
        public SmallCover()
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
