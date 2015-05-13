
using Growl.Connector;
using System;
using System.Drawing;
namespace Spofy.Classes
{
    public class GrowlInterop
    {
        private GrowlConnector growl;
        private const string applicationName = "Spofy";

        public GrowlInterop()
        {
            growl = new GrowlConnector();
            //growl.EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText;

            Register();
        }

        private void Register()
        {
            Growl.Connector.Application application = new Growl.Connector.Application(applicationName);
            application.Icon = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("Spofy.Images.logo_small.png"));

            growl.Register(application, new NotificationType[] {
                new NotificationType(NotificationTypes.TrackChanged.Type, NotificationTypes.TrackChanged.Name),
                new NotificationType(NotificationTypes.MusicPaused.Type, NotificationTypes.MusicPaused.Name),
                new NotificationType(NotificationTypes.MusicResumed.Type, NotificationTypes.MusicResumed.Name),
            });
        }

        private void Notify(string title, string message, string cover, NotifyType notificationType)
        {
            Growl.CoreLibrary.Resource Icon = cover;
            growl.Notify(new Notification(applicationName, notificationType.Type, DateTime.Now.Ticks.ToString(), message, title, Icon, false, Priority.VeryLow, null));
        }

        public void TrackChanged(string artist, string music, string cover)
        {
            Notify(artist, music, cover, NotificationTypes.TrackChanged);
        }

        public void MusicPaused(string artist, string music, string cover)
        {
            Notify(artist, "[PAUSED] " + music, cover, NotificationTypes.MusicPaused);
        }

        public void MusicResumed(string artist, string music, string cover)
        {
            Notify(artist, "[RESUMED] " + music, cover, NotificationTypes.MusicResumed);
        }
    }

    internal static class NotificationTypes
    {
        public static NotifyType TrackChanged = new NotifyType("TRACK_CHANGED", "Track Changed");
        public static NotifyType MusicPaused = new NotifyType("MUSIC_PAUSED", "Music Paused");
        public static NotifyType MusicResumed = new NotifyType("MUSIC_RESUMED", "Music Resumed");
    }

    internal class NotifyType
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public NotifyType(string type, string name)
        {
            this.Type = type;
            this.Name = name;
        }
    }
}
