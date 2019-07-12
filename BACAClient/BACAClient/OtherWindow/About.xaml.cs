using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BACAClient.OtherWindow
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>

    public partial class About : Window
    {
        public About()
        {
            this.InitializeComponent();
        }
        
        private void _MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                base.DragMove();
            }
            catch
            {
            }
        }

        private void Close_Click(string Type)
        {
            base.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Version.Text = "主程序版本：" + Assembly.GetExecutingAssembly().GetName().Version;
        }
         
    }
}
