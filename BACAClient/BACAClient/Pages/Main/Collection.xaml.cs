namespace BACAClient.Pages.Main
{
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.Template;
    using BACAClient.UserControls.Controllers;
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
    using System.Windows.Media;

    public partial class Collection : BasePage
    {
        private MESSAGE message = new MESSAGE();
        public event ChangedEvent gotokdp;

        public Collection()
        {
            this.InitializeComponent();
        }
        
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.UserName.Text = this.model.UserName;
            this.UserID.Text = this.model.UserId; 
            this.Assignment();
        }

        public void Assignment()
        {
            this.List.Children.Clear();
            int recordCount = 0;
            List<KnowledgeAndRelease> list = new List<KnowledgeAndRelease>();//todo
            this.Number.Content = string.Format("收藏 {0} 条", recordCount);
            if (list == null)
            {
                this.Message(this.message.NoCollection);
            }
            else
            {
                int num2 = 0;
                foreach (KnowledgeAndRelease release in list)
                {
                    if (num2 > 0x63)
                    {
                        break;
                    }
                    BACAClient.Template.Collection element = new BACAClient.Template.Collection
                    {
                        UserId = this.model.UserId,
                        model = release
                    };
                    element.Refresh += new BACAClient.Template.Collection.Event(this.Assignment);
                    element.gotokdp += new BACAClient.Template.Collection.ChangedEvent(this.goTokdp);
                    this.List.Children.Add(element);
                    num2++;
                }
            }
        }

        private void DeleteAll_Click(string Name)
        {
            TypeInitialization initialization = new TypeInitialization();
            BACAClient.Model.Collection model = new BACAClient.Model.Collection
            {
                UserID = this.model.UserId,
                TypeID = initialization.INT,
                LngId = initialization.INT
            };
            MESSAGE message = new MESSAGE();
            OpenWindows.Prompt(message.RemoveSuccess);
        }

        public void goTokdp(KnowledgeAndRelease model)
        {
            GoodAndHot hot = new GoodAndHot
            {
                TypeID = model.TypeId,
                LngId = model.LngId,
                SourceId = model.SourceID,
                Name = model.Key1
            };
            this.gotokdp(hot);
        }
         
        public void Message(string info)
        {
            Label element = new Label
            {
                Content = info,
                Foreground = new BrushConverter().ConvertFromInvariantString("#cccccc") as Brush,
                FontSize = 20.0,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.List.Children.Add(element);
        }
         
        public InterfacesUsers model { get; set; }

        public delegate void ChangedEvent(GoodAndHot model);
    }
}

