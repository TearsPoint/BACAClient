namespace BACAClient.Pages.Type
{ 
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.Template.HisDetail;
    using BACAClient.UserControls.Controllers;
    using BACAClient.UserControls.Controls;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml;

    public partial class Kdp : BasePage
    { 
        private CacheParameterName cache = new CacheParameterName(); 
        private int count = 0;
        private List<Node> data = null;
        private string FormImage = string.Empty;
        private FulltextXML fulltextxml = null;
        private List<BACAClient.Model.History> history = null;
        private int Id = 0;
        private int index = 0;
        private bool IsAddHistory = false;
        private Knowledge model = null;  
        private string NodeName = string.Empty;
        private PageSwitch pageswitch = new PageSwitch(); 
        internal System.Windows.Controls.Frame Right;
        private List<BACAClient.Model.Type> Type = null;
        private BACAClient.Model.TypeRetrieve typeretrieve = null;
        private InterfacesUsers users = new InterfacesUsers();

        public Kdp()
        {
            try
            {
                this.InitializeComponent();
                this.users = CacheHelper.GetInterfacesUsersModel();
                string cache = CacheHelper.GetCache(this.cache.HistoryIndex);
                if (!string.IsNullOrEmpty(cache))
                {
                    this.index = int.Parse(cache);
                }
                string str2 = CacheHelper.GetCache(this.cache.Id);
                if (!string.IsNullOrEmpty(str2))
                {
                    this.Id = int.Parse(str2);
                    this.GetData();
                }
                base.Loaded += new RoutedEventHandler(this._Loaded);
            }
            catch
            {
            }
        }
        
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                base.ParentWindow.SetNavIcon(this.model.TypeId);
            }
            catch
            {
            }
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                double num = (((((base.ActualHeight - this.row1.ActualHeight) - this.row2.ActualHeight) - this.ActualHeight) - 36.0) - 10.0) - 30.0;
                if (num < 200.0)
                {
                    num = 200.0;
                }
                this.Scroll.MaxHeight = num;
                double num2 = (((base.ActualWidth - 210.0) - 5.0) - (2 * this.count)) / ((double)this.count);
                foreach (UIElement element in this.Nav.Children)
                {
                    if (element is BACAClient.Template.HisDetail.Nav)
                    {
                        BACAClient.Template.HisDetail.Nav nav = (BACAClient.Template.HisDetail.Nav)element;
                        nav.Width = num2;
                    }
                }
            }
            catch
            {
            }
        }

        private void BackWard_Click()
        {
            try
            {
                while (this.index < this.history.Count)
                {
                    this.index--;
                    if (this.history[this.index].TypeId != 3)
                    {
                        break;
                    }
                }
                CacheHelper.SetCache(new CacheParameterName().Id, this.history[this.index].Id.ToString());
                if (this.IsAddHistory)
                {
                    this.index++;
                }
                CacheHelper.SetCache(new CacheParameterName().HistoryIndex, this.index.ToString());
                base.ParentWindow.PageSwitch(new PageSwitch().kdp);
            }
            catch
            {
            }
        }

        public void BasicInfo()
        {
            try
            {
                switch (this.model.TypeId)
                {
                    case 1:
                        this.Name.Text = this.fulltextxml.Name;
                        this.Name_E.Text = "英文名：" + this.fulltextxml.Name_E;
                        this.Name_A.Text = "别名：" + this.fulltextxml.Name_A;
                        this.CategoryName.Text = "所属科目：" + this.fulltextxml.CategoryName;
                        this.ICD.Text = "ICD号：" + this.fulltextxml.ICD;
                        break;

                    case 2:
                        this.Name.Text = this.fulltextxml.Name;
                        this.Name_E.Text = "英文名：" + this.fulltextxml.Name_E;
                        this.Name_A.Text = "别名：" + this.fulltextxml.Name_A;
                        this.CategoryName.Text = "检查科目：" + this.fulltextxml.CategoryName;
                        this.ICD.Text = "分类名称：" + this.fulltextxml.CategoryOtherName;
                        break;

                    case 4:
                        this.Name.Text = this.fulltextxml.Name;
                        this.Name_E.Text = "年份：" + this.fulltextxml.Years;
                        this.Name_A.Text = "版本：" + this.fulltextxml.Version;
                        this.CategoryName.Text = "ICD号：" + this.model.Key4;
                        this.ICD.Text = "标准住院流程";
                        this.FormImage = this.fulltextxml.Form;
                        break;

                    case 5:
                        this.Name.Text = this.fulltextxml.Name;
                        this.Name_E.Text = "英文名：" + this.fulltextxml.Name_E;
                        this.Name_A.Text = "别名：" + this.fulltextxml.Name_A;
                        this.CategoryName.Text = "疾病科目：" + this.fulltextxml.CategoryName;
                        this.ICD.Text = "ICD号：" + this.model.Key4;
                        break;

                    case 6:
                        this.Name.Text = this.fulltextxml.Name;
                        this.Name_E.Text = "别名：" + this.fulltextxml.Summary;
                        this.Name_A.Text = "疾病科目：" + this.fulltextxml.CategoryName;
                        break;
                }
                if (string.IsNullOrEmpty(this.fulltextxml.PDFPath) || string.IsNullOrEmpty(this.fulltextxml.PDFName))
                {
                    this.PDFPath.IsEnabled = false;
                    this.Down.IsEnabled = false;
                    this.PDFPath.Content = "无全文";
                    this.PDFPath.FontSize = 12.0;
                }
                else
                {
                    this.Down.model = this.model;
                }
                this.Name.Foreground = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.Color) as Brush;
                this.BasicBorder.BorderBrush = new BrushConverter().ConvertFromInvariantString(this.typeretrieve.Color) as Brush;
                this.InitializeStyle();
                this.RelationalInfo();
            }
            catch (Exception)
            {
            }
        }

        public int BindFrame(int type, int i)
        {
            int num3;
            try
            {
                DocDetail detail;
                int status = 0;
                switch (type)
                {
                    case 0:
                        detail = new DocDetail();
                        status = detail.status;
                        if (status != 1)
                        {
                            break;
                        }
                        goto Label_010E;

                    case 1:
                        {
                            RelateInfo info = new RelateInfo
                            {
                                LngId = this.model.LngId,
                                TypeId = this.model.TypeId,
                                ReTypeId = i
                            };
                            info.gotokdp += new RelateInfo.ChangedEventHandler(this.gotokdp);
                            this.Right.Content = info;
                            goto Label_010E;
                        }
                    case 2:
                        {
                            Form form = new Form
                            {
                                ImageList = this.FormImage
                            };
                            this.Right.Content = form;
                            goto Label_010E;
                        }
                    default:
                        goto Label_010E;
                }
                detail.data = this.data;
                detail.PageIndex = i;
                detail.Counts = this.data.Count;
                detail.ChangNav += new DocDetail.ChangedEventHandler(this.ChangNavStyle);
                detail.gotokdp += new DocDetail.ChangedEvent(this.gotokdp);
                this.Right.Content = detail;
                Label_010E:
                num3 = status;
            }
            catch
            {
                num3 = 0;
            }
            return num3;
        }

        public void ChangNavStyle(int i, int type)
        {
            try
            {
                foreach (UIElement element in this.Nav.Children)
                {
                    if (element is BACAClient.Template.HisDetail.Nav)
                    {
                        BACAClient.Template.HisDetail.Nav nav = (BACAClient.Template.HisDetail.Nav)element;
                        if (nav.enter == 1)
                        {
                            nav.Arrow.Visibility = Visibility.Hidden;
                            nav.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
                            nav.Bg.Background = Brushes.White;
                            nav.Bg.BorderThickness = new Thickness(0.0, 0.0, 0.0, 2.0);
                            nav.enter = 0;
                        }
                        if (nav.Index == i)
                        {
                            this.NodeName = nav.Title;
                            nav.ChangeStyle();
                            this.BindFrame(0, i);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void ChangStyle(int i)
        {
            try
            {
                foreach (UIElement element in this.relational.Children)
                {
                    if (element is BACAClient.Template.HisDetail.Relational)
                    {
                        BACAClient.Template.HisDetail.Relational relational = (BACAClient.Template.HisDetail.Relational)element;
                        if (relational.enter == 1)
                        {
                            if ((relational.model != null) && (relational.model.ReTypeId != 7))
                            {
                                string uriString = string.Format("pack://application:,,,/BACAClient;component/Images/Pages/Index/Left/icon{0}.png", relational.model.ReTypeId);
                                relational.Icon.Source = new BitmapImage(new Uri(uriString));
                            }
                            relational.Name.Foreground = new BrushConverter().ConvertFromInvariantString("#333333") as Brush;
                            relational.grid.Background = Brushes.White;
                            relational.Bg.BorderThickness = new Thickness(0.0, 0.0, 1.0, 0.0);
                            relational.enter = 0;
                        }
                    }
                }
                this.ChangNavStyle(i, 0);
            }
            catch
            {
            }
        }

        private void Collection_Click(string Name)
        {
            if (string.IsNullOrEmpty(this.users.UserId))
            {
                OpenWindows.Prompt(new MESSAGE().NotLogin);
            }
            else
            {
                UserActionUtils.WriteCollection(this.users.UserId, this.model);
            }
        }

        public void DetailNav()
        {
            try
            {
                int num = 0;
                bool flag = true;
                int i = 1;
                foreach (Node node in this.data)
                {
                    num++;
                    if (flag)
                    {
                        i = num;
                        this.NodeName = node.Name_c;
                    }
                    BACAClient.Template.HisDetail.Nav element = new BACAClient.Template.HisDetail.Nav
                    {
                        Index = num,
                        BgColor = this.typeretrieve.BgColor,
                        First = flag,
                        Title = node.Name_c,
                        TypeId = this.model.TypeId
                    };
                    element.Click += new BACAClient.Template.HisDetail.Nav.ChangedEventHandler(this.Nav_Click);
                    this.Nav.Children.Add(element);
                    flag = false;
                }
                this.BindFrame(0, i);
            }
            catch
            {
            }
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            BACAClient.Model.Feedback model = new BACAClient.Model.Feedback
            {
                TypeID = this.model.TypeId.ToString(),
                LngId = this.model.LngId.ToString(),
                Knowledge = this.fulltextxml.Name,
                NodeName = this.NodeName,
                PageName = "详细信息"
            };
            OpenWindows.Feedback(model);
        }

        private void FormClick(int relateid)
        {
            this.ChangStyle(0);
            this.BindFrame(2, 0);
        }

        private void ForWard_Click()
        {
            try
            {
                while (this.index < this.history.Count)
                {
                    this.index++;
                    if (this.history[this.index].TypeId != 3)
                    {
                        break;
                    }
                }
                CacheHelper.SetCache(new CacheParameterName().Id, this.history[this.index].Id.ToString());
                if (this.IsAddHistory)
                {
                    this.index++;
                }
                CacheHelper.SetCache(new CacheParameterName().HistoryIndex, this.index.ToString());
                base.ParentWindow.PageSwitch(new PageSwitch().kdp);
            }
            catch
            {
            }
        }

        public void GetData()
        {
             //todo
        }

        public void gotokdp(int TypeId, int LngId, string parameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    parameter = ReplaceUtils.ReplaceAHerf(parameter);
                    if (parameter.Contains(","))
                    {
                        if (!ShowEvent.WindowIsOpen(new DocMore().Title))
                        {
                            DocMore more = new DocMore();
                            more.gotokdp += new DocMore.ChangedEventHandler(this.gotokdp);
                            more.StrName = parameter;
                            more.Show();
                        }
                        return;
                    }
                    char[] separator = new char[] { '/' };
                    TypeId = int.Parse(parameter.Split(separator)[1]);
                    char[] chArray2 = new char[] { '/' };
                    LngId = int.Parse(parameter.Split(chArray2)[0]);
                }
                if (TypeId == 7)
                {
                    new PDF { TypeId = TypeId, ReLngId = LngId.ToString(), Owner = new BasePage().ParentWindow }.Show();
                }
                else
                {
                    int id = -1;  //todo
                    if (TypeId == 3)
                    {
                        new PDF { TypeId = TypeId, Id = id, Owner = new BasePage().ParentWindow }.Show();
                    }
                    else
                    {
                        CacheHelper.SetCache(new CacheParameterName().HistoryIndex, string.Empty);
                        CacheHelper.SetCache(this.cache.Id, id.ToString());
                        base.ParentWindow.PageSwitch(this.pageswitch.kdp);
                    }
                }
            }
            catch
            {
            }
        }
        
        public void InitializeStyle()
        {
            try
            {
                this.ForWard.typeretrieve = this.typeretrieve;
                this.BackWard.typeretrieve = this.typeretrieve;
                this.Down.typeretrieve = this.typeretrieve;
                this.Collection.BackGround = this.typeretrieve.Color;
                this.Collection.Hover = this.typeretrieve.BgHover;
                this.PDFPath.BackGround = this.typeretrieve.Color;
                this.PDFPath.Hover = this.typeretrieve.BgHover;
                this.Good.model = this.model;
                this.Good.UserId = this.users.UserId;
                this.Good.typeretrieve = this.typeretrieve;
            }
            catch
            {
            }
        }

        private void Nav_Click(int Number)
        {
            if (this.BindFrame(0, Number) != 1)
            {
                this.ChangStyle(Number);
            }
        }

        private void PDFPath_Click(string Name)
        {
            new PDF { Id = this.model.Id }.Show();
        }

        private void relateInfo(int ReTypeId)
        {
            this.ChangStyle(0);
            this.BindFrame(1, ReTypeId);
        }

        public void RelationalInfo()
        {
            try
            {
                if (this.model.TypeId == 4)
                {
                    if (!string.IsNullOrEmpty(this.FormImage))
                    {
                        BACAClient.Template.HisDetail.Relational element = new BACAClient.Template.HisDetail.Relational
                        {
                            TypeId = this.model.TypeId
                        };
                        element.RelateInfo += new BACAClient.Template.HisDetail.Relational.ChangedEventHandler(this.FormClick);
                        this.relational.Children.Add(element);
                    }
                }
                else
                {
                    List<BACAClient.Model.Relational> list = new List<Model.Relational>();  //todo
                    if ((list != null) && (list.Count > 0))
                    {
                        foreach (BACAClient.Model.Relational relational2 in list)
                        {
                            BACAClient.Template.HisDetail.Relational relational3 = new BACAClient.Template.HisDetail.Relational
                            {
                                model = relational2,
                                data = this.Type,
                                TypeId = this.model.TypeId
                            };
                            relational3.RelateInfo += new BACAClient.Template.HisDetail.Relational.ChangedEventHandler(this.relateInfo);
                            this.relational.Children.Add(relational3);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void Statistics()
        {
            try
            {
                UserActionUtils.WriteHotSpot(this.users.UserId, 0, this.model);
                this.history = new List<Model.History>(); //new LocalHistoryUtils().GetHistory();
                if (this.history == null)
                {
                    this.ForWard.IsEnabled = false;
                    this.ForWard.Icon.Visibility = Visibility.Hidden;
                }
                if (this.index == 0)
                {
                    this.BackWard.IsEnabled = false;
                    this.BackWard.Icon.Visibility = Visibility.Hidden;
                }
                else if (this.index == (this.history.Count - 1))
                {
                    this.ForWard.IsEnabled = false;
                    this.ForWard.Icon.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
            }
        }
         
        public void WebBrowser(int Id)
        {
            try
            {
                CacheHelper.SetCache(new CacheParameterName().HistoryIndex, string.Empty);
                CacheHelper.SetCache(this.cache.Id, Id.ToString());
                base.ParentWindow.PageSwitch(this.pageswitch.kdp);
            }
            catch
            {
            }
        }
    }
}

