namespace BACAClient.OtherWindow
{
    using BACAClient.Common;
    using BACAClient.Model;
    using BACAClient.Template;
    using BACAClient.UserControls.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    public partial class PDF : Window
    {
        private CacheParameterName cache = new CacheParameterName();  
        private string[] IMGPath = null;
        private int IMGs = 0;
        private string Knowledge = string.Empty;
        private BACAClient.Model.Knowledge model = null;
        private int page = 0; 
        private string PDFName = string.Empty;
        private string PDFPath = string.Empty;
        private Relational reModel = null; 
        private InterfacesUsers users = new InterfacesUsers(); 

        public PDF()
        {
            this.InitializeComponent();
            this.LayOut();
        }
         
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.users = CacheHelper.GetInterfacesUsersModel();
                this.OpenFullTextImage();
            }
            catch
            {
            }
        }

        public bool BindFullImages()
        {
            try
            {
                this.wait.Visibility = Visibility.Visible;
                this.Up.Visibility = Visibility.Hidden;
                this.Down.Visibility = Visibility.Hidden;
                this.scrollviewer.ScrollToHome();
                string str = this.IMGPath[this.page];
                if (string.IsNullOrEmpty(str))
                {
                    return true;
                }
                this.pdf.Source = new BitmapImage(new Uri(string.Format(@"{0}\{1}", ConfigerHelper.GetConfiger(new ConfigerParameterName().Resource), str)));
            }
            catch
            {
                this.wait.Content = "打开全文失败，请稍候重试。";
                return false;
            }
            return true;
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            ShowEvent.WindowsClose(new BACAClient.OtherWindow.Feedback().Title);
            BACAClient.Model.Feedback feedback = new BACAClient.Model.Feedback
            {
                TypeID = this.TypeId.ToString(),
                LngId = this.ReLngId,
                Knowledge = this.Knowledge,
                PageName = "全文浏览",
                PageIndex = this.page + 1
            };
            new BACAClient.OtherWindow.Feedback { model = feedback, Owner = this }.Show();
        }

        private void Icon_Click(string Name)
        {
            string str = Name.ToLower();
            if (!(str == "up"))
            {
                if (str == "download")
                {
                    string typename = string.Empty;
                    if (this.TypeId == 7)
                    {
                        typename = "引文文献";
                    }
                    else
                    {
                        typename = this.model.TypeName;
                    }
                    UserActionUtils.DownLoad(this.PDFPath, this.PDFName, typename, this.Knowledge);
                }
                else if (str == "close")
                {
                    base.Close();
                    ShowEvent.ShowOrHideWindow(new Flash().Title);
                }
                else if (str == "down")
                {
                    this.page++;
                    this.BindFullImages();
                }
            }
            else
            {
                this.page--;
                this.BindFullImages();
            }
        }
        
        public void LayOut()
        {
            try
            {
                this.wait.Visibility = Visibility.Visible;
                this.Up.Visibility = Visibility.Hidden;
                this.Down.Visibility = Visibility.Hidden;
                ShowEvent.ShowOrHideWindow(new Flash().Title);
            }
            catch
            {
            }
        }

        public void OpenFullTextImage()
        {
            try
            {
                if (this.TypeId == 7)
                {
                    this.reModel = new Relational(); //todo
                    if (this.reModel != null)
                    {
                        this.Knowledge = this.reModel.ReTitle;
                        this.IMGs = this.reModel.IMGs;
                        char[] trimChars = new char[] { ';' };
                        char[] separator = new char[] { ';' };
                        this.IMGPath = this.reModel.IMGPath.TrimEnd(trimChars).Split(separator);
                        this.PDFName = this.reModel.PDFName;
                        this.PDFPath = this.reModel.PDFPath;
                    }
                }
                else
                {
                    this.model = new Model.Knowledge(); // todo
                    if (this.model != null)
                    {
                        this.Knowledge = this.model.Key1;
                        this.IMGs = this.model.IMGs;
                        char[] chArray3 = new char[] { ';' };
                        char[] chArray4 = new char[] { ';' };
                        this.IMGPath = this.model.IMGPath.TrimEnd(chArray3).Split(chArray4);
                        this.PDFName = this.model.PDFName;
                        this.PDFPath = this.model.PDFPath;
                        this.ReLngId = this.model.LngId.ToString();
                    }
                }
                if ((this.IMGs == 0) || (this.IMGPath == null))
                {
                    this.wait.Content = "暂无全文";
                }
                else if (this.BindFullImages() && (this.TypeId != 7))
                {
                    this.Statistics();
                }
            }
            catch
            {
            }
        }

        private void pdf_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.wait.Visibility = Visibility.Visible;
            this.Up.Visibility = Visibility.Hidden;
            this.Down.Visibility = Visibility.Hidden;
            double num = base.ActualWidth - 200.0;
            double actualWidth = this.pdf.ActualWidth;
            if (actualWidth > num)
            {
                this.pdf.Width = num;
                double num3 = (actualWidth - num) / actualWidth;
                double actualHeight = this.pdf.ActualHeight;
                this.pdf.Height = actualHeight - (actualHeight * num3);
            }
            this.SetButton();
        }

        public void SetButton()
        {
            if (this.IMGs == 1)
            {
                this.Up.Visibility = Visibility.Hidden;
                this.Down.Visibility = Visibility.Hidden;
            }
            else if (this.page <= 0)
            {
                this.Up.Visibility = Visibility.Hidden;
                this.Down.Visibility = Visibility.Visible;
            }
            else if (this.page >= (this.IMGs - 1))
            {
                this.Up.Visibility = Visibility.Visible;
                this.Down.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Up.Visibility = Visibility.Visible;
                this.Down.Visibility = Visibility.Visible;
            }
            this.wait.Visibility = Visibility.Hidden;
        }

        public void Statistics()
        {
            UserActionUtils.WriteHotSpot(this.users.UserId, 1, this.model);
            //List<BACAClient.Model.History> history = new LocalHistoryUtils().GetHistory();
            //if (!new LocalHistoryUtils().IsExists(history, this.model))
            //{
            //    new LocalHistoryUtils().SetHistory(this.model);
            //}
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                base.Close();
            }
        }

        public string CreateTime { get; set; }

        public int Id { get; set; }

        public string ReLngId { get; set; }

        public int TypeId { get; set; }
    }
}

