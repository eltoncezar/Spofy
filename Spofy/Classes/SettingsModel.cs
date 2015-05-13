using System.ComponentModel;

namespace Spofy.Classes
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private bool alwaysOnTop;
        public bool AlwaysOnTop
        {
            get
            {
                return this.alwaysOnTop;
            }
            set
            {
                this.alwaysOnTop = value;
                this.NotifyPropertyChanged("AlwaysOnTop");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
