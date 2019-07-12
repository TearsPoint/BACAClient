using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BACAClient.Common
{
    public class ApplicationInfo
    {
        public static void CloseApplication()
        {
            try
            {
                Environment.Exit(0);
                Application.Current.Shutdown();
            }
            catch
            {
            }
        }
    }

}
