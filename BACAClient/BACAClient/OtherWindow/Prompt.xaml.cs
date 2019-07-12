namespace BACAClient.OtherWindow
{
    using BACAClient.Common;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Threading;

    public partial class Prompt : Window
    {
        private DateTime time;
        private DispatcherTimer timer = new DispatcherTimer();

        public Prompt()
        {
            this.InitializeComponent();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.data.Text = this.Message;
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
                TimeSpan span = (TimeSpan)(DateTime.Now - this.time);
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

        public string Message { get; set; }

        private void Close_Click(string Name)
        {
            this.timer.Stop();
            this.timer.Tick -= new EventHandler(this.TimerTick);
            base.Close(); 
        }
    }
}

