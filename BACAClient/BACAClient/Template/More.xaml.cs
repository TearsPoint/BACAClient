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
    using System.Windows.Media.Imaging;

    public partial class More : UserControl
    {
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Index/Left";
        public event ChangedEventHandler gotokdp;

        public More()
        {
            this.InitializeComponent();
        }

        private void _Click(string Name)
        {
            this.gotokdp(this.model);
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            this.gotokdp(this.model);
        }
         
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.index.Text = this.Index.ToString();
                base.Width = this.width;
                base.ToolTip = this.model.Name;
                this.Name.Text = this.model.Name;
                this.CategoryName.Text = this.model.CategoryName;
                this.Name_E.Text = this.model.Name_E;
                if (this.model.SourceId == 9)
                {
                    this.Source.Source = new BitmapImage(new Uri(string.Format("{0}/icon23.png", this.url)));
                }
                this.Type.Source = new BitmapImage(new Uri(string.Format("{0}/icon{1}.png", this.url, this.model.TypeID)));
            }
            catch
            {
            }
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            this.Detail.Visibility = Visibility.Visible;
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            this.Detail.Visibility = Visibility.Hidden;
        }
         
        public int Index { get; set; }

        public GoodAndHot model { get; set; }

        public double width { get; set; }

        public delegate void ChangedEventHandler(GoodAndHot model);
    }
}

