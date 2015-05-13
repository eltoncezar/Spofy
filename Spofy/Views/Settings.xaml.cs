using Spofy.Classes;
using System.Windows;

namespace Spofy
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            this.DataContext = new SettingsModel();
            ((SettingsModel)this.DataContext).AlwaysOnTop = Properties.Settings.Default.AlwaysOnTop;
        }

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((SettingsModel)this.DataContext).AlwaysOnTop = !((SettingsModel)this.DataContext).AlwaysOnTop;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AlwaysOnTop = ((SettingsModel)this.DataContext).AlwaysOnTop;

            if (rdb0.IsChecked.Value)
            {
                Properties.Settings.Default.View = 0;
            }
            else if (rdb1.IsChecked.Value)
            {
                Properties.Settings.Default.View = 1;
            }
            else if (rdb2.IsChecked.Value)
            {
                Properties.Settings.Default.View = 2;
            }

            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
