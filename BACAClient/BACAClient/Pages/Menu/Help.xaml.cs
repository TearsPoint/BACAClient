namespace BACAClient.Pages.Menu
{
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.OtherWindow;
    using BACAClient.Template;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;

    public partial class Help : BasePage
    {
        public Help()
        {
            this.InitializeComponent();
        }
        
        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            Help1 help = new Help1();
            this.Right.Content = help;
        }

        private void Button_Click(string Name)
        {
            this.Initialization(Name);
            string str = Name;
            if ((str == "help5") && !ShowEvent.Reopen("History"))
            {
                new BACAClient.OtherWindow.History().Show();
            }
        }

        public void Initialization(string ClickName)
        {
            foreach (object obj2 in this.Button.Children)
            {
                if (obj2 is BACAClient.Template.Button)
                {
                    BACAClient.Template.Button button = (BACAClient.Template.Button) obj2;
                    if (button.Name == ClickName)
                    {
                        button.BackGround = "#3399cc";
                        button.Foreground = Brushes.White;
                    }
                    else
                    {
                        button.BackGround = "#FFFFFF";
                        button.Foreground = new BrushConverter().ConvertFromInvariantString("#716B6B") as Brush;
                    }
                    button.Hover = "#cccccc";
                }
            }
        }
        
    }
}

