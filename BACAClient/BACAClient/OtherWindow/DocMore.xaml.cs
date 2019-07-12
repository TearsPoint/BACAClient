namespace BACAClient.OtherWindow
{
    using BACAClient.Model;
    using BACAClient.Template;
    using BACAClient.Template.HisDetail;
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

    public partial class DocMore : Window
    {
        public event ChangedEventHandler gotokdp;

        public DocMore()
        {
            this.InitializeComponent();
        }
        
        private void Close_Click(string Name)
        {
            base.Close();
        }

        public void GetData()
        {
            //todo
        }

        private void goTokdp(int TypeId, string LngId)
        {
            base.Close();
            this.gotokdp(TypeId, int.Parse(LngId), string.Empty);
            base.Close();
        }
         
        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetData();
        }

        public string ReLngId { get; set; }

        public int ReTypeId { get; set; }

        public string StrName { get; set; }

        public delegate void ChangedEventHandler(int TypeId, int LngId, string parameter);
    }
}

