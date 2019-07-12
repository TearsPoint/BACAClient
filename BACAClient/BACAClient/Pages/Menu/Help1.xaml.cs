namespace BACAClient.Pages.Menu
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public partial class Help1 : Page
    { 
        public Help1()
        {
            this.InitializeComponent();
        }
         
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (object obj2 in this.help.Children)
            {
                if (obj2 is StackPanel)
                {
                    StackPanel panel = (StackPanel) obj2;
                    foreach (object obj3 in panel.Children)
                    {
                        if (obj3 is TextBlock)
                        {
                            TextBlock block = (TextBlock) obj3;
                            block.Width = base.ActualWidth - 40.0;
                        }
                    }
                }
            }
        }
         
    }
}

