namespace BACAClient.Template.HisDetail
{
    using BACAClient.Model;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
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

    public partial class Relational : UserControl
    {
        public int enter = 0;
        public string icon = "pack://application:,,,/BACAClient;component/Images/Pages/Index/";
        public string url = "pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisDetail/Related/";
        
        public event ChangedEventHandler RelateInfo;

        public Relational()
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
            this.RelateInfo(this.model.ReTypeId);
            if ((this.model.ReTypeId != 7) && (this.TypeId != 4))
            {
                object[] objArray1 = new object[] { this.icon, "Right/icon", this.model.ReTypeId, "1.png" };
                this.Icon.Source = new BitmapImage(new Uri(string.Concat(objArray1)));
            }
            this.Bg.BorderThickness = new Thickness(0.0);
            ImageBrush brush = new ImageBrush();
            object[] objArray2 = new object[] { this.url, "relate", this.TypeId, ".png" };
            brush.ImageSource = new BitmapImage(new Uri(string.Concat(objArray2)));
            this.grid.Background = brush;
            this.Name.Foreground = Brushes.White;
            this.enter = 1;
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string reName = string.Empty;
                if (this.model == null)
                {
                    reName = "表单";
                }
                else
                {
                    this.model.ReTypeId = 0;     //todo
                    reName = this.model.ReName;
                }
                base.ToolTip = reName;
                this.Name.Text = reName;
                if (this.TypeId == 4)
                {
                    this.Icon.Visibility = Visibility.Hidden;
                }
                else if (this.model.ReTypeId != 7)
                {
                    this.Icon.Source = new BitmapImage(new Uri(string.Format("{0}Left/icon{1}.png", this.icon, this.model.ReTypeId)));
                }
            }
            catch
            {
            }
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.enter == 0)
            {
                ImageBrush brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(this.url + "relate.png"))
                };
                this.grid.Background = brush1;
            }
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.enter == 0)
            {
                this.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
                if ((this.model.ReTypeId != 7) && (this.TypeId != 4))
                {
                    this.Icon.Source = new BitmapImage(new Uri(string.Format("{0}Left/icon{1}.png", this.icon, this.model.ReTypeId)));
                }
                this.grid.Background = Brushes.White;
            }
        }

        public List<BACAClient.Model.Type> data { get; set; }

        public BACAClient.Model.Relational model { get; set; }

        public int TypeId { get; set; }

        public delegate void ChangedEventHandler(int ReTypeId);
    }
}

