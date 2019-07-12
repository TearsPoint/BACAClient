namespace BACAClient.Pages.Main
{
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.Template;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public partial class Announcement : BasePage
    { 
        public Announcement()
        {
            this.InitializeComponent();
        }
         
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.Assignment();
        }

        public void Assignment()
        {
        }
         
    }
}

