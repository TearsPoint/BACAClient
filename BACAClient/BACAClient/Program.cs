namespace BACAClient
{
    using BACAClient.Common;
    using BACAClient.Model;
    using System;
    using System.Management;
    using System.Threading;
    using System.Windows;

    internal class Program
    {
        public static Mutex Run;

        public static void ApplicationStart()
        {
            try
            {
                bool createdNew = false;
                App app = new App();
                Run = new Mutex(true, "MainWindow", out createdNew);
                if (createdNew)
                {
                    app.activate();
                }
                else
                {
                    app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
            }
            catch (Exception)
            {
            }
        }

        [STAThread]
        private static void Main(string[] args)
        { 
            try
            {
                DllInfo modelByProcessArguments = LocalDllUtils.GetModelByProcessArguments();
                ConfigerParameterName name = new ConfigerParameterName();
                CacheParameterName name2 = new CacheParameterName();
                if (modelByProcessArguments != null)
                {
                    if (modelByProcessArguments.IsOpen == 1)
                    {
                        //todo 
                    }
                } 
                new single().Run(args);
            }
            catch (Exception)
            {
            }
        }
    }
}

