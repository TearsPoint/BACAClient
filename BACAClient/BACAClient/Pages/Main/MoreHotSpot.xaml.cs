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
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public partial class MoreHotSpot : BasePage
    {
        private string Category = string.Empty;
        private List<BACAClient.Model.GoodAndHot> data = null; 

        
        public event ChangedEventHandler gotokdp;

        public MoreHotSpot()
        {
            this.InitializeComponent();
        }
        
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.More3.Visibility = Visibility.Hidden;
            this.More4.Visibility = Visibility.Hidden;
            this.BackGroud(this.Type);
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (UIElement element in this.List.Children)
            {
                if (element is More)
                {
                    More more = (More) element;
                    more.Width = base.ActualWidth - 30.0;
                }
            }
        }

        public void BackGroud(string MoreType)
        {
            this.More1.BorderThickness = new Thickness(0.0);
            this.More2.BorderThickness = new Thickness(0.0);
            this.More3.BorderThickness = new Thickness(0.0);
            this.More4.BorderThickness = new Thickness(0.0);
            string tableName = string.Empty;
            string str2 = MoreType;
            if (!(str2 == "近期热门"))
            {
                if (str2 == "点赞推荐")
                {
                    this.More2.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                    this.Category = string.Empty;
                    tableName = new Good().GetType().Name;
                }
                else if (str2 == "热门")
                {
                    this.More3.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                    this.More3.Visibility = Visibility.Visible;
                    this.More4.Visibility = Visibility.Visible;
                    this.Category = this.UserDep;
                    tableName = new BACAClient.Model.HotSpot().GetType().Name;
                }
                else if (str2 == "点赞")
                {
                    this.More4.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                    this.More3.Visibility = Visibility.Visible;
                    this.More4.Visibility = Visibility.Visible;
                    this.Category = this.UserDep;
                    tableName = new Good().GetType().Name;
                }
            }
            else
            {
                this.More1.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                this.Category = string.Empty;
                tableName = new BACAClient.Model.HotSpot().GetType().Name;
            }
            this.GoodAndHot(tableName);
        }

        public void BindControls(int i, int PageCount, int type)
        {
        }

        private void ChangeType(string Name)
        {
            this.BackGroud(Name);
        }

        private void GoodAndHot(string TableName)
        {
            this.List.Children.Clear();
            this.data = new List<Model.GoodAndHot>();//todo
            if (this.data == null)
            {
                this.IsNull.Visibility = Visibility.Visible;
            }
            else
            {
                int num = 1;
                foreach (BACAClient.Model.GoodAndHot hot in this.data)
                {
                    More element = new More {
                        Index = num,
                        width = base.ActualWidth - 30.0,
                        model = hot
                    };
                    element.gotokdp += new More.ChangedEventHandler(this.goTokdp);
                    this.List.Children.Add(element);
                    num++;
                }
            }
        }

        public void goTokdp(BACAClient.Model.GoodAndHot model)
        {
            this.gotokdp(model);
        }
         
        private void PagerIndexChanged(int pageIndex)
        {
        }
         
        public string Type { get; set; }

        public string UserDep { get; set; }

        public delegate void ChangedEventHandler(GoodAndHot model);
    }
}

