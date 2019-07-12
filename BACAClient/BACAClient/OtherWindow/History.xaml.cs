namespace BACAClient.OtherWindow
{
    using BACAClient.Common;
    using BACAClient.Model;
    using BACAClient.Template;
    using BACAClient.UserControls.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class History : Window
    {
        public History()
        {
            this.InitializeComponent();
        }

        private void _Click(string Name)
        {
            string text = this.Time.Text;
            if (!string.IsNullOrEmpty(text) && ConfigerHelper.SetAppSettingsConfig(new ConfigerParameterName().HistorySaveDays, text))
            {
                OpenWindows.Prompt("设定成功");
                base.Close();
            }
        }
        
        private void Close_Click(string Name)
        {
            base.Close();
        }
         
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9-]+").IsMatch(e.Text);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
    }
}

