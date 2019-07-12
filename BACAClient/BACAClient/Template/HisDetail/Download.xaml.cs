namespace BACAClient.Template.HisDetail
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
    using System.Windows.Media;

    public partial class Download : UserControl
    {
        public Download()
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
            UserActionUtils.DownLoad(this.model.PDFPath, this.model.PDFName, this.model.TypeName, this.model.Key1);
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.typeretrieve != null)
                {
                    this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgColor) as Brush;
                }
            }
            catch
            {
            }
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.typeretrieve.BgHover))
            {
                this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgHover) as Brush;
            }
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.typeretrieve.BgColor))
            {
                this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgColor) as Brush;
            }
        }

        public Knowledge model { get; set; }

        public TypeRetrieve typeretrieve { get; set; }
    }
}

