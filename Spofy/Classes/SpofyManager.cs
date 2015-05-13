using SpotifyAPI.SpotifyLocalAPI;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Spofy.Classes
{
    public class SpofyManager
    {
        private static SpofyManager instance;

        public static SpofyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpofyManager();
                }
                return instance;
            }
        }
        #region Properties

        public static string appName = "Spofy";
        static GrowlInterop growl;
        static SpotifyLocalAPIClass spotify;
        static SpotifyMusicHandler mh;
        static SpotifyEventHandler eh;
        public SpofyModel model;

        #endregion

        private SpofyManager()
        {
            model = new SpofyModel();
            growl = new GrowlInterop();

            spotify = new SpotifyLocalAPIClass();
            if (!SpotifyLocalAPIClass.IsSpotifyRunning())
            {
                spotify.RunSpotify();
                Thread.Sleep(5000);
            }

            if (!SpotifyLocalAPIClass.IsSpotifyWebHelperRunning())
            {
                spotify.RunSpotifyWebHelper();
                Thread.Sleep(4000);
            }

            if (!spotify.Connect())
            {
                Boolean retry = true;
                while (retry)
                {
                    MessageBoxResult result = MessageBox.Show("SpotifyLocalAPIClass could'nt load!\nDo you want to retry?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (spotify.Connect())
                            retry = false;
                        else
                            retry = true;
                    }
                    else
                    {
                        App.Current.Shutdown();
                        //this.Close();
                        return;
                    }
                }
            }
            mh = spotify.GetMusicHandler();
            eh = spotify.GetEventHandler();
        }

        public void Register(Dispatcher dispatcher)
        {
            spotify.Update();
            UpdateTrack(mh.GetCurrentTrack());

            eh.OnTrackChange += OnTrackChange;
            eh.OnPlayStateChange += OnPlayStateChange;
            eh.OnTrackTimeChange += OnTrackTimeChange;

            eh.SetSynchronizingObject(new DispatcherWinFormsCompatAdapter(dispatcher));
            eh.ListenForEvents(true);
        }

        private void OnPlayStateChange(PlayStateEventArgs e)
        {
            model.IsPlaying = e.playing;

            Track track = mh.GetCurrentTrack();
            if (track != null)
            {
                if (model.IsPlaying)
                {
                    if (track == model.LastTrack)
                    {
                        growl.MusicResumed(track.GetArtistName(), track.GetTrackName(), track.GetAlbumArtURL(AlbumArtSize.SIZE_160));
                    }
                    if (model.LastTrack == null)
                    {
                        UpdateTrack(track);
                    }
                }
                else
                {
                    growl.MusicPaused(track.GetArtistName(), track.GetTrackName(), track.GetAlbumArtURL(AlbumArtSize.SIZE_160));
                }
            }
        }

        private void OnTrackChange(TrackChangeEventArgs e)
        {
            UpdateTrack(e.new_track);
        }

        private void OnTrackTimeChange(TrackTimeChangeEventArgs e)
        {
            UpdateTime(e.track_time);
        }

        private async void UpdateTrack(Track track)
        {
            if (track != null && !mh.IsAdRunning())
            {
                string artistName = track.GetArtistName();
                string trackName = track.GetTrackName();
                double totalTimeSeconds = track.GetLength();
                string totalTime = formatTime(totalTimeSeconds);

                App.Current.MainWindow.Title = String.Format("{0} - {1} - {2}", appName, artistName, trackName);
                model.TrackName = trackName;
                model.ArtistName = artistName;
                model.TotalTimeSeconds = totalTimeSeconds;
                model.TotalTime = totalTime;
                model.Image = await track.GetAlbumArtAsync(AlbumArtSize.SIZE_640);

                growl.TrackChanged(artistName, trackName, track.GetAlbumArtURL(AlbumArtSize.SIZE_160));

                model.LastTrack = track;
            }
        }

        private void UpdateTime(double time)
        {
            model.CurrentTimeSeconds = time;
            model.CurrentTime = formatTime(time);
        }

        private String formatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }

        #region Media Commands

        public void PlayPause()
        {
            if (model.IsPlaying)
            {
                model.IsPlaying = false;
                mh.Pause();
            }
            else
            {
                model.IsPlaying = true;
                mh.Play();
            }
        }

        public void Previous()
        {
            model.IsPlaying = true;
            mh.Previous();
        }

        public void Next()
        {
            model.IsPlaying = true;
            mh.Skip();
        }

        #endregion
    }
}
