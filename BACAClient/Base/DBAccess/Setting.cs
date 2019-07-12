using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Reflection; 
using System.Runtime.Serialization;
using System.Data.Objects.DataClasses;

namespace JetSun.Core.DBAccess
{
    /// <summary>
    /// 数据库服务器设置信息
    /// </summary>
    [DataContract]
    public class DbsSetting
    {
        /// <summary>
        /// 表示空的实例。
        /// </summary>
        public static readonly DbsSetting Empty = new DbsSetting();
        private DbsSetting()
        {
            this.IsEmpty = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbs"></param>
        /// <param name="provider"></param>
        /// <param name="currentConnectionString"></param>
        public DbsSetting(Dbs dbs, DbsProvider provider, string currentConnectionString)
        {
            this.Dbs = dbs;
            this.CurrentConnectionString = currentConnectionString;
            this.Provider = provider;
            this.HistoryConnectionString = string.Empty;
            this.IsEmpty = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbs"></param>
        /// <param name="provider"></param>
        public DbsSetting(Dbs dbs, DbsProvider provider)
            : this(dbs, provider, string.Empty)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// 获取数据库服务器类型
        /// </summary>
        public Dbs Dbs { get; private set; }
        /// <summary>
        /// 获取/设置当前数据库服务器串
        /// </summary>
        public string CurrentConnectionString { get; set; }

        private DbsProvider _provider;
        /// <summary>
        /// 获取/设置数据库提供者。未设置时系统默认使用 <see cref="DbsProvider.MsSql"/>
        /// </summary>
        public DbsProvider Provider
        {
            get { return _provider ?? DbsProvider.MsSql; }
            set { _provider = value; }
        }
        /// <summary>
        /// 获取/设置历史数据库服务器串
        /// </summary>
        public string HistoryConnectionString { get; set; }


        /// <summary>
        /// 获取/设置元数据嵌入资源的程序集的完整名称。
        /// </summary>
        public string MetadataAssemblyFullName { get; set; }

        /// <summary>
        /// 返回适用于指定Dto类型的Edm连接串
        /// </summary>
        /// <typeparam name="TEdm"></typeparam>
        /// <param name="isHistory"></param>
        /// <returns></returns>
        public string ToEdmConnectionString<TEdm>(bool isHistory) where TEdm : IEntityWithKey
        {
            return ToEdmConnectionString<TEdm>(isHistory, ConnectionSettings.Empty);
        }
        /// <summary>
        /// 返回适用于指定Dto类型的Edm连接串
        /// </summary>
        /// <typeparam name="TEdm"></typeparam>
        /// <param name="isHistory"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public string ToEdmConnectionString<TEdm>(bool isHistory, ConnectionSettings settings) where TEdm : IEntityWithKey
        {
            return BuildConnectionString(typeof(TEdm), settings, isHistory);
        }

        /// <summary>
        /// 返回适用于指定Dto类型的Edm连接串
        /// </summary>
        /// <param name="dtoType"></param>
        /// <param name="isHistory"></param>
        /// <returns></returns>
        public string ToEdmConnectionString(Type dtoType, bool isHistory)
        {
            return ToEdmConnectionString(dtoType, ConnectionSettings.Empty, isHistory);
        }

        /// <summary>
        /// 返回适用于指定Dto类型的Edm连接串
        /// </summary>
        /// <param name="dtoType"></param>
        /// <param name="settings"></param>
        /// <param name="isHistory"></param>
        /// <returns></returns>
        public string ToEdmConnectionString(Type dtoType, ConnectionSettings settings, bool isHistory)
        {
            return BuildConnectionString(dtoType, settings, isHistory);
        }

        /// <summary>
        /// 创建数据库服务器
        /// </summary>
        /// <param name="isHistory"></param>
        /// <returns></returns>
        public DbConnection CreateConnection(bool isHistory)
        {
            string cnstr = isHistory ? HistoryConnectionString : CurrentConnectionString;

            return Provider.CreateConnect(cnstr);
        }
        /// <summary>
        /// 创建一个桥接器
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateAdapter()
        {
            return Provider.CreateAdapter();
        }

        private string BuildConnectionString(Type dtoType, ConnectionSettings settings, bool isHistory)
        {
            string strCon = isHistory ? HistoryConnectionString : CurrentConnectionString;

            if (string.IsNullOrEmpty(strCon))
                throw new KeyNotFoundException(string.Format("{0} 对应的 {1} 数据库服务器设置为空", Dbs, isHistory ? "历史" : "当前"));

            if (!settings.IsEmpty)
            {
                strCon = Provider.BuildConnectionString(strCon, settings);
            }

            return string.Format("metadata=res://{0}/; provider={1}; provider connection string=\"{2}\"", Provider.GetEdmMetadata(dtoType), Provider.Provider, strCon);
        }
    }
}
