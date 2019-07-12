namespace BACAClient.Pages.Main
{
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.UserControls.Controllers;
    using BACAClient.UserControls.Controls;
    using BACAClient.UserControls.Controls.FrontDesk;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class RapidRetrieval : BasePage
    { 
        private CacheParameterName cache = new CacheParameterName();
        private List<BACAClient.Model.Knowledge> data = null;
        private bool IsReturn = true; 
        private int pageIndex = 1; 
        private BACAClient.Model.Response Response = new BACAClient.Model.Response();
        private ReturnSearch returnsearch = null;
        private SystemEnum.SOURCETYPE SourceID = SystemEnum.SOURCETYPE.ALL;
        private TypeInitialization TYPE = new TypeInitialization();

        public RapidRetrieval()
        {
            this.InitializeComponent();
        }
        
        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.Statistic();
                this.GetData();
            }
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.RadioChecked();
            this.GetData();
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double num = base.ActualWidth - 70.0;
            foreach (UIElement element in this.List.Children)
            {
                if (element is BACAClient.UserControls.Controls.FrontDesk.RapidRetrieval)
                {
                    BACAClient.UserControls.Controls.FrontDesk.RapidRetrieval retrieval = (BACAClient.UserControls.Controls.FrontDesk.RapidRetrieval)element;
                    retrieval.Width = num;
                }
            }
            foreach (UIElement element2 in this.Page1.Children)
            {
                if (element2 is BACAClient.UserControls.Controls.Page)
                {
                    BACAClient.UserControls.Controls.Page page = (BACAClient.UserControls.Controls.Page)element2;
                    page.Width = num;
                }
            }
            foreach (UIElement element3 in this.Page2.Children)
            {
                if (element3 is BACAClient.UserControls.Controls.Page)
                {
                    BACAClient.UserControls.Controls.Page page2 = (BACAClient.UserControls.Controls.Page)element3;
                    page2.Width = num;
                }
            }
        }

        public void BindPageControl(int PageCount, int type)
        {
            Grid grid = this.Page1;
            if (type == 2)
            {
                grid = this.Page2;
            }
            BACAClient.UserControls.Controls.Page element = new BACAClient.UserControls.Controls.Page
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = base.ActualWidth - 70.0,
                PageIndex = this.pageIndex,
                PageCount = PageCount
            };
            element.PagerIndexChanged += new BACAClient.UserControls.Controls.Page.ChangedEventHandler(this.PagerIndexChanged);
            grid.Children.Add(element);
        }

        public void BoundControls()
        {
            if (this.data.Count > 10)
            {
                this.Page1.Children.Clear();
                this.Page2.Children.Clear();
                int pageCount = PageUtils.GetPageCount(this.data.Count);
                this.BindPageControl(pageCount, 1);
                this.BindPageControl(pageCount, 2);
            }
            List<BACAClient.Model.Knowledge> listByPage = new List<Model.Knowledge>();//todo
            string str = string.Empty;
            foreach (BACAClient.Model.Knowledge knowledge in listByPage)
            {
                str = str + string.Format("'{0}_{1}',", knowledge.TypeId, knowledge.LngId);
            }
            char[] trimChars = new char[] { ',' };
            List<Good_top> list = new List<Good_top>(); //todo
            int num = ((this.pageIndex - 1) * PageUtils.GetPageSize()) + 1;
            this.List.Children.Clear();
            foreach (BACAClient.Model.Knowledge knowledge2 in listByPage)
            {
                BACAClient.UserControls.Controls.FrontDesk.RapidRetrieval element = new BACAClient.UserControls.Controls.FrontDesk.RapidRetrieval
                {
                    model = knowledge2,
                    good = list,
                    index = num,
                    width = base.ActualWidth - 70.0,
                    UserID = this.returnsearch.UserId
                };
                element.gotokdp += new BACAClient.UserControls.Controls.FrontDesk.RapidRetrieval.ChangedEventHandler(this.gotokdp);
                this.List.Children.Add(element);
                num++;
            }
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            BACAClient.Model.Feedback model = new BACAClient.Model.Feedback
            {
                PageName = "快速检索",
                SearchType = this.RadioButtonCheck(2),
                SearchInfo = this.keyword.Text
            };
            OpenWindows.Feedback(model);
        }

        public void GetData()
        {
            //bool? isChecked = this.All.IsChecked;
            //bool flag2 = true;
            //if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            //{
            //    this.SourceID = SystemEnum.SOURCETYPE.ALL;
            //}
            //else
            //{
            //    isChecked = this.Knowledge.IsChecked;
            //    flag2 = true;
            //    if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            //    {
            //        this.SourceID = SystemEnum.SOURCETYPE.SYSTEM;
            //    }
            //    else
            //    {
            //        isChecked = this.Recommend.IsChecked;
            //        flag2 = true;
            //        if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            //        {
            //            this.SourceID = SystemEnum.SOURCETYPE.SELF;
            //        }
            //    }
            //}
            //DoSearch model = new DoSearch
            //{
            //    TypeId = 0,
            //    Key = this.TYPE.INT,
            //    IsDell = this.TYPE.INT,
            //    SourceID = (int)this.SourceID,
            //    KeyWord = this.keyword.Text
            //};
            //string category = string.Empty;
            //this.data = new List<Model.Knowledge>(); //todo
            //if ((this.data != null) && (this.data.Count > 0))
            //{
            //    this.IsNull.Visibility = Visibility.Hidden;
            //    this.NotNull.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    this.IsNull.Visibility = Visibility.Visible;
            //    this.NotNull.Visibility = Visibility.Hidden;
            //    this.RecordsNumber.Visibility = Visibility.Hidden;
            //    return;
            //}
            //if (this.data.Count == 1)
            //{
            //    this.IsReturn = false;
            //    this.gotokdp(this.data[0]);
            //}
            //if (this.data.Count > 100)
            //{
            //    this.Number.Content = this.data.Count;
            //    this.RecordsNumber.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    this.RecordsNumber.Visibility = Visibility.Hidden;
            //}
            //this.Page1.Children.Clear();
            //this.Page2.Children.Clear();
            //this.BoundControls();
        }

        public void gotokdp(BACAClient.Model.Knowledge model)
        {
            if (model.TypeId == 3)
            {
                new PDF { Id = model.Id, TypeId = model.TypeId }.Show();
            }
            else
            {
                this.returnsearch = new ReturnSearch();
                if (this.IsReturn)
                {
                    SystemEnum.ISRETURN yES = SystemEnum.ISRETURN.YES;
                    this.returnsearch.IsReturn = yES.ToString();
                }
                else
                {
                    this.returnsearch.IsReturn = SystemEnum.ISRETURN.NO.ToString();
                }
                this.returnsearch.TypeId = 0;
                this.returnsearch.PageIndex = this.pageIndex;
                this.returnsearch.SourceId = (int)this.SourceID;
                this.returnsearch.KeyWord = this.keyword.Text;
                this.returnsearch.Pages = new PageSwitch().RapidRetrieval;
                CacheHelper.SetReturnSearch(this.returnsearch);
                CacheHelper.SetCache(this.cache.HistoryIndex, string.Empty);
                CacheHelper.SetCache(this.cache.Id, model.Id.ToString());
                base.ParentWindow.PageSwitch(new PageSwitch().kdp);
            }
        }
        
        private void PagerIndexChanged(int i)
        {
            this.pageIndex = i;
            this.BoundControls();
        }

        public string RadioButtonCheck(int type)
        {
            string str = string.Empty;
            foreach (UIElement element in this.RadioList.Children)
            {
                if (element is RadioButton)
                {
                    RadioButton button = (RadioButton)element;
                    bool? isChecked = button.IsChecked;
                    bool flag3 = true;
                    if ((isChecked.GetValueOrDefault() == flag3) ? isChecked.HasValue : false)
                    {
                        if (type == 1)
                        {
                            return button.Tag.ToString();
                        }
                        return button.Content.ToString();
                    }
                }
            }
            return str;
        }

        public void RadioChecked()
        {
            try
            {
                this.returnsearch = CacheHelper.GetReturnSearchModel();
                if (this.returnsearch != null)
                {
                    SystemEnum.ISRETURN yES = SystemEnum.ISRETURN.YES;
                    if (this.returnsearch.IsReturn == yES.ToString())
                    {
                        this.pageIndex = this.returnsearch.PageIndex;
                    }
                    this.keyword.Text = this.returnsearch.KeyWord;
                    switch (this.returnsearch.SourceId)
                    { 
                    }
                }
            }
            catch
            {
            }
        }

        private void Search_Click(string Name)
        {
            this.Statistic();
            this.GetData();
        }

        public void Statistic()
        {
            UserActionUtils.WriteSearchHistory(0, string.Empty, this.keyword.Text, string.Empty, this.RadioButtonCheck(2), this.returnsearch.UserId);
        }
          
    }
}

