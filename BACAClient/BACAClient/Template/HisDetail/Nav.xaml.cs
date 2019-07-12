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

    public partial class Nav : UserControl
    {
        public int enter = 0; 
        public string url = "pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisDetail/Nav/";
        
        public event ChangedEventHandler Click;

        public Nav()
        {
            this.InitializeComponent();
            this.PreviewMouseUp += Button_PreviewMouseUp;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            This_Click(sender, e);
        }

        public void ChangeStyle()
        {
            this.Arrow.Source = new BitmapImage(new Uri(string.Format("{0}arrow{1}.png", this.url, this.TypeId)));
            this.Bg.Background = new BrushConverter().ConvertFromInvariantString(this.BgColor) as Brush;
            this.Bg.BorderThickness = new Thickness(0.0);
            this.Name.Foreground = Brushes.White;
            this.Arrow.Visibility = Visibility.Visible;
            this.enter = 1;
        }
        
        private void This_Click(object sender, MouseButtonEventArgs e)
        {
            this.Click(this.Index);
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            this.Name.Text = this.Title;
            base.ToolTip = this.Title;
            this.Arrow.Visibility = Visibility.Hidden;
            this.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
            this.Bg.Background = Brushes.White;
            this.Bg.BorderThickness = new Thickness(0.0, 0.0, 0.0, 2.0);
            if (this.First)
            {
                this.ChangeStyle();
            }
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.enter == 0)
            {
                this.Arrow.Source = new BitmapImage(new Uri(string.Format("{0}arrow.png", this.url)));
                this.Bg.Background = new BrushConverter().ConvertFromInvariantString("#dddddd") as Brush;
                this.Arrow.Visibility = Visibility.Visible;
            }
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.enter == 0)
            {
                this.Arrow.Visibility = Visibility.Hidden;
                this.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
                this.Bg.Background = Brushes.White;
                this.Bg.BorderThickness = new Thickness(0.0, 0.0, 0.0, 2.0);
            }
        }

        public string BgColor { get; set; }

        public bool First { get; set; }

        public int Index { get; set; }

        public string Title { get; set; }

        public int TypeId { get; set; }

        public delegate void ChangedEventHandler(int N);
    }
}

