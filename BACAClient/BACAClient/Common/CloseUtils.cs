namespace BACAClient.Common
{ 
    using System;
    using System.Windows;

    public class CloseUtils
    {
        public static void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        public static void CloseWindows(string data)
        {
            char[] separator = new char[] { '|' };
            foreach (string str in data.Split(separator))
            {
                ShowEvent.WindowsClose(str);
            }
        }
    }
}

