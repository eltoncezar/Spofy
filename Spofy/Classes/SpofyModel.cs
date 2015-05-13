using SpotifyAPI.SpotifyLocalAPI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Spofy.Classes
{
    public class SpofyModel : INotifyPropertyChanged
    {
        private string trackName;
        public string TrackName
        {
            get
            {
                return this.trackName;
            }
            set
            {
                this.trackName = value;
                this.NotifyPropertyChanged("TrackName");
            }
        }

        private string artistName;
        public string ArtistName
        {
            get
            {
                return this.artistName;
            }
            set
            {
                this.artistName = value;
                this.NotifyPropertyChanged("ArtistName");
            }
        }

        //public Bitmap image;
        public Bitmap Image
        {
            //get
            //{
            //    return this.image;
            //}
            set
            {
                //this.image = value;
                this.Cover = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(value.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(200, 200));
                //this.NotifyPropertyChanged("Image");
            }
        }

        public BitmapSource cover;
        public BitmapSource Cover
        {
            get
            {
                return this.cover;
            }
            set
            {
                this.cover = value;
                this.NotifyPropertyChanged("Cover");
            }
        }

        private string totalTime;
        public string TotalTime
        {
            get
            {
                return this.totalTime;
            }
            set
            {
                this.totalTime = value;
                this.NotifyPropertyChanged("TotalTime");
            }
        }

        private double totalTimeSeconds;
        public double TotalTimeSeconds
        {
            get
            {
                return this.totalTimeSeconds;
            }
            set
            {
                this.totalTimeSeconds = value;
                this.NotifyPropertyChanged("TotalTimeSeconds");
            }
        }

        private string currentTime;
        public string CurrentTime
        {
            get
            {
                return this.currentTime;
            }
            set
            {
                this.currentTime = value;
                this.NotifyPropertyChanged("CurrentTime");
            }
        }

        private double currentTimeSeconds;
        public double CurrentTimeSeconds
        {
            get
            {
                return this.currentTimeSeconds;
            }
            set
            {
                this.currentTimeSeconds = value;
                this.NotifyPropertyChanged("CurrentTimeSeconds");
            }
        }

        private string remainingTime;
        public string RemainingTime
        {
            get
            {
                return this.remainingTime;
            }
            set
            {
                this.remainingTime = value;
                this.NotifyPropertyChanged("RemainingTime");
            }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get
            {
                return this.isPlaying;
            }
            set
            {
                this.isPlaying = value;
                this.NotifyPropertyChanged("IsPlaying");
            }
        }

        private Track lastTrack;
        public Track LastTrack
        {
            get
            {
                return this.lastTrack;
            }
            set
            {
                this.lastTrack = value;
                this.NotifyPropertyChanged("LastTrack");
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
