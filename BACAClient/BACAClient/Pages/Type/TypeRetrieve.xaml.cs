namespace BACAClient.Pages.Type
{
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.Template;
    using BACAClient.UserControls.Controllers;
    using BACAClient.UserControls.Controls;
    using BACAClient.UserControls.Controls.FrontDesk;
    using JsonModel;
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
    using System.Windows.Media;

    public partial class TypeRetrieve : BasePage
    {
        private CacheParameterName cache = new CacheParameterName();
        private string category = string.Empty;
        private string CategoryId = string.Empty;
        private List<Knowledge> data = null;
        private bool IsReturn = true;
        private int pageIndex = 1;
        private BACAClient.Model.ReturnSearch returnsearch = null;
        private BACAClient.Model.TypeRetrieve tRetrieve = new BACAClient.Model.TypeRetrieve();
        private TypeInitialization TYPE = new TypeInitialization();
        private int TypeId = 1;
        private string UserDep = string.Empty;

        public TypeRetrieve()
        {
            this.InitializeComponent();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ReturnSearch();
            }
            catch
            {
            }
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double num = (base.ActualWidth - 154.0) - 70.0;
            foreach (UIElement element in this.List.Children)
            {
                if (element is BACAClient.UserControls.Controls.FrontDesk.TypeRetrieve)
                {
                    BACAClient.UserControls.Controls.FrontDesk.TypeRetrieve retrieve = (BACAClient.UserControls.Controls.FrontDesk.TypeRetrieve)element;
                    retrieve.Width = num + 20.0;
                }
            }
            foreach (UIElement element2 in this.Page1.Children)
            {
                if (element2 is BACAClient.UserControls.Controls.Page)
                {
                    BACAClient.UserControls.Controls.Page page = (BACAClient.UserControls.Controls.Page)element2;
                    page.Width = num + 20.0;
                }
            }
            foreach (UIElement element3 in this.Page2.Children)
            {
                if (element3 is BACAClient.UserControls.Controls.Page)
                {
                    BACAClient.UserControls.Controls.Page page2 = (BACAClient.UserControls.Controls.Page)element3;
                    page2.Width = num + 20.0;
                }
            }
        }

        private void AllCheck_Click(object sender, RoutedEventArgs e)
        {
            this.BoxChange(false);
        }

        private void AllCheckBtn_Click(string Name)
        {
            this.BoxChange(true);
        }

        public void BindControls()
        {
            string str = string.Format("pack://application:,,,/BACAClient;component/Images/Pages/Nav/HisList/TreeView/nav{0}.png", this.TypeId);
            this.AllCheckBtn.Content = this.tRetrieve.SelectedButton;
            this.AllCheckBtn.BackGround = str;
            this.AllCheckBtn.Hover = str;
            this.keyword.Tag = this.tRetrieve.TextBoxMessage;
            this.Search.Content = this.tRetrieve.Button;
            char[] separator = new char[] { ',' };
            string[] strArray = this.tRetrieve.ComBoxItems.Split(separator);
            int num = 0;
            this.Type.Items.Clear();
            foreach (string str2 in strArray)
            {
                ComboBoxItem item = new ComboBoxItem();
                this.Type.Items.Add(new ComboxItem(str2, num.ToString()));
                num++;
            }
            if ((this.returnsearch != null) && (this.returnsearch.IsReturn == 1.ToString()))
            {
                this.pageIndex = this.returnsearch.PageIndex;
                this.Type.Text = this.returnsearch.Key;
                this.keyword.Text = this.returnsearch.KeyWord;
            }
            else
            {
                this.Type.Text = strArray[0];
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
                Width = (base.ActualWidth - 154.0) - 70.0,
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
            List<Knowledge> listByPage = new List<Knowledge>();  //todo
            string str = string.Empty;
            foreach (Knowledge knowledge in listByPage)
            {
                str = str + string.Format("'{0}_{1}',", knowledge.TypeId, knowledge.LngId);
            }
            char[] trimChars = new char[] { ',' };
            List<Good_top> list = new List<Good_top>();  //todo
            int num = ((this.pageIndex - 1) * PageUtils.GetPageSize()) + 1;
            this.List.Children.Clear();
            foreach (Knowledge knowledge2 in listByPage)
            {
                BACAClient.UserControls.Controls.FrontDesk.TypeRetrieve element = new BACAClient.UserControls.Controls.FrontDesk.TypeRetrieve
                {
                    model = knowledge2,
                    good = list,
                    index = num,
                    width = (base.ActualWidth - 154.0) - 70.0,
                    UserID = this.returnsearch.UserId
                };
                element.gotokdp += new BACAClient.UserControls.Controls.FrontDesk.TypeRetrieve.ChangedEventHandler(this.gotokdp);
                this.List.Children.Add(element);
                num++;
            }
        }

        public void BoxChange(bool isBtn)
        {
            bool flag = true;
            string str = string.Format("bg{0}", this.TypeId);
            string str2 = "bg_hover";
            string color = this.tRetrieve.Color;
            bool? isChecked = this.AllCheck.IsChecked;
            bool flag3 = true;
            if ((isChecked.GetValueOrDefault() == flag3) ? isChecked.HasValue : false)
            {
                if (isBtn)
                {
                    flag = false;
                    this.AllCheck.IsChecked = false;
                    str = "bg";
                    color = "#716B6B";
                }
                else
                {
                    this.AllCheck.IsChecked = true;
                }
            }
            else if (isBtn)
            {
                this.AllCheck.IsChecked = true;
            }
            else
            {
                flag = false;
                this.AllCheck.IsChecked = false;
                str = "bg";
                color = "#716B6B";
            }
            //foreach (UIElement element in this.Tree.Children)
            //{
            //    if (element is BACAClient.Template.TreeView)
            //    {
            //        BACAClient.Template.TreeView view = (BACAClient.Template.TreeView)element;
            //        view.TreeBox.IsChecked = new bool?(flag);
            //        view.BackGround = str;
            //        view.Hover = str2;
            //        view.Title.Foreground = new BrushConverter().ConvertFromInvariantString(color) as Brush;
            //    }
            //}
        }

        public void Category()
        {
            //this.Tree.Children.Clear();
            //List<BACAClient.Model.Category> list = new List<Model.Category>();
            //foreach (BACAClient.Model.Category category in list)
            //{
            //    BACAClient.Template.TreeView element = new BACAClient.Template.TreeView
            //    {
            //        TypeId = this.TypeId,
            //        model = category,
            //        Color = this.tRetrieve.Color
            //    };
            //    element.BoxChanged += new BACAClient.Template.TreeView.ChangedEventHandler(this.OneCategoryClick);
            //    this.Tree.Children.Add(element);
            //}
        }

        public void CategoryIsChecked()
        {
            if (!string.IsNullOrEmpty(this.returnsearch.Category))
            {
                this.category = this.returnsearch.Category;
            }
            //foreach (object obj2 in this.Tree.Children)
            //{
            //    if (obj2 is BACAClient.Template.TreeView)
            //    {
            //        BACAClient.Template.TreeView view = (BACAClient.Template.TreeView)obj2;
            //        char[] trimChars = new char[] { ',' };
            //        char[] separator = new char[] { ',' };
            //        foreach (string str in this.category.TrimEnd(trimChars).Split(separator))
            //        {
            //            if (view.model.CategoryID.ToString() == str)
            //            {
            //                view.TreeBox.IsChecked = true;
            //                view.BackGround = string.Format("bg{0}", this.TypeId);
            //                view.Hover = "bg_hover";
            //                view.Title.Foreground = new BrushConverter().ConvertFromInvariantString(this.tRetrieve.Color) as Brush;
            //            }
            //        }
            //    }
            //}
        }

        private void data_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.Statistic();
                this.GetData();
            }
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            BACAClient.Model.Feedback model = new BACAClient.Model.Feedback
            {
                PageName = this.tRetrieve.PageName,
                SearchType = ((ComboxItem)this.Type.SelectedItem).Text,
                SearchInfo = this.keyword.Text,
                Category = this.GetCategoryID(false),
                PageIndex = this.pageIndex
            };
            OpenWindows.Feedback(model);
        }

        private string GetCategoryID(bool isCategoryID)
        {
            this.category = string.Empty;
            //foreach (UIElement element in this.Tree.Children)
            //{
            //    if (element is BACAClient.Template.TreeView)
            //    {
            //        BACAClient.Template.TreeView view = (BACAClient.Template.TreeView)element;
            //        bool? isChecked = view.TreeBox.IsChecked;
            //        bool flag3 = true;
            //        if ((isChecked.GetValueOrDefault() == flag3) ? isChecked.HasValue : false)
            //        {
            //            if (isCategoryID)
            //            {
            //                this.category = this.category + view.model.CategoryID + ",";
            //            }
            //            else
            //            {
            //                this.category = this.category + view.model.CategoryName + ",";
            //            }
            //        }
            //    }
            //}
            //char[] trimChars = new char[] { ',' };
            //return (this.category = this.category.TrimEnd(trimChars));
            return string.Empty;
        }

        public void GetData()
        {
            DoSearch model = new DoSearch();
            int iNT = int.Parse(((ComboxItem)this.Type.SelectedItem).Values);
            if (iNT == 0)
            {
                iNT = this.TYPE.INT;
            }
            model.TypeId = this.returnsearch.TypeId;
            model.Key = iNT;
            model.SourceID = this.TYPE.INT;
            model.KeyWord = this.keyword.Text;
            this.GetCategoryID(true);
            model.Category = this.category;
            this.data = new List<Knowledge>();  //todo
            this.Category();
            this.CategoryIsChecked();
            if ((this.data != null) && (this.data.Count > 0))
            {
                this.IsNull.Visibility = Visibility.Hidden;
                this.NotNull.Visibility = Visibility.Visible;
            }
            else
            {
                this.IsNull.Visibility = Visibility.Visible;
                this.NotNull.Visibility = Visibility.Hidden;
                this.RecordsNumber.Visibility = Visibility.Hidden;
                return;
            }
            if (this.data.Count == 1)
            {
                this.IsReturn = false;
                this.gotokdp(this.data[0]);
            }
            if (this.data.Count > 100)
            {
                this.num.Content = this.data.Count;
                this.RecordsNumber.Visibility = Visibility.Visible;
            }
            else
            {
                this.RecordsNumber.Visibility = Visibility.Hidden;
            }
            this.BoundControls();
        }

        public void gotokdp(Knowledge model)
        {
            if (model.TypeId == 3)
            {
                new PDF { Id = model.Id, TypeId = model.TypeId }.Show();
            }
            else
            {
                this.returnsearch = new BACAClient.Model.ReturnSearch();
                if (this.IsReturn)
                {
                    SystemEnum.ISRETURN yES = SystemEnum.ISRETURN.YES;
                    this.returnsearch.IsReturn = yES.ToString();
                }
                else
                {
                    this.returnsearch.IsReturn = SystemEnum.ISRETURN.NO.ToString();
                }
                this.returnsearch.TypeId = this.TypeId;
                this.returnsearch.PageIndex = this.pageIndex;
                this.returnsearch.SourceId = 0;
                this.returnsearch.KeyWord = this.keyword.Text;
                this.returnsearch.Category = this.GetCategoryID(true);
                this.returnsearch.Pages = new PageSwitch().TypeRetrieve;
                this.returnsearch.Key = ((ComboxItem)this.Type.SelectedItem).Text;
                CacheHelper.SetReturnSearch(this.returnsearch);
                CacheHelper.SetCache(new CacheParameterName().HistoryIndex, string.Empty);
                CacheHelper.SetCache(this.cache.Id, model.Id.ToString());
                base.ParentWindow.PageSwitch(new PageSwitch().kdp);
            }
        }

        private void OneCategoryClick()
        {
            this.GetData();
        }

        private void PagerIndexChanged(int i)
        {
            this.pageIndex = i;
            this.BoundControls();
        }

        public void ReturnSearch()
        {
             //todo
        }

        private void Search_Click(string Name)
        {
            this.Statistic();
            this.GetData();
        }

        public void Statistic()
        {
            UserActionUtils.WriteSearchHistory(this.TypeId, ((ComboxItem)this.Type.SelectedItem).Text, this.keyword.Text, this.category, "全部资源", this.returnsearch.UserId);
        }

    }
}

