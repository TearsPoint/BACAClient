namespace BACAClient.Template
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

    public partial class TwoNav : UserControl
    {
        public bool isClick = false; 
        private string url = "pack://application:,,,/BACAClient;component/Images/MainWindow/twonav/";

        public event ChangedEventHandler Click;

        public TwoNav()
        {
            this.InitializeComponent(); 
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            this.Name.Foreground = Brushes.White;
            ImageBrush brush1 = new ImageBrush {
                ImageSource = new BitmapImage(new Uri(this.url + "in_two_nav.png"))
            };
            this.grid.Background = brush1;
            this.Click(this.Title);
            this.isClick = true;
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Title == "首页推荐")
            {
                this.Name.Foreground = Brushes.White;
                ImageBrush brush1 = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri(this.url + "in_two_nav.png"))
                };
                this.grid.Background = brush1;
                this.isClick = true;
            }
            else
            {
                ImageBrush brush2 = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri(this.url + "two_nav.png"))
                };
                this.grid.Background = brush2;
                this.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
            }
            this.Name.Text = this.Title;
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            this.Name.Foreground = Brushes.White;
            ImageBrush brush1 = new ImageBrush {
                ImageSource = new BitmapImage(new Uri(this.url + "in_two_nav.png"))
            };
            this.grid.Background = brush1;
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.isClick)
            {
                this.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
                ImageBrush brush1 = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri(this.url + "two_nav.png"))
                };
                this.grid.Background = brush1;
            }
        }
         
        public string Title { get; set; }

        public delegate void ChangedEventHandler(string type);
    }
}

