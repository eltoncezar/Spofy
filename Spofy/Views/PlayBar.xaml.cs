using System;
using System.Windows;
using System.Windows.Controls;

namespace Spofy.Views
{
    /// <summary>
    /// Interaction logic for PlayBar.xaml
    /// </summary>
    public partial class PlayBar : UserControl
    {
        #region Properties

        #region Time
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(PlayBar), new PropertyMetadata(ValueChangedStatic));
        public double Time
        {
            get { return (double)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        private static void ValueChangedStatic(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((PlayBar)sender).ValueChanged(e.NewValue);
        }

        private void ValueChanged(object parameter)
        {
            double totalTime = (double)GetValue(TotalTimeProperty);
            double currentTime = (double)parameter;

            bar.Width = totalTime != 0 ? (currentTime * this.ActualWidth) / totalTime : 0;
        }
        #endregion

        #region TotalTime
        public static readonly DependencyProperty TotalTimeProperty = DependencyProperty.Register("TotalTime", typeof(double), typeof(PlayBar), new PropertyMetadata(TotalValueChangedStatic));
        public double TotalTime
        {
            get { return (double)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        private static void TotalValueChangedStatic(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((PlayBar)sender).TotalValueChanged(e.NewValue);
        }

        private void TotalValueChanged(object parameter)
        {
            bar.Width = 0;
        }
        #endregion

        #endregion

        public PlayBar()
        {
            InitializeComponent();
        }
    }
}
