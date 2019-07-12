namespace BACAClient.OtherWindow
{
    using BACAClient.Model;
    using BACAClient.Template;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class Announcement : Window
    {
        public Announcement()
        {
            this.InitializeComponent();
        }
        
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.AnnouncementTitle.Content = this.model.Title;
            this.Content.Text = this.model.Content;
            this.CreateTime.Content = this.model.CreateTime.ToString("yyyy-MM-dd");
        }

        private void _MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Content.Width = base.ActualWidth - 120.0;
            this.ContentBox.MaxHeight = base.ActualHeight - 190.0;
            this.Content.MinHeight = base.ActualHeight - 190.0;
        }

        private void Close_Click(string Name)
        {
            base.Close();
        }
         
        public BACAClient.Model.Announcement model { get; set; }
    }
}

