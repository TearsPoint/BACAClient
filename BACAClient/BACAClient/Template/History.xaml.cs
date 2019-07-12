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
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class History : UserControl
    {
        private bool IsOpen = true; 
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Index/Left/";
        public event ChangedEvent gotokdp;

        public History()
        {
            this.InitializeComponent();
            this.PreviewMouseUp += Button_PreviewMouseUp;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            This_Click(sender, e);
        }

        public void Assignment()
        {
            try
            {
                base.Width = this.width;
                this.Icon.Source = new BitmapImage(new Uri(string.Format("{0}icon{1}.png", this.url, this.model.TypeId)));
                this.Type.Text = this.model.TypeName;
                this.Name.Text = this.model.Name;
                this.index.Text = (this.Index + 1).ToString();
                DateTime createTime = this.model.CreateTime;
                if (true)
                {
                    this.CreateTime.Text = this.model.CreateTime.ToString("yyyy-MM-dd");
                }
            }
            catch
            {
            }
        }

        private void DeleteOne_Click(string Name)
        {
            this.IsOpen = false;
            MESSAGE message = new MESSAGE();
            string data = string.Empty;
            //if (new LocalHistoryUtils().RemoveChildNodes(this.model))
            //{
            //    data = message.RemoveSuccess;
            //    this.gotokdp(null, this.Index);
            //}
            //else
            //{
            //    data = message.RemoveError;
            //}
            OpenWindows.Prompt(data);
        }
        
        private void This_Click(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsOpen)
            {
                this.IsOpen = true;
            }
            else
            {
                this.gotokdp(this.model, this.Index);
            }
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            this.Assignment();
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            this.DeleteOne.Visibility = Visibility.Visible;
            this.grid.Background = new BrushConverter().ConvertFromInvariantString("#dddddd") as Brush;
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            this.DeleteOne.Visibility = Visibility.Hidden;
            this.grid.Background = Brushes.White;
        }

        public int Index { get; set; }

        public BACAClient.Model.History model { get; set; }

        public double width { get; set; }

        public delegate void ChangedEvent(BACAClient.Model.History model, int index);
    }
}

