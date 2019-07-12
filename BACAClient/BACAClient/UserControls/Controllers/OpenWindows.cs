namespace BACAClient.UserControls.Controllers
{
    using BACAClient.Common;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.UserControls.Controls;
    using BACAClient.UserControls.Controls.FrontDesk;
    using System;

    public class OpenWindows
    {
        public static void About()
        {
            ShowEvent.WindowsClose(new BACAClient.OtherWindow.About().Title);
            new BACAClient.OtherWindow.About().ShowDialog();
        }

        public static void Logon()
        {
            ShowEvent.WindowsClose(new BACAClient.Logon.Logon().Title);
            new BACAClient.Logon.Logon().ShowDialog();
        }


        public static void SettingProfile()
        {
            ShowEvent.WindowsClose(new BACAClient.Logon.Profile().Title);
            new BACAClient.Logon.Profile().ShowDialog();
        }


        public static void Announcement(BACAClient.Model.Announcement model)
        {
            ShowEvent.WindowsClose(new BACAClient.OtherWindow.Announcement().Title);
            new BACAClient.OtherWindow.Announcement { model = model }.Show();
        }

        public static void Feedback(BACAClient.Model.Feedback model)
        {
            ShowEvent.WindowsClose(new BACAClient.OtherWindow.Feedback().Title);
            new BACAClient.OtherWindow.Feedback { model = model }.Show();
        }

        public static void Good()
        {
            new BACAClient.UserControls.Controls.FrontDesk.GoodSuccess().Show();
        }

        public static void Prompt(string data)
        {
            new BACAClient.UserControls.Controls.Prompt { Message = data, Topmost = true }.Show();
        }
    }
}

