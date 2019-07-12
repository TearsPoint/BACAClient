namespace BACAClient.Controllers
{
    using BACAClient.UserControls.Controls;
    using BACAClient.UserControls.Controls.FrontDesk;
    using System;

    public class OpenWindows
    {
        public static void Good()
        {
            new GoodSuccess().Show();
        }

        public static void Prompt(string data)
        {
            new BACAClient.UserControls.Controls.Prompt { Message = data, Topmost = true }.Show();
        }
    }
}

