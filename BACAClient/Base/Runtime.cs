using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base.RegeditOperate;
using System.IO;
using System.Reflection;
using Base.IO;
using System.Security.Policy;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using Microsoft.Win32;
using Base.DBAccess;

namespace Base
{
    /// <summary>
    /// 运行时
    /// </summary>
    public static class Runtime
    {
        /// <summary>
        /// 
        /// </summary>
        public static IDictionary<string, DBConnection> Dbs { get; set; }


        static IDictionary<string, Type> _loaders;
        static Runtime()
        {
            _loaders = new Dictionary<string, Type>();
        }

        private static StackTrace _stackTrace;
        public static StackTrace StackTrace
        {
            get
            {
                if (_stackTrace == null)
                    _stackTrace = new StackTrace(true);
                return _stackTrace;
            }
        }

        /// <summary>
        /// 当前调用的方法信息
        /// </summary>
        public static MethodBase CurrentInvokeMethodInfo
        {
            get
            {
                return StackTrace.GetFrame(1).GetMethod();
            }
        }

        /// <summary>
        /// 当前调用的方法名
        /// </summary>
        public static string CurrentInvokeMethodName
        {
            get
            {
                MethodBase mb = StackTrace.GetFrame(1).GetMethod();
                StringBuilder paras = new StringBuilder();

                foreach (var item in mb.GetParameters())
                {
                    paras.Append(item.ParameterType.Name);
                    paras.Append(" ");
                    paras.Append(item.Name);
                    paras.Append(",");
                }
                return string.Format(@"{0}({1})", mb.Name, paras.Length == 0 ? "" : paras.ToString().Substring(0, paras.Length - 1));
            }
        }

        static string _jetSun30ExecutablePath;
        /// <summary>
        /// 
        /// </summary>
        public static string JetSun30ExecutablePath
        {
            get
            {
                if (string.IsNullOrEmpty(_jetSun30ExecutablePath))
                    _jetSun30ExecutablePath = RegistryOperator.GetValue<string>(Path.Combine(RegistryOperator.RegisterRoot, TearsPointRegistryItemEnum.ExecutablePath.ToString()), "");
                return _jetSun30ExecutablePath;
            }
            set { _jetSun30ExecutablePath = value; }
        }

        static string _executablePath;
        /// <summary>
        /// 可执行路径 (如果注册表未设置ExecutablePath，则取Core.dll所在路径）
        /// </summary>
        public static string ExecutablePath
        {
            get
            {
                _executablePath = RegistryOperator.GetValue<string>(Path.Combine(RegistryOperator.RegisterRoot, TearsPointRegistryItemEnum.ExecutablePath.ToString()), "");
                if (string.IsNullOrEmpty(_executablePath) || HttpContext.Current != null)
                {
                    if (HttpContext.Current != null)
                    {
                        _executablePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "bin");

                        try
                        {
                            Loger.WriteEntry("Runtime", string.Format("注册表未设置ExecutablePath,取网站根目录所在路径:{0}", _executablePath), EventLogEntryType.Warning);
                        }
                        catch
                        { }
                    }
                    else
                    {
                        _executablePath = typeof(Runtime).Assembly.Location.Remove(typeof(Runtime).Assembly.Location.LastIndexOf("\\"));
                        _configFilePath = Path.Combine(_executablePath, GlobalParameter.AppConfigFileName);
                        if (!File.Exists(_configFilePath))
                            _configFilePath = Path.Combine(_executablePath, GlobalParameter.WebConfigFileName);
                        if (!File.Exists(_configFilePath))
                        {
                            _executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                        }
                        try
                        {
                            //Loger.WriteEntry("Runtime", "注册表未设置ExecutablePath,当前取Core.dll所在路径：" + _executablePath, EventLogEntryType.Error);
                        }
                        catch { }
                    }
                }
                return _executablePath;
            }
        }

        public static string ExecutablePathParent
        {
            get
            {
                return Directory.GetParent(Runtime.ExecutablePath).FullName;
            }
        }

        /// <summary>
        /// 取得父目录路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetParentDirectoryPath(this string path)
        {
            try
            {
                return Directory.GetParent(path).FullName;
            }
            catch
            {
            }
            return string.Empty;
        }

        /// <summary>
        /// 日志记录的路径
        /// </summary>
        public static string LogPath
        {
            get
            {
                return string.Format(@"{0}\{1}\", Runtime.ExecutablePathParent, GlobalParameter.LogFilePrefixName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DataPath
        {
            get { return Path.Combine(Runtime.ExecutablePathParent, GlobalParameter.DataFilePrefixName); }
        }

        /// <summary>
        /// Chromium Embedded Framework  (Chromium 内核)
        /// </summary>
        public static string CEF_Donwload_Path
        {
            get { return Path.Combine(Runtime.ExecutablePath, GlobalParameter.CEF_Directory); }
        }
        /// <summary>
        /// Nodejs运行时路径
        /// </summary>
        public static string Nodejs_Path
        {
            get { return Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ChromiumEF\"), "nodejs\\" + "node-v6.11.0-win-x64"); }
        }

        static string _configFilePath;
        /// <summary>
        /// 配置文件路径 
        /// </summary>
        public static string ConfigFilePath
        {
            get
            {
                _configFilePath = Path.Combine(ExecutablePath, GlobalParameter.AppConfigFileName);
                if (!File.Exists(_configFilePath))
                    if (HttpContext.Current != null)
                        _configFilePath = Path.Combine(ExecutablePath, GlobalParameter.WebConfigFileName);
                    else
                        _configFilePath = Path.Combine(ExecutablePath, GlobalParameter.JsenvConfigFileName);
                
                if (!File.Exists(_configFilePath))
                {
                    try
                    {
                        Loger.WriteEntry(Assembly.GetExecutingAssembly().FullName, "找不到系统配置文件:" + _configFilePath, EventLogEntryType.Warning);
                    }
                    catch
                    {
                    }
                }
                return _configFilePath;
            }
        }

        static string _dbsFilePath;
        /// <summary>
        /// 配置文件路径 
        /// </summary>
        public static string DbsFilePath
        {
            get
            {
                _dbsFilePath = Path.Combine(Path.Combine(Directory.GetParent(JetSun30ExecutablePath).FullName, "Config"), GlobalParameter.DbsFileName);
                if (!File.Exists(_dbsFilePath))
                {
                    try
                    {
                        Loger.WriteEntry(Assembly.GetExecutingAssembly().FullName, "找不到系统dbs.config配置文件:" + _dbsFilePath, EventLogEntryType.Error);
                    }
                    catch
                    {
                    }
                }
                return _dbsFilePath;
            }
        }
        /// <summary>
        /// 系统资源文件夹路径
        /// </summary>
        public static string ResoucesFolderPath
        {
            get
            {
                return Path.Combine(ExecutablePathParent, GlobalParameter.ResoucesFolderName);
            }
        }

        static Assembly[] _executeableAssemblys;
        /// <summary>
        /// 取得系统的可执行程序集列表
        /// </summary>
        public static Assembly[] ExecuteableAssemblys
        {
            get
            {
                LoadPreLoadAssemblys();
                return AppDomain.CurrentDomain.GetAssemblies();
            }
            set { _executeableAssemblys = value; }
        }

        static bool _isLoaded;
        /// <summary>
        /// 加载需要加载的程序集
        /// </summary>
        /// <returns></returns>
        public static void LoadPreLoadAssemblys()
        {
            if (_isLoaded) return;
            if (AppSetting.NeedPreLoadAssemblys != null)
                foreach (var assemblyName in AppSetting.NeedPreLoadAssemblys)
                {
                    if (File.Exists(Path.Combine(ExecutablePath, assemblyName)))
                        Assembly.LoadFrom(Path.Combine(ExecutablePath, assemblyName));
                    Thread.Sleep(3000);
                }
            _isLoaded = true;
        }

        /// <summary>
        /// 计算文件大小
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public static string CalFileSize(double fileSize)
        {
            if (fileSize < 1048576)
                return string.Format("{0:0.#}{1}", (fileSize / 1024), "KB");
            else if (fileSize >= 1048576 && fileSize < 1073741924)
                return string.Format("{0:0.#}{1}", (fileSize / 1048576), "MB");
            return "0KB";
        }

        public static string GetFileName(this Assembly assembly)
        {
            if (assembly == null) return "";
            return assembly.CodeBase.Substring(assembly.CodeBase.LastIndexOf('/') + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Intance<T>(this object type)
            where T : class
        {
            return type.Intance() as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Intance(this object type)
        {
            if (type is Type)
            {
                Type t = ((Type)type);
                if (t.IsInterface)   //如果接口，找到接口对应该的实现类
                {
                    List<Type> implementTypes = new List<Type>();
                    foreach (var item in ExecuteableAssemblys)
                    {
                        var itemps = item.GetTypes().Where(a => a != null && a.IsClass && a.GetInterface(t.FullName) != null);
                        if (itemps.Count() > 0)
                            implementTypes.AddRange(itemps.ToList());
                    }
                    if (implementTypes.Count() == 0)
                        throw new Exception(string.Format("没有找到接口:{0}的实现类", t.FullName));
                    type = implementTypes.First();
                }
                return Activator.CreateInstance(type as Type);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Intance(this object type, params object[] args)
        {
            if (type is Type)
            {
                return Activator.CreateInstance(type as Type, args);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type FindTypeInCurrentDomain(string typeName)
        {
            Type t;
            if (_loaders.TryGetValue(typeName, out t))
            {
                return t;
            }
            else
            {
                t = FindTypeByName(typeName);
                _loaders[typeName] = t;
            }
            return t;
        }

        private static Type FindTypeByName(string typeName)
        {
            Type type = null;

            //如果该类型已经装载
            type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            //在EntryAssembly中查找
            if (Assembly.GetEntryAssembly() != null)
            {
                type = Assembly.GetEntryAssembly().GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            //在CurrentDomain的所有Assembly中查找
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            for (int i = 0; (i < assemblyArrayLength); ++i)
            {
                Type[] typeArray = assemblyArray[i].GetTypes();
                int typeArrayLength = typeArray.Length;
                for (int j = 0; j < typeArrayLength; ++j)
                {
                    if (typeArray[j].Name.Equals(typeName))
                    {
                        return typeArray[j];
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// 启动默认浏览器,并指定访问的url
        /// </summary>
        /// <param name="uri"></param>
        public static void StartDefaultBrowser(string uri)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            string defaultBrowserPath = key.GetValue("").ToString();
            string app = defaultBrowserPath.Split(new string[] { " -" }, StringSplitOptions.RemoveEmptyEntries)[0];
            System.Diagnostics.Process.Start(app, uri);
        }

    }
}
