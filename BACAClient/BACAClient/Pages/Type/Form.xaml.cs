namespace BACAClient.Pages.Type
{
    using BACAClient.Common;
    using BACAClient.Model;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    public partial class Form : Page
    {
        public Form()
        {
            this.InitializeComponent();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            char[] trimChars = new char[] { ';' };
            char[] separator = new char[] { ';' };
            string[] strArray = this.ImageList.TrimEnd(trimChars).Split(separator);
            if ((strArray != null) && (strArray.Length > 0))
            {
                string configer = ConfigerHelper.GetConfiger(new ConfigerParameterName().Resource);
                int num = 0;
                foreach (string str2 in strArray)
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        this.List.RowDefinitions.Add(new RowDefinition());
                        Image element = new Image {
                            Source = new BitmapImage(new Uri(string.Format(@"{0}\{1}", configer, str2)))
                        };
                        this.List.Children.Add(element);
                        Grid.SetRow(element, num);
                        num++;
                    }
                }
            }
        }
         
        public string ImageList { get; set; }
    }
}

