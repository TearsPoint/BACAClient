namespace BACAClient.Template
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

    public partial class TreeView : UserControl
    {
        private string backgroud = string.Empty;
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisList/TreeView/";
        public event ChangedEventHandler BoxChanged;

        public TreeView()
        {
            this.InitializeComponent();
            this.PreviewMouseUp += Button_PreviewMouseUp;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            This_Click(sender, e);
        }

        private void BoxChange(int type)
        {
            string str = string.Format("bg{0}", this.TypeId);
            string str2 = str;
            string str3 = "bg_hover";
            string color = this.Color;
            bool? isChecked = this.TreeBox.IsChecked;
            bool flag2 = true;
            if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            {
                if (type == 0)
                {
                    this.TreeBox.IsChecked = false;
                    str2 = "bg";
                    color = "#716B6B";
                }
                else
                {
                    this.TreeBox.IsChecked = true;
                    str3 = str;
                }
            }
            else if (type == 0)
            {
                this.TreeBox.IsChecked = true;
                str3 = str;
            }
            else
            {
                this.TreeBox.IsChecked = false;
                str2 = "bg";
                color = "#716B6B";
            }
            this.BackGround = str2;
            if (!string.IsNullOrEmpty(color))
            {
                this.Title.Foreground = new BrushConverter().ConvertFromInvariantString(color) as Brush;
            }
            this.Hover = str3;
        }


        public void SetBackGroud()
        {
            if (string.IsNullOrEmpty(this.BackGround))
            {
                this.BackGround = "bg";
            }
            ImageBrush brush1 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(string.Format("{0}{1}.png", this.url, this.BackGround)))
            };
            base.Background = brush1;
        }

        private void This_Click(object sender, MouseButtonEventArgs e)
        {
            this.BoxChange(0);
            this.BoxChanged();
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            this.TreeBox.Tag = this.model.CategoryID;
            this.Title.Text = this.model.CategoryName;
            base.ToolTip = this.model.CategoryName;
            if (string.IsNullOrEmpty(this.Hover))
            {
                this.Hover = "bg_hover";
            }
            this.SetBackGroud();
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageBrush brush1 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(string.Format("{0}{1}.png", this.url, this.Hover)))
            };
            base.Background = brush1;
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            this.SetBackGroud();
        }

        private void TreeBox_Click(object sender, RoutedEventArgs e)
        {
            this.BoxChange(1);
        }

        public string BackGround
        {
            get
            {
                return this.backgroud;
            }
            set
            {
                this.backgroud = value;
                this.SetBackGroud();
            }
        }

        public string Color { get; set; }

        public string Hover { get; set; }

        public Category model { get; set; }

        public int TypeId { get; set; }

        public delegate void ChangedEventHandler();
    }
}

