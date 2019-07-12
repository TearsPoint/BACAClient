namespace BACAClient.Template.HisDetail
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class PageLeft : UserControl
    { 
        public string url = "pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisDetail/Page/";
         
        public event ChangedEventHandler Click;

        public PageLeft()
        {
            this.InitializeComponent();
            this.PreviewMouseUp += Button_PreviewMouseUp;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            This_Click(sender, e);
        }

        private void This_Click(object sender, MouseButtonEventArgs e)
        {
            this.Click();
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Arrow.Source = new BitmapImage(new Uri(this.url + "lefthover.png"));
            this.grid.Background = new BrushConverter().ConvertFromInvariantString("#dddddd") as Brush;
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Arrow.Source = new BitmapImage(new Uri(this.url + "left.png"));
            this.grid.Background = Brushes.White;
        }

        public delegate void ChangedEventHandler();
    }
}

