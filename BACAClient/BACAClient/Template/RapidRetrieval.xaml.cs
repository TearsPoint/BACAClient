namespace BACAClient.Template
{
    using BACAClient.Common;
    using BACAClient.Controllers;
    using BACAClient.Model;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    public partial class RapidRetrieval : UserControl
    {
        private bool IsGood = false; 
        private string url = "pack://application:,,,/BACAClient;component/Images/Pages/Search";
        public event ChangedEventHandler gotokdp;

        public RapidRetrieval()
        {
            this.InitializeComponent();
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.IsGood)
                {
                    this.IsGood = false;
                }
                else
                {
                    this.gotokdp(this.model);
                }
            }
            catch
            {
            }
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.Assignment();
        }

        public void Assignment()
        {
            try
            {
                base.Width = this.width;
                this.recordindex.Text = this.index.ToString();
                this.key1.Text = this.model.Key1;
                this.category.Text = this.model.CategoryName;
                this.key4.Text = this.model.Key4;
                this.key2.Text = this.model.Key2;
                this.intro.Text = ReplaceUtils.NoHTML(HttpUtility.HtmlDecode(this.model.Intro));
                this.key1.ToolTip = this.model.Key1;
                this.key4.ToolTip = this.model.Key4;
                this.key2.ToolTip = this.model.Key2;
                this.intro.ToolTip = this.model.Key1;
                this.category.ToolTip = this.model.CategoryName;
                this.dataType.Source = new BitmapImage(new Uri(string.Format("{0}/icon{1}.png", this.url, this.model.TypeId)));
                if (this.model.SourceID == 9)
                {
                    this.selfBuilt.Source = new BitmapImage(new Uri(string.Format("{0}/icon_self.png", this.url)));
                }
                if (string.IsNullOrEmpty(this.model.PDFName) || string.IsNullOrEmpty(this.model.PDFPath))
                {
                    this.DownLoad.Visibility = Visibility.Hidden;
                }
                if (this.good != null)
                {
                    string keyword = string.Format("{0}_{1}", this.model.TypeId, this.model.LngId);
                    string goodCount = "0";  //todo
                    this.Count.Text = goodCount;
                    this.Count.ToolTip = goodCount;
                }
            }
            catch
            {
            }
        }

        private void DownLoad_Click(string Name)
        {
            try
            {
                this.IsGood = true;
                UserActionUtils.DownLoad(this.model.PDFPath, this.model.PDFName, this.model.TypeName, this.model.Key1);
            }
            catch
            {
            }
        }

        private void Good_Click(string Name)
        {
            try
            {
                this.IsGood = true;
                int num = UserActionUtils.WriteGood(this.Count.Text, this.UserID, this.model);
                if (num != 0)
                {
                    this.Count.Text = num.ToString();
                    this.Count.ToolTip = num.ToString();
                }
            }
            catch
            {
            }
        }

        public List<Good_top> good { get; set; }

        public int index { get; set; }

        public Knowledge model { get; set; }

        public string UserID { get; set; }

        public double width { get; set; }

        public delegate void ChangedEventHandler(Knowledge model);
    }
}

