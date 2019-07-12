namespace BACAClient.Template.HisDetail
{
    using BACAClient.Model;
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

    public partial class Forward : UserControl
    { 
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisDetail/Icon/";
        public event ChangedEventHandler Click;
        public Forward()
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

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.typeretrieve != null)
                {
                    this.Icon.Source = new BitmapImage(new Uri(string.Format("{0}x_{1}.png", this.url, this.Type)));
                    this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgColor) as Brush;
                }
            }
            catch
            {
            }
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.typeretrieve.BgHover))
            {
                this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgHover) as Brush;
            }
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.typeretrieve.BgColor))
            {
                this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgColor) as Brush;
            }
        }

        public string Type { get; set; }

        public TypeRetrieve typeretrieve { get; set; }

        public delegate void ChangedEventHandler();
    }
}

