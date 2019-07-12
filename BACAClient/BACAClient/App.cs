namespace BACAClient
{
    using BACAClient.Repository;
    using Base;
    using Base.Repository;
    using fastJSON;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    public partial class App : Application
    {
        private MainWindow WinObj;

        public void activate()
        {
            try
            {
                this.WinObj.WindowState = WindowState.Normal;
                this.WinObj.Activate();
                //this.WinObj.Reload();
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName == "BACAClient")
                    {
                        process.Kill();
                        return;
                    }
                }
                base.OnExit(e);
            }
            catch
            {
            }
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
        }

        
        public void Initialize()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/BACAClient;component/app.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        //[STAThread, DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //public static void Main()
        //{
        //    App app = new App();
        //    app.InitializeComponent();
        //    app.Run();
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                this.WinObj = new MainWindow();
                this.WinObj.Show();
            }
            catch
            {
            }
        }
    }
}

