namespace BACAClient.UserControls.Controls.FrontDesk
{
    using BACAClient.Common;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    public partial class GoodSuccess : Window
    {
        private DateTime time;
        private DispatcherTimer timer = new DispatcherTimer();

        public GoodSuccess()
        {
            this.InitializeComponent();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.icon.Source = new BitmapImage(new Uri("pack://application:,,,/BACAClient;component/Images/OtherWindow/Good/ss_good.png"));
                this.time = DateTime.Now;
                this.timer.Tick += new EventHandler(this.TimerTick);
                this.timer.Start();
            }
            catch
            {
            }
        }
        
        private void TimerTick(object sender, EventArgs e)
        {
            try
            {
                double num = double.Parse(ConfigerHelper.GetAutoCloseTimer());
                TimeSpan span = (TimeSpan) (DateTime.Now - this.time);
                if (span.TotalSeconds >= num)
                {
                    this.timer.Stop();
                    this.timer.Tick -= new EventHandler(this.TimerTick);
                    base.Close();
                }
            }
            catch
            {
            }
        }
    }
}

