using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        private SpofyManager()
        {
            model = new SpofyModel();
            _growl = new GrowlInterop();

            _spotify = new SpotifyLocalAPI();

            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                SpotifyLocalAPI.RunSpotify();
                Thread.Sleep(5000);
            }

            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                SpotifyLocalAPI.RunSpotifyWebHelper();
                Thread.Sleep(4000);
            }

            if (!_spotify.Connect())
            {
                Boolean retry = true;
                while (retry)
                {
                    MessageBoxResult result = MessageBox.Show("SpotifyLocalAPIClass could'nt load!\nDo you want to retry?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (_spotify.Connect())
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
        }

        #region Properties

        public static string appName = "Spofy";
        public SpofyModel model;

        private ImplicitGrantAuth _auth;
        private static GrowlInterop _growl;
        private static SpotifyLocalAPI _spotify;
        private static SpotifyWebAPI _spotifyWeb;
        private Track _currentTrack;
        private Bitmap _currentLargeCover;
        private string _currentSmallCoverUrl;

        #endregion

        public void Register(Dispatcher dispatcher)
        {
            UpdateTrack();

            _auth = new ImplicitGrantAuth
            {
                RedirectUri = "http://localhost:8888",
                ClientId = "12a2836e14274a93b95e12c1e4eabd29",
                Scope = Scope.UserLibrarayRead | Scope.UserLibraryModify,
                State = "XSS"
            };
            _auth.OnResponseReceivedEvent += _auth_OnResponseReceivedEvent;

            _auth.StartHttpServer(8888);
            _auth.DoAuth();

            _spotify.OnTrackChange += _spotify_OnTrackChange;
            _spotify.OnPlayStateChange += _spotify_OnPlayStateChange;
            _spotify.OnTrackTimeChange += _spotify_OnTrackTimeChange;

            _spotify.SynchronizingObject = new DispatcherWinFormsCompatAdapter(dispatcher);
            _spotify.ListenForEvents = true;
        }

        private String formatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }

        private void _auth_OnResponseReceivedEvent(Token token, string state)
        {
            _auth.StopHttpServer();

            if (state != "XSS")
            {
                MessageBox.Show(@"Wrong state received.", @"SpotifyWeb API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (token.Error != null)
            {
                MessageBox.Show(String.Format("Error: {0}", token.Error), @"SpotifyWeb API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _spotifyWeb = new SpotifyWebAPI
            {
                UseAuth = true,
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            //InitialSetup();
        }

        private void _spotify_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            if (model.IsAdPlaying)
                return;

            model.IsPlaying = e.Playing;

            if (_currentTrack != null)
            {
                if (model.IsPlaying)
                {
                    if (_currentTrack == model.LastTrack)
                    {
                        _growl.MusicResumed(_currentTrack.ArtistResource.Name, _currentTrack.TrackResource.Name, _currentSmallCoverUrl);
                    }
                    if (model.LastTrack == null)
                    {
                        UpdateTrack();
                    }
                }
                else
                {
                    _growl.MusicPaused(_currentTrack.ArtistResource.Name, _currentTrack.TrackResource.Name, _currentSmallCoverUrl);
                }
            }
        }

        private void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            if (model.IsAdPlaying)
                return;

            UpdateTrack();
        }

        private void _spotify_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            if (model.IsAdPlaying)
                return;

            UpdateTime(e.TrackTime);
        }

        private void UpdateTime(double time)
        {
            model.CurrentTimeSeconds = time;
            model.CurrentTime = formatTime(time);
        }

        private async void UpdateTrack()
        {
            StatusResponse status = _spotify.GetStatus();
            if (status == null || status.Track == null)
            {
                model.IsAdPlaying = true;
                model.ArtistName = "";
                model.Cover = null;
                model.Image = null;
                model.TrackName = null;
                return;
            }
            else
            {
                model.IsAdPlaying = false;
            }

            if (
                //Is Ad Running
                (!status.NextEnabled && !status.PrevEnabled) ||
                (status.Track.AlbumResource == null || status.Track.ArtistResource == null)
                )
                return; //TODO: better ad treatment

            _currentTrack = status.Track;
            _currentLargeCover = await _currentTrack.GetAlbumArtAsync(AlbumArtSize.Size640);
            _currentSmallCoverUrl = _currentTrack.GetAlbumArtUrl(AlbumArtSize.Size160);

            if (_currentTrack != null)
            {
                string artistName = _currentTrack.ArtistResource.Name;
                string trackName = _currentTrack.TrackResource.Name;
                double totalTimeSeconds = _currentTrack.Length;
                string totalTime = formatTime(totalTimeSeconds);

                App.Current.MainWindow.Title = String.Format("{0} - {1} - {2}", appName, artistName, trackName);
                model.TrackName = trackName;
                model.ArtistName = artistName;
                model.TotalTimeSeconds = totalTimeSeconds;
                model.TotalTime = totalTime;
                model.Image = _currentLargeCover;

                _growl.TrackChanged(artistName, trackName, _currentSmallCoverUrl);

                model.LastTrack = _currentTrack;
            }
        }

        #region Media Commands

        public void Next()
        {
            model.IsPlaying = true;
            _spotify.Skip();
        }

        public void PlayPause()
        {
            if (model.IsPlaying)
            {
                model.IsPlaying = false;
                _spotify.Pause();
            }
            else
            {
                model.IsPlaying = true;
                _spotify.Play();
            }
        }

        public void Previous()
        {
            model.IsPlaying = true;
            _spotify.Previous();
        }

        public bool SaveCurrentTrack()
        {
            if (_spotifyWeb != null)
            {
                var id = _currentTrack.TrackResource.Uri.Substring(14);
                ErrorResponse response = _spotifyWeb.SaveTrack(id);
                if (!response.HasError())
                    return true;
                else
                    return false;
            }
            else
            {
                MessageBox.Show(@"Couldn't connect to Spotify Web.", @"SpotifyWeb API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool RemoveCurrentTrack()
        {
            if (_spotifyWeb != null)
            {
                var id = _currentTrack.TrackResource.Uri.Substring(14);
                ErrorResponse response = _spotifyWeb.RemoveSavedTracks(new List<string>() { id });
                if (!response.HasError())
                    return true;
                else
                    return false;
            }
            else
            {
                MessageBox.Show(@"Couldn't connect to Spotify Web.", @"SpotifyWeb API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion
    }
}
