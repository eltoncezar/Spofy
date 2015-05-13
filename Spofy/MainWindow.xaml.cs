///file:///C:/Windows7TrainingKit/Labs/Taskbar%20-%20Managed/Lab.html/html/docSet_d2c46e57-0c16-4bf8-bcee-8d2d1ba49ffa.html#_Toc236828999
///
///https://github.com/JohnnyCrazy/SpotifyAPI-NET/wiki/SpotifyLocalAPI-Usage
///! http://www.codeproject.com/KB/shell/dotnetbandobjects.aspx?msg=3230393#xx3230393xx
///https://msdn.microsoft.com/en-us/library/windows/desktop/cc144099(v=vs.85).aspx
///http://www.codeproject.com/Articles/1323/Internet-Explorer-Toolbar-Deskband-Tutorial
///https://msdn.microsoft.com/en-us/library/dd378460(v=vs.85).aspx
///
///http://www.codeproject.com/Articles/65185/Windows-Taskbar-C-Quick-Reference
///https://msdn.microsoft.com/en-us/library/ff699128.aspx
///
///http://nikosbaxevanis.com/blog/2010/12/01/building-a-metro-ui-with-wpf/
///
/// GROWL ALIKE NOTIFICATIONS
/// http://www.codeproject.com/Articles/499241/Growl-Alike-WPF-Notifications
/// http://stackoverflow.com/questions/3034741/create-popup-toaster-notifications-in-windows-with-net
/// https://www.simple-talk.com/dotnet/.net-framework/creating-tray-applications-in-.net-a-practical-guide/
/// http://labs.bjfocus.co.uk/2013/06/add-a-notification-icon-to-your-wpf-program/
/// http://www.hardcodet.net/wpf-notifyicon // https://visualstudiogallery.msdn.microsoft.com/aacbc77c-4ef6-456f-80b7-1f157c2909f7/

using Spofy.Classes;
using Spofy.Views;
using System;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace Spofy
{
    public partial class MainWindow : Window
    {
        #region Init

        protected override void OnInitialized(EventArgs e)
        {
            Title = SpofyManager.appName;
            AllowsTransparency = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.Manual;
            WindowStyle = WindowStyle.None;

            CheckSettings();

            //snap to edges
            MouseDown += Window_MouseDown;
            MouseUp += Window_MouseUp;
            Loaded += Window_Loaded;

            SourceInitialized += HandleSourceInitialized;
            base.OnInitialized(e);
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = SpofyManager.Instance.model;
            SpofyManager.Instance.Register(this.Dispatcher);
        }

        #endregion

        private void CheckSettings()
        {
            this.Topmost = Properties.Settings.Default.AlwaysOnTop;

            UserControl view;

            switch (Properties.Settings.Default.View)
            {
                case 0:
                    view = new LargeCover();
                    break;
                case 1:
                    view = new SmallCover();
                    break;
                case 2:
                    view = new MinimalCover();
                    break;
                default:
                    view = new LargeCover();
                    break;
            }

            this.Height = view.Height;
            this.Width = view.Width;
            mainContent.Children.Add(view);
        }

        //private void HandlePreviewMouseMove(Object sender, MouseEventArgs e)
        //{
        //    if (Mouse.LeftButton == MouseButtonState.Pressed)
        //    {
        //        DragMove();
        //        //Cursor = Cursors.SizeAll;
        //    }
        //}

        #region Buttons

        private void ThumbPlayButton_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Shadows

        private HwndSource m_hwndSource;

        /// <summary>
        /// Handles the source initialized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleSourceInitialized(Object sender, EventArgs e)
        {
            m_hwndSource = (HwndSource)PresentationSource.FromVisual(this);

            // Returns the HwndSource object for the window
            // which presents WPF content in a Win32 window.
            HwndSource.FromHwnd(m_hwndSource.Handle).AddHook(
                new HwndSourceHook(NativeMethods.WindowProc));

            // http://msdn.microsoft.com/en-us/library/aa969524(VS.85).aspx
            Int32 DWMWA_NCRENDERING_POLICY = 2;
            NativeMethods.DwmSetWindowAttribute(
                m_hwndSource.Handle,
                DWMWA_NCRENDERING_POLICY,
                ref DWMWA_NCRENDERING_POLICY,
                4);

            // http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx
            NativeMethods.ShowShadowUnderWindow(m_hwndSource.Handle);
        }

        #endregion

        #region ContextMenu

        private void cmiSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
            settingsWindow.Closed += settingsWindow_Closed;
        }

        void settingsWindow_Closed(object sender, EventArgs e)
        {
            CheckSettings();
        }

        private void cmiMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void cmiExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Snap to Edges

        // Get the working area of the screen.  It excludes any dockings or toolbars, which
        // is exactly what we want.
        private Rect screen = new Rect(
            System.Windows.SystemParameters.VirtualScreenLeft,
            System.Windows.SystemParameters.VirtualScreenTop,
            System.Windows.SystemParameters.VirtualScreenWidth,
            System.Windows.SystemParameters.VirtualScreenHeight);

        // This will be the flag for the automatic positioning.
        private bool dragging = false;

        // Wait until window is lodaded, but prior to being rendered to set position.  This 
        // is done because prior to being loaded you'll get NaN for this.Height and 0 for
        // this.ActualHeight.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Sets the initial position.
            SetInitialWindowPosition();
            // Sets the monitoring timer loop.
            InitializeWindowPositionMonitoring();
        }

        // Allows the window to be dragged where the are no other controls present.
        // PreviewMouseButton could be used, but then you have to do more work to ensure that if
        // you're pressing down on a button, it executes its routine if dragging was not performed.
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Set the dragging flag to true, so that position would not be reset automatically.
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                dragging = true;
                this.DragMove();
                Cursor = Cursors.SizeAll;
            }
        }

        // Similar to MouseDown.  We're setting dragging flag to false to allow automatic 
        // positioning.
        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                dragging = false;
                Cursor = Cursors.Arrow;
            }
        }

        // Sets the initial position of the window.  I made mine static for now, but later it can
        // be modified to be whatever the user chooses in the settings.
        private void SetInitialWindowPosition()
        {
            this.Left = screen.Width - this.Width;
            this.Top = screen.Height / 2 - this.Height / 2;
        }

        // Setup the monitoring routine that automatically positions the window based on its location
        // relative to the working area.
        private void InitializeWindowPositionMonitoring()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += delegate
            {
                // Check if window is being dragged (assuming that mouse down on any portion of the
                // window is connected to dragging).  This is a fairly safe assumption and held
                // true thus far.  Even if you're not performing any dragging, then the position
                // doesn't change and nothing gets reset.  You can add an extra check to see if
                // position has changed, but there is no significant performance gain.
                // Correct me if I'm wrong, but this is just O(n) execution, where n is the number of
                // ticks the mouse has been held down on that window.
                if (!dragging)
                {
                    // Checking the left side of the window.
                    if (this.Left > screen.Width - this.Width)
                    {
                        this.Left = screen.Width - this.Width;
                    }
                    else if (this.Left < 0)
                    {
                        this.Left = 0;
                    }

                    // Checking the top of the window.
                    if (this.Top > screen.Height - this.Height)
                    {
                        this.Top = screen.Height - this.Height;
                    }
                    else if (this.Top < 0)
                    {
                        this.Top = 0;
                    }
                }
            };

            // Adjust this based on performance and preference.  I set mine to 10 milliseconds.
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();
        }

        #endregion
    }
}
