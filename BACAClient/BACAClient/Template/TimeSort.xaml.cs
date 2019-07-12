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
    using System.Windows.Media.Imaging;

    public partial class TimeSort : UserControl
    {
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Menu/History/";

        
        public event ChangedEventHandler Click;

        public TimeSort()
        {
            this.InitializeComponent();
        }

        private void Grid_Click(object sender, MouseButtonEventArgs e)
        {
            string sort = "desc";
            if (base.Tag.ToString() == "desc")
            {
                sort = "asc";
            }
            base.Tag = sort;
            this.Icon.Source = new BitmapImage(new Uri(this.url + sort + ".png"));
            this.Click(sort);
        }
        
        public delegate void ChangedEventHandler(string Sort);
    }
}

