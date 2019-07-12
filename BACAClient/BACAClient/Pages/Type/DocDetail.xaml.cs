namespace BACAClient.Pages.Type
{
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.Template.HisDetail;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Forms.Integration;
    using System.Windows.Markup;

    public partial class DocDetail : BasePage
    { 
        public int status = 0;
        private WebBrowser web_browser;
        public event ChangedEventHandler ChangNav;
        public event ChangedEvent gotokdp;

        public DocDetail()
        {
            this.InitializeComponent();
        }
        
        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double actualHeight = base.ActualHeight;
            this.pageleft.Height = actualHeight;
            this.pageright.Height = actualHeight;
            this.PageInfo();
        }

        private void A_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement elementFromPoint = (sender as HtmlDocument).GetElementFromPoint(e.ClientMousePosition);
            HtmlElementCollection elementsByTagName = this.web_browser.Document.GetElementById("detail").GetElementsByTagName("a");
            string name = string.Empty;
            foreach (HtmlElement element3 in elementsByTagName)
            {
                if (element3.OuterHtml == elementFromPoint.OuterHtml)
                {
                    name = element3.Name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                TypeInitialization initialization = new TypeInitialization();
                this.gotokdp(initialization.INT, initialization.INT, name);
            }
        }

        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.web_browser.ReadyState == WebBrowserReadyState.Complete)
            {
                this.web_browser.Document.Click += new HtmlElementEventHandler(this.A_Click);
                HtmlElement elementById = this.web_browser.Document.GetElementById("detail");
                if (elementById != null)
                {
                    double height = elementById.ClientRectangle.Height;
                    double num2 = System.Convert.ToDouble(this.web_browser.Document.GetElementById("MinHeight").InnerText);
                    if (height <= num2)
                    {
                        this.web_browser.ScrollBarsEnabled = false;
                    }
                }
            }
        }
         
        private void Next()
        {
            if (this.status != 1)
            {
                int pageIndex = this.PageIndex;
                this.PageIndex = pageIndex + 1;
                this.ChangNav(this.PageIndex, 1);
            }
        }

        public void PageInfo()
        {
            if (this.status != 1)
            {
                this.status = 1;
                this.pageleft.Visibility = Visibility.Visible;
                this.pageright.Visibility = Visibility.Visible;
                if (this.PageIndex == 1)
                {
                    this.pageleft.Visibility = Visibility.Hidden;
                }
                else if (this.PageIndex == this.Counts)
                {
                    this.pageright.Visibility = Visibility.Hidden;
                }
                string str = HtmlUtils.WriterHtml(this.data[this.PageIndex - 1].Content, base.ActualHeight);
                this.web_browser = new WebBrowser();
                this.web_browser.ScriptErrorsSuppressed = true;
                this.web_browser.AllowWebBrowserDrop = false;
                this.web_browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.DocumentCompleted);
                this.web_browser.DocumentText = str;
                this.XmlData.Child = this.web_browser;
                this.status = 0;
            }
        }

        private void Previous()
        {
            if (this.status != 1)
            {
                int pageIndex = this.PageIndex;
                this.PageIndex = pageIndex - 1;
                this.ChangNav(this.PageIndex, 1);
            }
        }
         
        public int Counts { get; set; }

        public List<Node> data { get; set; }

        public int PageIndex { get; set; }

        public delegate void ChangedEvent(int TypeId, int LngId, string parameter);

        public delegate void ChangedEventHandler(int i, int type);
    }
}

