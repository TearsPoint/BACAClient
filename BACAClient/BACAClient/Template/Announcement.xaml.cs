namespace BACAClient.Template
{
    using BACAClient.Model;
    using BACAClient.UserControls.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class Announcement : UserControl
    {
        public Announcement()
        {
            this.InitializeComponent();
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            OpenWindows.Announcement(this.model);
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            base.ToolTip = this.model.Title;
            this.name.Text = this.model.Title;
            this.time.Text = this.model.CreateTime.ToString("yyyy-MM-dd");
        }

         
        public BACAClient.Model.Announcement model { get; set; }
    }
}

