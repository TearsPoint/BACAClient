using Base.Ex;
using Base.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;

namespace Base
{

    /// <summary>
    /// 应用程序设置
    /// </summary>
    public static class AppSetting
    {
        static AppSetting()
        {
            LoadAppSettings(Runtime.ConfigFilePath);
        }

        private static NameValueCollection _dicSettings = System.Configuration.ConfigurationManager.AppSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataServiceGroup"></param>
        /// <returns></returns>
        public static string GetDataServiceBaseUrl(DataServiceGroup dataServiceGroup)
        {
            try
            {
                if (_dicSettings != null && _dicSettings.Count > 0)
                {
                    return _dicSettings[dataServiceGroup.Key];
                }
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName()
                    + string.Format("获取模块{0}的数据服务基地址出错", dataServiceGroup.Key), ex);
            }
            return string.Empty;
        }

        private static string _AppName = "Ex";
        /// <summary>
        /// AppName
        /// </summary>
        public static string AppName
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _AppName = _dicSettings["AppName"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _AppName ?? " ";
            }
        }


        /// <summary>
        /// 是否已加载
        /// </summary>
        static bool _isLoadedAppSettings;
        public static void LoadAppSettings(string configPath)
        {
            try
            {
                if (_isLoadedAppSettings) return;
                if (!File.Exists(configPath)) return;

                XElement doc;
                using (TextReader tr = TextReader.Synchronized(new StreamReader(configPath)))
                {
                    doc = XElement.Load(tr);
                }

                if (doc == null) return;


                foreach (var item in doc.GetChildElements("appSettings"))
                {
                    _dicSettings[item.Attributes("key").FirstOrDefault().Value] = item.Attributes("value").FirstOrDefault().Value;
                }
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }

            _isLoadedAppSettings = true;
        }

        private static string _DataServiceBaseUrl = "http://localhost/";
        /// <summary>
        /// 数据服务基地址
        /// </summary>
        public static string DataServiceBaseUrl
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _DataServiceBaseUrl = _dicSettings["DataServiceBaseUrl"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _DataServiceBaseUrl ?? "";
            }
        }

        private static string _MDTDataServiceBaseUrl = "http://localhost/";
        /// <summary>
        /// MDT数据服务基地址
        /// </summary>
        public static string MDTDataServiceBaseUrl
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _MDTDataServiceBaseUrl = _dicSettings["MDTDataServiceBaseUrl"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _MDTDataServiceBaseUrl ?? "";
            }
        }

        private static string _CDSSDataServiceBaseUrl = "http://localhost/";
        /// <summary>
        /// CDSS数据服务基地址
        /// </summary>
        public static string CDSSDataServiceBaseUrl
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _CDSSDataServiceBaseUrl = _dicSettings["CDSSDataServiceBaseUrl"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _CDSSDataServiceBaseUrl ?? "";
            }
        }

        private static bool _isDebugHost = false;
        /// <summary>
        /// 返回当前的Wcf服务是否是调试状态
        /// </summary> 
        public static bool IsDebugHost
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _isDebugHost = _dicSettings["IsDebugHost"].Convert<bool>();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _isDebugHost;
            }
        }

        private static bool _isUseWCF = true;
        /// <summary>
        /// 返回是否使用WCF服务
        /// </summary> 
        public static bool IsUseWCF
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _isUseWCF = _dicSettings["IsUseWCF"].Convert<bool>();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _isUseWCF;
            }
        }

        private static bool _isUseLocalWebServer = true;
        /// <summary>
        /// 返回是否启用本地WebServer
        /// </summary> 
        public static bool IsUseLocalWebServer
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _isUseLocalWebServer = _dicSettings["IsUseLocalWebServer"].Convert<bool>();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _isUseLocalWebServer;
            }
        }

        private static int _localWebServerPort = 0;
        /// <summary>
        /// 本地WebServer端口
        /// </summary> 
        public static int LocalWebServerPort
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _localWebServerPort = _dicSettings["LocalWebServerPort"].Convert<int>();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _localWebServerPort;
            }
        }


        private static string _localWebServerVirtualPath = "/";
        /// <summary>
        /// 本地Web服务虚拟路径
        /// </summary>
        public static string LocalWebServerVirtualPath
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _localWebServerVirtualPath = _dicSettings["LocalWebServerVirtualPath"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _localWebServerVirtualPath ?? "/";
            }
        }


        private static string _PageHeaderName = "PageHeaderName";
        /// <summary>
        /// 本地Web服务虚拟路径
        /// </summary>
        public static string PageHeaderName
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _PageHeaderName = _dicSettings["PageHeaderName"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _PageHeaderName ?? "PageHeaderName";
            }
        }

        private static string _localWebServerPhysicalPath = "";
        /// <summary>
        /// 本地Web服务物理路径
        /// </summary>
        public static string LocalWebServerPhysicalPath
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _localWebServerPhysicalPath = _dicSettings["LocalWebServerPhysicalPath"];
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _localWebServerPhysicalPath ?? "";
            }
        }

        private static int _socketPort = 8050;
        /// <summary>
        /// Socket端口号
        /// </summary> 
        public static int SocketPort
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _socketPort = _dicSettings["SocketPort"].Convert<int>();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _socketPort;
            }
        }

        private static IList<string> _needPreLoadAssemblys;

        /// <summary>
        /// 
        /// </summary>
        public static IList<string> NeedPreLoadAssemblys
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_dicSettings["NeedPreLoadAssemblys"]))
                        _needPreLoadAssemblys = _dicSettings["NeedPreLoadAssemblys"].Split(',', '，').ToList();
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _needPreLoadAssemblys;
            }
            set { _needPreLoadAssemblys = value; }
        }


        public static bool _isEnableNodejs = false;
        public static bool IsEnableNodejs
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _isEnableNodejs = _dicSettings["IsEnableNodejs"].Convert<bool>();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _isEnableNodejs;
            }
        }

        private static string _nodejs_startup_path = "";
        /// <summary>
        /// NodeJS 
        /// </summary>
        public static string NodejsStartupFilePath
        {
            get
            {
                try
                {
                    if (_dicSettings != null && _dicSettings.Count > 0)
                    {
                        _nodejs_startup_path = _dicSettings["NodejsStartupFilePath"];
                    }
                    if (!Directory.Exists(_nodejs_startup_path))
                    {
                        _nodejs_startup_path = Runtime.ExecutablePath.GetParentDirectoryPath();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                return _nodejs_startup_path ?? "";
            }
        }

    }
}