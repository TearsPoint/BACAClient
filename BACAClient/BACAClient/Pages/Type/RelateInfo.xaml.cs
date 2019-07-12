namespace BACAClient.Pages.Type
{
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.Template;
    using BACAClient.Template.HisDetail;
    using BACAClient.UserControls.Controls;
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

    public partial class RelateInfo : BasePage
    {
        private List<BACAClient.Model.Relational> data = null;
        private int PageIndex = 1;
        private Response response = null;

        
        public event ChangedEventHandler gotokdp;

        public RelateInfo()
        {
            this.InitializeComponent();
        }
        
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.GetData();
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double num = base.ActualWidth - 20.0;
            foreach (UIElement element in this.List.Children)
            {
                if (element is ReInfo)
                {
                    ReInfo info = (ReInfo) element;
                    info.Width = num;
                }
            }
            foreach (UIElement element2 in this.Page1.Children)
            {
                if (element2 is BACAClient.Template.Page)
                {
                    BACAClient.Template.Page page = (BACAClient.Template.Page) element2;
                    page.Width = num;
                }
            }
            foreach (UIElement element3 in this.Page2.Children)
            {
                if (element3 is BACAClient.Template.Page)
                {
                    BACAClient.Template.Page page2 = (BACAClient.Template.Page) element3;
                    page2.Width = num;
                }
            }
        }

        public void BindPageControl(int i, int PageCount, int type)
        {
            Grid grid = this.Page1;
            if (type == 2)
            {
                grid = this.Page2;
            }
            BACAClient.UserControls.Controls.Page element = new BACAClient.UserControls.Controls.Page {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = base.ActualWidth - 70.0,
                PageIndex = i,
                PageCount = PageCount
            };
            element.PagerIndexChanged += new BACAClient.UserControls.Controls.Page.ChangedEventHandler(this.PagerIndexChanged);
            grid.Children.Add(element);
        }

        public void BoundControls()
        {
            foreach (BACAClient.Model.Relational relational in this.data)
            {
                ReInfo element = new ReInfo {
                    width = base.ActualWidth - 20.0,
                    model = relational
                };
                element.gotokdp += new ReInfo.ChangedEventHandler(this.goTokdp);
                this.List.Children.Add(element);
            }
        }

        public void GetData()
        {
            this.Page1.Children.Clear();
            this.Page2.Children.Clear();
            this.List.Children.Clear();
            this.data = new List<Model.Relational>();  //todo
            if (this.response.RecordCount > 0)
            {
                if (this.response.PageCount > 1)
                {
                    this.BindPageControl(this.PageIndex, this.response.PageCount, 1);
                    this.BindPageControl(this.PageIndex, this.response.PageCount, 2);
                }
                this.BoundControls();
            }
        }

        public void goTokdp(int ReTypeId, string ReLngId)
        {
            if (ReLngId.Contains(","))
            {
                DocMore more = new DocMore {
                    ReTypeId = ReTypeId,
                    ReLngId = ReLngId
                };
                more.gotokdp += new DocMore.ChangedEventHandler(this.goToKdp);
                more.Show();
            }
            else
            {
                this.gotokdp(ReTypeId, int.Parse(ReLngId), string.Empty);
            }
        }

        public void goToKdp(int TypeId, int LngId, string parameter)
        {
            this.gotokdp(TypeId, LngId, string.Empty);
        }
        
        private void PagerIndexChanged(int pageIndex)
        {
            this.PageIndex = pageIndex;
            this.GetData();
        }
        
        public int LngId { get; set; }

        public int ReTypeId { get; set; }

        public int TypeId { get; set; }

        public delegate void ChangedEventHandler(int TypeId, int LngId, string parameter);
    }
}

