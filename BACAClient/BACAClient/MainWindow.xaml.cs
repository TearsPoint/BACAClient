namespace BACAClient
{
    using BACAClient.Common;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.UserControls.Controllers;
    using Base;
    using Base.Ex;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class MainWindow : Window
    {
        private CacheParameterName cache = new CacheParameterName();

        public static readonly double DefaultHeight = 760;
        public static readonly double DefaultWidth = 1024;
        public static readonly double DefaultTopMenuHeight = 34;

        private BACAClient.Model.PageSwitch pageswitch = new BACAClient.Model.PageSwitch();
        public MainWindow()
        {
            this.InitializeComponent();

            this.Width = DefaultWidth;
            this.Height = DefaultHeight;

            foreach (MenuItem item in this.popMenu.Items)
            {
                item.Click += this.Menu_Click;
            }

            foreach (MenuItem item in this.h1popMenu.Items)
            {
                item.Click += this.Menu_Click;
            }
            foreach (MenuItem item in this.h2popMenu.Items)
            {
                item.Click += this.Menu_Click;
            }

            foreach (var item in _wpMenu.Children.CastAs<BACAClient.Template.TabControl>())
            {
                item.MouseEnter += Nav_MouseEnter;
            }
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Flash flash = new Flash();
                this.Reload();
                this.LayOut();
                new Flash { ShowInTaskbar = false, mainwindowhide = (Flash.MainWindowHide)Delegate.Combine(flash.mainwindowhide, new Flash.MainWindowHide(this.HideOrShow)) }.Show();
                //this.Version.Content = new BACAClient.Model.Acquiescence().Version + Assembly.GetExecutingAssembly().GetName().Version;
                this._contentLoaded = false;
                this.PageSwitch(this.pageswitch.Index);
            }
            catch
            {
            }
        }

        public void CloseWindows()
        {
            try
            {
                CloseUtils.CloseWindows(new About().Title + "|" + new BACAClient.OtherWindow.Announcement().Title + "|" + new DocMore().Title + "|" + new BACAClient.OtherWindow.Feedback().Title + "|" + new BACAClient.OtherWindow.History().Title);
            }
            catch
            {
            }
        }

        public void HideOrShow()
        {
            try
            {
                if (base.WindowState == WindowState.Minimized)
                {
                    base.WindowState = WindowState.Normal;
                }
                else
                {
                    base.WindowState = WindowState.Minimized;
                }
            }
            catch
            {
            }
        }

        private void Icon_Click(string Name)
        {
            try
            {
                this.CloseWindows();
                string str = Name.ToLower();
                if (!(str == "close"))
                {
                    if (str == "max")
                    {
                        goto Label_004E;
                    }
                    if (str == "min")
                    {
                        goto Label_0057;
                    }
                    if (str == "menubutton")
                    {
                        goto Label_0060;
                    }
                }
                else
                {
                    ApplicationInfo.CloseApplication();
                }
                return;
                Label_004E:
                this.LayOut();
                return;
                Label_0057:
                this.HideOrShow();
                return;
                Label_0060:
                if (this.Menu.IsOpen)
                {
                    this.Menu.IsOpen = false;
                }
                else
                {
                    this.Menu.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                OpenWindows.Prompt(ex.CreateErrorMsg());
            }
        }


        private void Nav_MouseEnter(object sender, MouseEventArgs e)
        {
            foreach (var item in _wpMenu.Children.CastAs<BACAClient.Template.TabControl>())
            {
                if (sender == item)
                {
                    foreach (var p in _wpMenu.Children.CastAs<Popup>())
                    {
                        if (p.Name.StartsWith(((System.Windows.FrameworkElement)item).Name))
                        {
                            p.IsOpen = true;
                            continue;
                        }
                        p.IsOpen = false;
                    }
                }
            }

        }


        private void Nav_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach (var item in _wpMenu.Children.CastAs<TabControl>())
            {
                if (sender == item)
                {
                    foreach (var p in _wpMenu.Children.CastAs<Popup>())
                    {
                        if (p.Name.StartsWith(item.Name))
                        {
                            p.IsOpen = false;
                            continue;
                        }
                    }
                }
            }
        }

        public void Initialization()
        {

        }

        public void LayOut()
        {
            try
            {
                if (base.Height == DefaultHeight)
                {
                    base.Top = 0.0;
                    base.Left = 0.0;
                    base.Height = SystemParameters.WorkArea.Height;
                    base.Width = SystemParameters.WorkArea.Width;
                }
                else
                {
                    base.Top = (SystemParameters.WorkArea.Height - DefaultHeight) / 2.0;
                    base.Left = (SystemParameters.WorkArea.Width - DefaultWidth) / 2.0;
                    base.Height = DefaultHeight;
                    base.Width = DefaultWidth;
                }
                this.Page.Width = base.Width;
                this.Page.Height = base.Height - DefaultTopMenuHeight;
            }
            catch
            {
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            string type = mi.Header.ToString();
            Menu menu = mi.Parent as Menu;
            string str2 = type;
            if (!(str2 == "显示悬浮框"))
            {
                if (str2 == "隐藏悬浮框")
                {
                    ShowEvent.ShowOrHideWindow(new Flash().Title);
                    this.Menu1.Header = "显示悬浮框";
                }
                else if (str2 == "关 于" || str2 == "版本信息")
                {
                    OpenWindows.About();
                }
                else
                {
                    this.RapidAndType(type);
                }
            }
            else
            {
                if (!ShowEvent.ShowOrHideWindow(new Flash().Title))
                {
                    new Flash().Show();
                }
                this.Menu1.Header = "隐藏悬浮框";
            }

            if (str2 == "切换账号")
                OpenWindows.Logon();
            if (str2 == "设置")
                OpenWindows.SettingProfile();

            this.Menu.IsOpen = false;
            this.h1pop.IsOpen = false;
            this.h2pop.IsOpen = false;
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                switch (e.ClickCount)
                {
                    case 1:
                        if (base.ActualHeight < SystemParameters.WorkArea.Height)
                        {
                            base.DragMove();
                        }
                        return;

                    case 2:
                        break;

                    default:
                        return;
                }
                this.CloseWindows();
                this.LayOut();
            }
            catch
            {
            }
        }

        private void Nav_Click(int TypeId, bool IsClick)
        {
            try
            {
                CacheHelper.RemoveAllCache();
                CacheHelper.SetCache(this.cache.TypeId, TypeId.ToString());
                this.PageSwitch(this.pageswitch.Index);
            }
            catch
            {
            }
        }

        public void OperationType()
        {
            //try
            //{
            //    ConfigerParameterName name = new ConfigerParameterName();
            //    DllInfo modelByXML = LocalDllUtils.GetModelByXML();
            //    if ((modelByXML == null) || (modelByXML.IsOpen == -1))
            //    {
            //        CacheHelper.SetDellInfo(new HISLocalAPIBusiness().Users(null));
            //        int num = 0;
            //        CacheHelper.SetCache(this.cache.IsReturn, num.ToString());
            //        this.RapidAndType("首页推荐");
            //    }
            //    else if (modelByXML.IsOpen == 0)
            //    {
            //        CacheHelper.SetDellInfo(new HISLocalAPIBusiness().Users(modelByXML));
            //        if ((modelByXML != null) && (modelByXML.IsOpen == 0))
            //        {
            //            ReturnSearch model = new ReturnSearch();
            //            model.IsReturn = 0.ToString();
            //            model.TypeId = modelByXML.TypeId;
            //            model.SourceId = -1;
            //            model.KeyWord = modelByXML.KeyWord;
            //            model.Isdell = modelByXML.IsOpen;
            //            CacheHelper.SetReturnSearch(model);
            //            switch (modelByXML.TypeId)
            //            {
            //                case 0:
            //                    this.PageSwitch(this.pageswitch.RapidRetrieval);
            //                    return;

            //                case 9:
            //                    if (!string.IsNullOrEmpty(modelByXML.KeyWord))
            //                    {
            //                        Knowledge knowledge = new HISSolrTableKnowledgeBusiness().GetModel(int.Parse(modelByXML.KeyWord));
            //                        if (knowledge.TypeId == 3)
            //                        {
            //                            new PDF { Id = knowledge.Id, TypeId = knowledge.TypeId }.Show();
            //                        }
            //                        else
            //                        {
            //                            CacheHelper.SetCache(this.cache.Id, modelByXML.KeyWord);
            //                            this.PageSwitch(this.pageswitch.kdp);
            //                        }
            //                    }
            //                    return;
            //            }
            //            this.PageSwitch(this.pageswitch.TypeRetrieve);
            //        }
            //    }
            //}
            //catch (Exception exception)
            //{
            //    BACAClient.Common.TemporaryFiles.Log.WriterLog(SystemEnum.LogType.Local, string.Empty, "打开错误:" + exception);
            //    Environment.Exit(0);
            //}
        }

        public void PageSwitch(string path)
        {
            System.Type type;
            if (!path.ToLower().Contains("BACAClient.pages.main"))
            {
                this.Initialization();
            }
            this.CloseWindows();
            string str = path.ToLower();
            if (!(str == "BACAClient.pages.type.kdp"))
            {
                if (str == "BACAClient.pages.type.typeretrieve")
                {
                    goto Label_0092;
                }
            }
            else
            {
                SystemEnum.ISRETURN yES = SystemEnum.ISRETURN.YES;
                if (CacheHelper.GetCache(this.cache.IsReturn) == yES.ToString())
                {
                    this.TwoNav2.Visibility = Visibility.Visible;
                }
                goto Label_0092;
            }
            this.TwoNav2.Visibility = Visibility.Hidden;
            Label_0092:
            type = System.Type.GetType(path);
            if (type != null)
            {
                System.Windows.Controls.Page page = type.Assembly.CreateInstance(path) as System.Windows.Controls.Page;
                this.Nested.Content = page;
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    if (info.Name.ToLower() == "parentwindow")
                    {
                        info.SetValue(page, this, null);
                        break;
                    }
                }
            }
        }

        public void RapidAndType(string Type)
        {
            this.Initialization();
            string uriString = "pack://application:,,,/BACAClient;component/Images/MainWindow/twonav/in_two_nav.png";
            string path = string.Empty;
            string str3 = Type;
            if (!(str3 == "首页推荐"))
            {
                if (str3 == "快速检索")
                {
                    ImageBrush brush2 = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(uriString))
                    };
                    CacheHelper.SetCache(this.cache.SourceId, (-1).ToString());
                    CacheHelper.SetCache(this.cache.keyword, string.Empty);
                    path = this.pageswitch.RapidRetrieval;
                }
                else if (str3 == "返回检索")
                {
                    path = CacheHelper.GetCache(this.cache.Pages);
                }
                else if (str3 == "浏览历史")
                {
                    path = this.pageswitch.History;
                }
                else if (str3.Contains("帮助"))
                {
                    path = this.pageswitch.Help;
                }
            }
            else
            {
                ImageBrush brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(uriString))
                };
                path = this.pageswitch.Index;
            }
            this.PageSwitch(path);
            this.NavIcon.Visibility = Visibility.Hidden;
        }

        public void Reload()
        {
            try
            {
                CacheHelper.RemoveAllCache();
                this.OperationType();
            }
            catch
            {
            }
        }

        private void ReturnSearch_Click(string type)
        {
            this.RapidAndType(type);
        }

        public void SetNavIcon(int TypeId)
        {
            this.NavIcon.Visibility = Visibility.Visible;
            int num = 0x3e + (0x5e * (TypeId - 1));
            this.NavIcon.Margin = new Thickness((double)num, 105.0, 0.0, 0.0);
        }

        private void TwoNav_Click(string type)
        {
            CacheHelper.RemoveAllCache();
            this.RapidAndType(type);
        }

        private void MAIN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = false;
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}
        }

    }
}

