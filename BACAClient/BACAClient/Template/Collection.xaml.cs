namespace BACAClient.Template
{
    using BACAClient.Model;
    using BACAClient.UserControls.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class Collection : UserControl
    {
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Index/Right";
        public event ChangedEvent gotokdp;
        public event Event Refresh;

        public Collection()
        {
            this.InitializeComponent();
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            this.Type.Source = new BitmapImage(new Uri(string.Format("{0}/icon{1}1.png", this.url, this.model.TypeId)));
            base.Background = new BrushConverter().ConvertFromInvariantString("#3399cc") as Brush;
            this.KnowledgeName.Foreground = Brushes.White;
            this.CreateTime.Foreground = Brushes.White;
            this.gotokdp(this.model);
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            base.ToolTip = this.model.Key1;
            this.KnowledgeName.Text = this.model.Key1;
            this.CreateTime.Text = this.model.CreateTime.ToString("yyyy-MM-dd");
            base.Background = new BrushConverter().ConvertFromInvariantString("#c7ddbf") as Brush;
            this.Type.Source = new BitmapImage(new Uri(string.Format("{0}/icon{1}.png", this.url, this.model.TypeId)));
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            this.Type.Source = new BitmapImage(new Uri(string.Format("{0}/icon{1}1.png", this.url, this.model.TypeId)));
            base.Background = new BrushConverter().ConvertFromInvariantString("#3399cc") as Brush;
            this.KnowledgeName.Foreground = Brushes.White;
            this.CreateTime.Foreground = Brushes.White;
            this.DeleteOne.Visibility = Visibility.Visible;
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            this.Type.Source = new BitmapImage(new Uri(string.Format("{0}/icon{1}.png", this.url, this.model.TypeId)));
            base.Background = new BrushConverter().ConvertFromInvariantString("#c7ddbf") as Brush;
            this.KnowledgeName.Foreground = new BrushConverter().ConvertFromInvariantString("#666666") as Brush;
            this.CreateTime.Foreground = new BrushConverter().ConvertFromInvariantString("#999999") as Brush;
            this.DeleteOne.Visibility = Visibility.Hidden;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            BACAClient.Model.Collection model = new BACAClient.Model.Collection
            {
                UserID = this.UserId,
                LngId = this.model.LngId,
                TypeID = this.model.TypeId
            };
            MESSAGE message = new MESSAGE();
            OpenWindows.Prompt(message.RemoveSuccess);
            this.Refresh();
        }
         
        public KnowledgeAndRelease model { get; set; }

        public string UserId { get; set; }

        public delegate void ChangedEvent(KnowledgeAndRelease model);

        public delegate void Event();
    }
}

