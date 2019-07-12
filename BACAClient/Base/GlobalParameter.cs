using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    /// <summary>
    /// 全局参数集
    /// </summary>
    public class GlobalParameter
    {
        /// <summary>
        /// 系统配置文件名
        /// </summary>
        public const string AppConfigFileName = "BACAClient.exe.config";
        public const string JsenvConfigFileName = "BACAClient.exe.config";
        public const string WebConfigFileName = "web.config";
        /// <summary>
        /// 
        /// </summary>
        public const string DbsFileName = "dbs.config";
        /// <summary>
        /// 数据库文件夹名称
        /// </summary>
        public const string DataBaseFolderName = "DataBase";

        /// <summary>
        /// 日志文件名前缀
        /// </summary>
        public const string LogFilePrefixName = "Log";

        /// <summary>
        /// 资源文件夹名
        /// </summary>
        public const string ResoucesFolderName = "Resources";

        /// <summary>
        /// 数据文件名前缀
        /// </summary>
        public const string DataFilePrefixName = "Data";

        /// <summary>
        /// Chromium Embedded Framework  (Chromium 内核)
        /// </summary>
        public const string CEF_Directory = "CEF";
    }
}
