namespace BACAClient.Template
{
    using BACAClient.Model;
    using fastJSON;
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

    public partial class IndexInfo : UserControl
    {
        public event ChangedEventHandler gotokdp;

        public IndexInfo()
        {
            this.InitializeComponent();
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            this.gotokdp(this.model);
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.Title.Content = this.model.GetStr("title");
            base.ToolTip = this.model.GetStr("title");
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            base.Background = new BrushConverter().ConvertFromInvariantString("#3399cc") as Brush;
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            base.Background = new BrushConverter().ConvertFromInvariantString("#98cde6") as Brush;
        }
        
        public DynamicJson model { get; set; }

        public delegate void ChangedEventHandler(DynamicJson model);
    }
}

