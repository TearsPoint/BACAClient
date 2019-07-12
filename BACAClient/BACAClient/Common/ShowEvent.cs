using System;
using System.Windows;
namespace BACAClient.Common
{
    public class ShowEvent
    {
        public static void PagesOpen(string title)
        {
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                if (Application.Current.Windows[i].Title == title)
                {
                    Application.Current.Windows[i].Show();
                    break;
                }
            }
        }

        public static bool Reopen(string title)
        {
            int num = 0;
            bool flag = false;
            while (num < Application.Current.Windows.Count)
            {
                if ((Application.Current.Windows[num].Title == title) && Application.Current.Windows[num].IsVisible)
                {
                    flag = true;
                    Application.Current.Windows[num].Hide();
                    Application.Current.Windows[num].Show();
                    return flag;
                }
                num++;
            }
            return flag;
        }

        internal static void WindowsClose(object title)
        {
            throw new NotImplementedException();
        }

        public static bool ShowOrHideMain(string title, bool IsOpen)
        {
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                if (Application.Current.Windows[i].Title == title)
                {
                    if (IsOpen)
                    {
                        Application.Current.Windows[i].Hide();
                        IsOpen = false;
                        return IsOpen;
                    }
                    IsOpen = true;
                    Application.Current.Windows[i].Show();
                    return IsOpen;
                }
            }
            return IsOpen;
        }

        public static bool ShowOrHidePages(string title)
        {
            int num = 0;
            bool flag = false;
            while (num < Application.Current.Windows.Count)
            {
                if (Application.Current.Windows[num].Title == title)
                {
                    flag = true;
                    if (Application.Current.Windows[num].IsVisible)
                    {
                        Application.Current.Windows[num].Hide();
                        return flag;
                    }
                    Application.Current.Windows[num].Show();
                    return flag;
                }
                num++;
            }
            return flag;
        }

        public static bool ShowOrHideWindow(string title)
        {
            int num = 0;
            bool flag = false;
            while (num < Application.Current.Windows.Count)
            {
                if (Application.Current.Windows[num].Title == title)
                {
                    flag = true;
                    if (Application.Current.Windows[num].IsVisible)
                    {
                        Application.Current.Windows[num].Hide();
                        return flag;
                    }
                    Application.Current.Windows[num].Show();
                    return flag;
                }
                num++;
            }
            return flag;
        }

        public static void WindowHide(string title)
        {
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                if (Application.Current.Windows[i].Title == title)
                {
                    if (Application.Current.Windows[i].IsVisible)
                    {
                        Application.Current.Windows[i].Hide();
                    }
                    break;
                }
            }
        }

        public static bool WindowIsOpen(string title)
        {
            int num = 0;
            while (num < Application.Current.Windows.Count)
            {
                if ((Application.Current.Windows[num].Title == title) && Application.Current.Windows[num].IsVisible)
                {
                    return true;
                }
                num++;
            }
            return false;
        }

        public static void WindowsClose(string title)
        {
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                if ((Application.Current.Windows[i].Title == title) && Application.Current.Windows[i].IsVisible)
                {
                    Application.Current.Windows[i].Close();
                    break;
                }
            }
        }
    }

}

