using Spofy.Classes;
using System.Windows.Controls;
using System.Windows.Input;

namespace Spofy.Views
{
    /// <summary>
    /// Interaction logic for ButtonsBar.xaml
    /// </summary>
    public partial class ButtonsBar : UserControl
    {
        public ButtonsBar()
        {
            InitializeComponent();

            btnPrevious.MouseLeftButtonDown += btnPrevious_MouseLeftButtonUp;
            btnPause.MouseLeftButtonDown += btnPause_MouseLeftButtonUp;
            btnPlay.MouseLeftButtonDown += btnPlay_MouseLeftButtonUp;
            btnNext.MouseLeftButtonDown += btnNext_MouseLeftButtonUp;
        }

        private void btnPrevious_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpofyManager.Instance.Previous();
        }

        private void btnPlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpofyManager.Instance.PlayPause();
        }

        private void btnPause_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpofyManager.Instance.PlayPause();
        }

        private void btnNext_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpofyManager.Instance.Next();
        }
    }
}
