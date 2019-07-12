namespace BACAClient
{
    using Microsoft.VisualBasic.ApplicationServices;
    using System;

    public class single : WindowsFormsApplicationBase
    {
        private App a;

        public single()
        {
            base.IsSingleInstance = true;
        }

        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            this.a = new App();
            this.a.InitializeComponent();
            this.a.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            this.a.activate();
        }
    }
}

