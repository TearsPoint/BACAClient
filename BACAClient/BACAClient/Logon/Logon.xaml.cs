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

namespace BACAClient.Logon
{
    /// <summary>
    /// 登录
    /// </summary>

    public partial class Logon : Window
    {
        public Logon()
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
            this.Hide();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
