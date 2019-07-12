namespace BACAClient.Pages.Menu
{
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.Template;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public partial class History : BasePage
    {
        private List<BACAClient.Model.History> data = null;

        public History()
        {
            this.InitializeComponent();
        }
        
        public void BoundControls()
        {
            this.List.Children.Clear();
            int num = 0;
            foreach (BACAClient.Model.History history in this.data)
            {
                BACAClient.Template.History element = new BACAClient.Template.History {
                    width = base.ActualWidth - 80.0,
                    model = history,
                    Index = num
                };
                element.gotokdp += new BACAClient.Template.History.ChangedEvent(this.gotokdp);
                this.List.Children.Add(element);
                num++;
            }
        }

        private void EmptyHistory_Click(string Name)
        {
            //if (new LocalUtils<BACAClient.Model.History>().XMLEmpty(0, new LocalXmlName().History))
            //{
            //    this.Number.Content = "0";
            //    this.List.Children.Clear();
            //    this.Message(new MESSAGE().HistoryIsEmpty);
            //}
        }

        public void GetData()
        {
            //todo
        }

        private void gotokdp(BACAClient.Model.History model, int index)
        {
            try
            {
                if (model == null)
                {
                    this.GetData();
                }
                else if (model.TypeId == 3)
                {
                    new PDF { Id = model.Id, TypeId = model.TypeId }.Show();
                }
                else
                {
                    CacheParameterName name = new CacheParameterName();
                    CacheHelper.SetCache(name.HistoryIndex, index.ToString());
                    CacheHelper.SetCache(name.Id, model.Id.ToString());
                    base.ParentWindow.PageSwitch(new PageSwitch().kdp);
                }
            }
            catch
            {
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetData();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (UIElement element in this.List.Children)
            {
                if (element is BACAClient.Template.History)
                {
                    BACAClient.Template.History history = (BACAClient.Template.History) element;
                    history.Width = base.ActualWidth - 80.0;
                }
            }
        }
        
        public void Message(string info)
        {
            this.IsNull.Visibility = Visibility.Visible;
            this.NotNull.Visibility = Visibility.Hidden;
            this.NullData.Text = info;
        }
        
        private void TimeSort_Click(string Sort)
        {
            string str = Sort.ToLower();
            //if (!(str == "desc"))
            //{
            //    if (str == "asc")
            //    {
            //        this.data = new SortUtils().HistorySort(SystemEnum.Sort.ASC, this.data);
            //    }
            //}
            //else
            //{
            //    this.data = new SortUtils().HistorySort(SystemEnum.Sort.DESC, this.data);
            //}
            this.BoundControls();
        }
    }
}

