namespace BACAClient.OtherWindow
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class Flash : Window
    {
        public MainWindowHide mainwindowhide;

        public Flash()
        {
            this.InitializeComponent();
            this.LayOut();
        }
         
        public void LayOut()
        {
            base.Top = 120.0;
            base.Left = SystemParameters.PrimaryScreenWidth - 170.0;
        }
         
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                base.DragMove();
            }
            else
            {
                this.mainwindowhide();
            }
        }

        public delegate void MainWindowHide();
    }
}

