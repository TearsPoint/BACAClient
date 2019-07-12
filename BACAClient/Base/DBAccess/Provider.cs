using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq.Expressions;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using Base;
using Base.Ex;

namespace JetSun.Core.DBAccess
{
    /// <summary>
    /// 表示数据库服务提供者。
    /// </summary>
    public abstract class DbsProvider
    {
        static readonly Regex ConnectionStringValidKeyRegex;
        static readonly Regex ConnectionStringValidValueRegex;
        static readonly Regex ConnectionStringQuoteValueRegex;
        static readonly Regex ConnectionStringQuoteOdbcValueRegex;
        static DbsProvider()
        {
            ConnectionStringValidKeyRegex = new Regex(@"^(?![;\s])[^\p{Cc}]+(?<!\s)$", RegexOptions.Compiled);
            ConnectionStringValidValueRegex = new Regex("^[^\0]*$", RegexOptions.Compiled);
            ConnectionStringQuoteValueRegex = new Regex("^[^\"'=;\\s\\p{Cc}]*$", RegexOptions.Compiled);
            ConnectionStringQuoteOdbcValueRegex = new Regex("^\\{([^\\}\0]|\\}\\})*\\}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        }

        private byte _code;
        static Dictionary<byte, DbsProvider> _providers = new Dictionary<byte, DbsProvider>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        protected DbsProvider(byte code)
        {
            _code = code;
            _providers[code] = this;
        }

        /// <summary>
        /// 返回提供者的唯一的编码。
        /// </summary>
        public byte Code
        {
            get { return _code; }
        }

        /// <summary>
        /// 返回提供这的描述
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// 获取参数名称
        /// </summary>
        protected abstract string GetParameterName(string paraName);

        /// <summary>
        /// 返回ReportService数据源的扩展描述。
        /// </summary>
        public abstract string RsdsExtension { get; }

        /// <summary>
        /// 返回提供者的类型
        /// </summary>
        public abstract string Provider { get; }

        /// <summary>
        /// 返回对应Dto类型的元数据
        /// </summary>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        public string GetEdmMetadata(Type dtoType)
        {
            return OnGetEdmMetadata(dtoType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        protected abstract string OnGetEdmMetadata(Type dtoType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        protected string GetDbName(Type dtoType)
        {
            return GetDbName(dtoType.Namespace);
        }
        /// <summary>
        /// 从数据模型命名空间或资源文件路径中获取数据库名称。
        /// </summary>
        /// <param name="namespaceOrResPath"></param>
        /// <returns></returns>
        public static string GetDbName(string namespaceOrResPath)
        {
            string dbName = namespaceOrResPath;
            int ix = dbName.IndexOf("DataModel");
            if (ix == -1) return null;

            dbName = dbName.Substring(ix + 9).Trim('.', '\\');
            ix = Math.Max(dbName.IndexOf("."), dbName.IndexOf('\\'));
            if (ix > 0)
                dbName = dbName.Left(ix);

            return dbName;
        }

        /// <summary>
        /// 返回是否集成身份认证。
        /// </summary>
        public virtual bool IntegratedSecurity
        {
            get { return false; }
        }

        /// <summary>
        /// 返回连接串主要参数的名称及描述
        /// </summary>
        public abstract IEnumerable<DbsConnectionParameter> GetMainParameters();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public abstract string BuildConnectionString(string connectionString, ConnectionSettings settings);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(this.Provider);
            return factory.CreateConnectionStringBuilder();
        }

        /// <summary>
        /// 创建一个连接。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DbConnection CreateConnect(string connectionString)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(this.Provider);
            DbConnection cn = factory.CreateConnection();
            cn.ConnectionString = connectionString;

            return cn;
        }

        /// <summary>
        /// 创建一个桥接器
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateAdapter()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(this.Provider);
            return factory.CreateDataAdapter();
        }

        /// <summary>
        /// 校验指定连接串格式是否正确。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public abstract bool Validate(string connectionString);
        /// <summary>
        /// 提取存储过程命令的参数。
        /// </summary>
        /// <param name="cmd"></param>
        public void DeriveParameters(DbCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (cmd.CommandType != CommandType.StoredProcedure)
            {
                throw new InvalidOperationException(string.Format("DeriveParameters 不支持命令类型 {0}", cmd.CommandType));
            }

            DbConnection connection = cmd.Connection;
            if ((connection == null) || (connection.State != ConnectionState.Open))
            {
                throw new InvalidOperationException("连接未打开。");
            }
            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                throw new InvalidOperationException("CommandText不能为空。");
            }

            OnDeriveParameters(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        protected virtual void OnDeriveParameters(DbCommand cmd)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(this.Provider);
            DbCommandBuilder cb = factory.CreateCommandBuilder();
            MethodInfo method = cb.GetType().GetMethod("DeriveParameters", BindingFlags.Static | BindingFlags.Public);

            method.Invoke(null, new object[] { cmd });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        protected virtual string FormatStoredProcedureParameter(string parameters)
        {
            return parameters;
        }

        /// <summary>
        /// 返回系统支持的 <see cref="DbsProvider"/> 的枚举数。
        /// </summary>
        public static IEnumerable<DbsProvider> Providers
        {
            get { return _providers.Values; }
        }

        /// <summary>
        /// 获取与指定代码相同的<see cref="DbsProvider"/>实例。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool TryGetProvider(byte code, out DbsProvider provider)
        {
            return _providers.TryGetValue(code, out provider);
        }

        /// <summary>
        /// 表示 Microsoft SQL Server 提供者。
        /// </summary>
        public static readonly DbsProvider MsSql = new MsSqlDbsProvider();
        /// <summary>
        /// 表示 Microsoft SQL Server 集成 Windows 身份验证的提供者。
        /// </summary>
        public static readonly DbsProvider MsSqlNtAuthorize = new NtMsSqlDbsProvider();
        /// <summary>
        /// 
        /// </summary>
        public static readonly DbsProvider Oracle = new OracleDbsProvider();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="isIdentity"></param>
        /// <returns></returns>
        protected abstract string GetEnsureExistsCommand(string schemaName, string tableName, string columnName, bool isIdentity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        protected static string GetValue(DbConnectionStringBuilder builder, string keyword)
        {
            object v;
            if (builder.TryGetValue(keyword, out v))
                return v.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cn"></param>
        /// <returns></returns>
        protected virtual DbCommand CreateCommand(DbConnection cn)
        {
            return cn.CreateCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract string QuoteStoreName(object name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual object ConvertValue(object value)
        {
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string FormatSqlBlock(string sql)
        {
            return string.Format("{0};", sql.Trim(';'));
        }
    }

    /// <summary>
    /// 数据库连接参数信息
    /// </summary>
    public class DbsConnectionParameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        public DbsConnectionParameter(Type type, string key)
            : this(type, key, key)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="displayName"></param>
        public DbsConnectionParameter(Type type, string key, string displayName)
        {
            this.Key = key;
            this.Type = type;
            this.DisplayName = displayName;
            this.SpecifiedType = DbsSpecifiedParameterType.Unspecified;
            this.AllowEmpty = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowEmpty { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public DbsSpecifiedParameterType SpecifiedType { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public bool CheckExistsOrAllowEmpty(DbConnectionStringBuilder builder)
        {
            if (AllowEmpty)
                return true;

            object v;
            if (builder.TryGetValue(this.Key, out v))
            {
                if (v == null)
                    return false;

                if (v is string && string.IsNullOrEmpty((string)v))
                    return false;

                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 数据库特殊连接参数类型。
    /// </summary>
    public enum DbsSpecifiedParameterType
    {
        /// <summary>
        /// 未特别指定的。
        /// </summary>
        Unspecified,
        /// <summary>
        /// 服务名
        /// </summary>
        DataSource,
        /// <summary>
        /// 数据库名
        /// </summary>
        InitialCatalog,
        /// <summary>
        /// 用户名
        /// </summary>
        UserID,
        /// <summary>
        /// 密码
        /// </summary>
        Password
    }

    /// <summary>
    /// 表示数据库连接的设置信息。
    /// </summary>
    public struct ConnectionSettings
    {
        private int _connectionTimeOut;
        private string _applicationName;
        private bool _isEmpty;

        /// <summary>
        /// 返回表示空的实例。
        /// </summary>
        public static readonly ConnectionSettings Empty = new ConnectionSettings { _isEmpty = true };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        public ConnectionSettings(int connectionTimeout)
            : this(string.Empty, connectionTimeout, false)
        {
        }

        private ConnectionSettings(string applicationName, int connectionTimeout, bool isEmpty)
        {
            _isEmpty = isEmpty;
            _applicationName = applicationName;
            _connectionTimeOut = connectionTimeout;
        }

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        public string ApplicationName
        {
            get { return _applicationName; }
            set
            {
                if (!_isEmpty)
                    _applicationName = value;
            }
        }
        /// <summary>
        /// 连接超时。单位秒
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _connectionTimeOut; }
            set
            {
                if (!_isEmpty)
                    _connectionTimeOut = value;
            }
        }
        /// <summary>
        /// 返回是否表示为空的实例。
        /// </summary>
        public bool IsEmpty
        {
            get { return _isEmpty; }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    internal class MsSqlDbsProvider : DbsProvider
    {
        static readonly string _existsCommandPattern;
        static readonly string _existsCommandIdentityPattern;

        static MsSqlDbsProvider()
        {
            _existsCommandPattern = GetExistsCommandPattern(false);
            _existsCommandIdentityPattern = GetExistsCommandPattern(true);
        }

        static string GetExistsCommandPattern(bool isIdentity)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SET NOCOUNT ON");
            sb.AppendLine("BEGIN");
            sb.AppendLine(" IF NOT EXISTS(SELECT [{2}] FROM [{0}].[{1}] WHERE [{2}] = @p0)");
            sb.AppendLine(" BEGIN");

            if (isIdentity)
                sb.AppendLine("     SET IDENTITY_INSERT [{0}].[{1}] ON");

            sb.AppendLine("     INSERT INTO [{0}].[{1}]([{2}])");
            sb.AppendLine("     VALUES(@p0)");

            if (isIdentity)
                sb.AppendLine("     SET IDENTITY_INSERT [{0}].[{1}] OFF");

            sb.AppendLine(" END");
            sb.AppendLine("END");
            sb.AppendLine("SET NOCOUNT OFF");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public MsSqlDbsProvider()
            : base(0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        protected MsSqlDbsProvider(byte code)
            : base(code)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Description
        {
            get { return "MS SQL"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override string RsdsExtension
        {
            get { return "SQL"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override string Provider
        {
            get { return "System.Data.SqlClient"; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paraName"></param>
        /// <returns></returns>
        protected override string GetParameterName(string paraName)
        {
            return "@" + paraName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string QuoteStoreName(object name)
        {
            return string.Format("[{0}]", name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override bool Validate(string connectionString)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(connectionString);

            return !(string.IsNullOrEmpty(sb.DataSource) || string.IsNullOrEmpty(sb.InitialCatalog) || string.IsNullOrEmpty(sb.UserID));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="isIdentity"></param>
        /// <returns></returns>
        protected override string GetEnsureExistsCommand(string schemaName, string tableName, string columnName, bool isIdentity)
        {
            if (isIdentity)
                return string.Format(_existsCommandIdentityPattern, (schemaName == string.Empty ? null : schemaName) ?? "dbo", tableName, columnName);
            else
                return string.Format(_existsCommandPattern, (schemaName == string.Empty ? null : schemaName) ?? "dbo", tableName, columnName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        protected override string OnGetEdmMetadata(Type dtoType)
        {
            return dtoType.Assembly.FullName;
        }

        public override string BuildConnectionString(string connectionString, ConnectionSettings settings)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(connectionString);
            sb.IntegratedSecurity = this.IntegratedSecurity;
            if (!settings.IsEmpty)
            {
                if (!string.IsNullOrEmpty(settings.ApplicationName))
                    sb.ApplicationName = settings.ApplicationName;

                if (settings.ConnectionTimeout > 0)
                    sb.ConnectTimeout = settings.ConnectionTimeout;
            }

            return sb.ConnectionString;
        }
        static DbsConnectionParameter _pDataSource = new DbsConnectionParameter(typeof(string), "Data Source", "服务名") { SpecifiedType = DbsSpecifiedParameterType.DataSource };
        static DbsConnectionParameter _pInitialCatalog = new DbsConnectionParameter(typeof(string), "Initial Catalog", "数据库") { SpecifiedType = DbsSpecifiedParameterType.InitialCatalog };
        static DbsConnectionParameter _pUserID = new DbsConnectionParameter(typeof(string), "User ID", "用户名") { SpecifiedType = DbsSpecifiedParameterType.UserID };
        static DbsConnectionParameter _pPassword = new DbsConnectionParameter(typeof(string), "Password", "密码") { SpecifiedType = DbsSpecifiedParameterType.Password };
        public override IEnumerable<DbsConnectionParameter> GetMainParameters()
        {
            yield return _pDataSource;
            yield return _pInitialCatalog;
            yield return _pUserID;
            yield return _pPassword;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class NtMsSqlDbsProvider : MsSqlDbsProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public NtMsSqlDbsProvider()
            : base(1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Description
        {
            get { return "MS SQL Windows 身份认证"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IntegratedSecurity
        {
            get { return true; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override bool Validate(string connectionString)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(connectionString);
            return !(string.IsNullOrEmpty(sb.DataSource) || string.IsNullOrEmpty(sb.InitialCatalog));
        }

        static DbsConnectionParameter _pDataSource = new DbsConnectionParameter(typeof(string), "Data Source", "服务名") { SpecifiedType = DbsSpecifiedParameterType.DataSource };
        static DbsConnectionParameter _pInitialCatalog = new DbsConnectionParameter(typeof(string), "Initial Catalog", "数据库") { SpecifiedType = DbsSpecifiedParameterType.InitialCatalog };
        public override IEnumerable<DbsConnectionParameter> GetMainParameters()
        {
            yield return _pDataSource;
            yield return _pInitialCatalog;
        }
    }


    internal class OracleDbsProvider : DbsProvider
    {
        static readonly string _existsCommandPattern;
        static OracleDbsProvider()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("INSERT INTO {0}.{1}({2})");
            sb.AppendLine("SELECT :p0 FROM dual");
            sb.AppendLine("WHERE NOT EXISTS(SELECT {2} FROM {0}.{1} WHERE {2} = :p0)");

            _existsCommandPattern = sb.ToString();
        }

        public OracleDbsProvider()
            : base(2)
        {
        }

        public override string Description
        {
            get { return "Oracle"; }
        }

        public override string RsdsExtension
        {
            get { return string.Empty; }
        }

        public override string Provider
        {
            //get { return "Oracle.DataAccess.Client"; }
            get { return "Oracle.ManagedDataAccess.Client"; }
        }

        protected override string GetParameterName(string paraName)
        {
            return ":" + paraName;
        }

        protected override string QuoteStoreName(object name)
        {
            return string.Format("\"{0}\"", name);
        }

        protected override object ConvertValue(object value)
        {
            if (value is bool)
                return (bool)value ? 1 : 0;
            else
                return base.ConvertValue(value);
        }

        protected override DbCommand CreateCommand(DbConnection cn)
        {
            DbCommand cmd = base.CreateCommand(cn);
            cmd.CommandType = System.Data.CommandType.Text;
            return cmd;
        }

        #region 临时处理

        #endregion

        public override bool IntegratedSecurity
        {
            get { return false; }
        }

        protected override string OnGetEdmMetadata(Type dtoType)
        {
            return string.Empty;
        }


        public override bool Validate(string connectionString)
        {
            DbConnectionStringBuilder sb = new DbConnectionStringBuilder();
            sb.ConnectionString = connectionString;

            if (this.IntegratedSecurity)
                return sb.ContainsKey(_pDataSource.Key);
            else
                return sb.ContainsKey(_pDataSource.Key) && sb.ContainsKey(_pUserID.Key);
        }

        protected override string FormatStoredProcedureParameter(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return string.Empty;

            return parameters.Replace("=>", "=");
        }

        protected override string GetEnsureExistsCommand(string schemaName, string tableName, string columnName, bool isIdentity)
        {
            return string.Format(_existsCommandPattern, (schemaName == string.Empty ? null : schemaName) ?? "dbo", tableName, columnName);
        }

        public override string BuildConnectionString(string connectionString, ConnectionSettings settings)
        {
            DbConnectionStringBuilder sb = CreateConnectionStringBuilder();
            sb.ConnectionString = connectionString;
            return sb.ConnectionString;
        }

        static DbsConnectionParameter _pDataSource = new DbsConnectionParameter(typeof(string), "Data Source", "服务名") { SpecifiedType = DbsSpecifiedParameterType.DataSource };
        static DbsConnectionParameter _pUserID = new DbsConnectionParameter(typeof(string), "User ID", "用户名") { SpecifiedType = DbsSpecifiedParameterType.UserID };
        static DbsConnectionParameter _pPassword = new DbsConnectionParameter(typeof(string), "Password", "密码") { SpecifiedType = DbsSpecifiedParameterType.Password };
        static DbsConnectionParameter _pPrivilege = new DbsConnectionParameter(typeof(string), "DBA PRIVILEGE", "数据库角色") { AllowEmpty = true };
        public override IEnumerable<DbsConnectionParameter> GetMainParameters()
        {
            yield return _pDataSource;
            yield return _pUserID;
            yield return _pPassword;
            yield return _pPrivilege;
        }
    }
}
