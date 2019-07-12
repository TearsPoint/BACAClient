namespace BACAClient.Template.HisDetail
{
    using BACAClient.Model;
    using BACAClient.UserControls.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;

    public partial class Good : UserControl
    {
        public string url = "pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisDetail/Icon/";

        public Good()
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
            int num = UserActionUtils.WriteGood(this.GoodCount.Text.ToString(), this.UserId, this.model);
            if (num != 0)
            {
                this.GoodCount.Text = num.ToString();
            }
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.typeretrieve != null)
                {
                    this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgColor) as Brush;
                    string str = string.Format("{0}_{1}", this.model.TypeId, this.model.LngId);
                    List<Good_top> data = new List<Good_top>();  
                    this.GoodCount.Text = data.Count.ToString();  
                }
            }
            catch
            {
            }
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgHover) as Brush;
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            this.grid.Background = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.BgColor) as Brush;
        }

        public Knowledge model { get; set; }

        public TypeRetrieve typeretrieve { get; set; }

        public string UserId { get; set; }
    }
}

