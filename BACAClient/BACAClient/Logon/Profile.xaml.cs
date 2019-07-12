using BACAClient.Common;
using BACAClient.UserControls.Controllers;
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
    /// 资料填写
    /// </summary>

    public partial class Profile : Window
    {
        public Profile()
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
        }

        private void _lnkSwitchUser_Click(object sender, RoutedEventArgs e)
        {
            OpenWindows.Logon();
        }

        private void OK_Click(string Name)
        {

            this.Close();
        }

        private void _lnkUpdateHeader_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
