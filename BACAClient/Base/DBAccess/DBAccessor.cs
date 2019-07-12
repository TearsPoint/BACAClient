using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Base.IO;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Data.Common;
using System.Diagnostics;
using System.Data.Entity;
using System.Data;
using DomainModel;
using Base;
using Base.Ex;

namespace Base.DBAccess
{
    /// <summary>
    /// 数据库访问器
    /// </summary>
    public class DBAccessor : IDbScriptExecuter
    {
        static IDictionary<string, DBAccessor> _cachedAccessor = new Dictionary<string, DBAccessor>();
        /// <summary>
        /// 是否已加载了配置文件标志
        /// </summary>
        static bool _isLoadConfig = false;

        static DBAccessor()
        {
            Load(Runtime.ConfigFilePath);
        }

        internal DBConnection CurrentDBConnection { get; set; }
        public DBAccessor(DBConnection dbc)
        {
            CurrentDBConnection = dbc;
        }

        public DBAccessor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string AccessorKey { get; set; }

        public override string ToString()
        {
            return string.Format("当前连接总数{0}:\r\n{1}", _cachedAccessor.Count, _cachedAccessor.Select(a => a.Key).ToString(","));
        }

        /// <summary>
        /// 取得一个数据库访问器实例
        /// </summary>
        public static DBAccessor Instance(DBConnection dbc)
        {
            DBAccessor dba = new DBAccessor(dbc) { AccessorKey = string.Format("{0}_{1}", dbc.ConnName, Guid.NewGuid().ToString()) };
            _cachedAccessor[dba.AccessorKey] = dba;
            dba.ConnectionDatabase();
            return dba;
        }

        static DBAccessor _instance;
        public static DBAccessor Instance()
        {
            if (_instance == null)
                _instance = new DBAccessor();
            _instance.ConnectionDatabase();
            return _instance;
        }

        public static void ClearCache()
        {
            _cachedAccessor.Clear();
            DbFactory.ClearCache();
        }

        /// <summary>
        /// 加载数据库配置
        /// </summary>
        /// <param name="configPath"></param>
        public static void Load(string configPath)
        {
            try
            {
                if (_isLoadConfig) return;
                Dbs.Clear();

                if (!File.Exists(configPath)) return;

                XElement doc;
                using (TextReader tr = TextReader.Synchronized(new StreamReader(configPath)))
                {
                    doc = XElement.Load(tr);
                }

                if (doc == null) return;

                DBConnection dbc;
                foreach (var item in doc.GetChildElements("connectionStrings"))
                {
                    dbc = new DBConnection()
                    {
                        ConnName = item.Attributes("name").FirstOrDefault().Value,
                        ConnString = item.Attributes("connectionString").FirstOrDefault().Value,
                        Type = item.Attributes("providerName").FirstOrDefault() == null ? "SqlServer" : item.Attributes("providerName").FirstOrDefault().Value
                    };
                    FormaterConnString(dbc);

                    Dbs.DBConnections.Add(new KeyValuePair<string, DBConnection>(dbc.ConnName, dbc));
                }
                _isLoadConfig = true;
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
        }

        /// <summary>
        /// 格式化连接串
        /// </summary>
        /// <param name="conn"></param>
        private static void FormaterConnString(DBConnection conn)
        {
            if (conn.DbType == DbsTypeEnum.Ole)
            {
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(conn.ConnString);
                builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                builder.ConnectionString = builder.ConnectionString;
                if (!File.Exists(builder.DataSource))
                    builder.DataSource = string.Format(builder.DataSource, Path.Combine(Runtime.ExecutablePath.GetParentDirectoryPath(), GlobalParameter.DataBaseFolderName));
                //conn.ConnString = string.Format(conn.ConnString, Path.Combine(Runtime.ExecutablePath, GlobalParameter.DataBaseFolderName));
                conn.ConnString = builder.ToString();
            }
        }

        /// <summary>
        /// 创建一个可存取的模型容器
        /// </summary>
        /// <typeparam name="TEdm"></typeparam>
        /// <param name="db"></param>
        public TEdm CreateEdm<TEdm>() where TEdm : ObjectContext
        {
            return CreateEdm<TEdm>(CurrentDBConnection);
        }

        /// <summary>
        /// 创建一个可存取的模型容器
        /// </summary>
        /// <typeparam name="TEdm"></typeparam>
        /// <param name="db"></param>
        public TEdm CreateEdm<TEdm>(DBConnection db) where TEdm : ObjectContext
        {
            if (db == null)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName + ":" + MethodBase.GetCurrentMethod(), "数据库连接DBConnection为NULL");
                return null;
            }
            if (CurrentDBConnection == null) CurrentDBConnection = db;

            //todo MySql.Data.MySqlClient
            //{"无法将类型为“Oracle.DataAccess.Client.OracleConnection”的对象强制转换为类型“System.Data.SqlClient.SqlConnection”。"}
            string providerName = db.DbType == DbsTypeEnum.Oracle ? "Oracle.DataAccess.Client" : "System.Data.SqlClient";

            //{"尝试加载 Oracle 客户端库时引发 BadImageFormatException。如果在安装 32 位 Oracle 客户端组件的情况下以 64 位模式运行，将出现此问题。"}
            //string providerName = db.DbType == DbsTypeEnum.Oracle ? "System.Data.OracleClient" : "System.Data.SqlClient";

            //{"指定的存储区提供程序在配置中找不到，或者无效。"}{"找不到请求的 .Net Framework Data Provider。可能没有安装。"}
            //string providerName = db.DbType == DbsTypeEnum.Oracle ? "System.Data.EntityClient" : "System.Data.SqlClient";


            string providerString = db.ConnString;
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;

            entityBuilder.ProviderConnectionString = providerString;

            if (typeof(TEdm).Assembly.FullName.StartsWith("DataModel.") && db.DbType == DbsTypeEnum.Oracle)
            {
                string assName = typeof(TEdm).Assembly.FullName.Substring(0, typeof(TEdm).Assembly.FullName.IndexOf(','));
                entityBuilder.Metadata = string.Format("res://{0}/", typeof(TEdm).Assembly.FullName.Replace(assName, assName + ".Oracle"));
            }
            else
                entityBuilder.Metadata = db.DbType == DbsTypeEnum.Oracle ? string.Format("res://{0}/", typeof(TEdm).Assembly.FullName.Replace(".DataModel", ".DataModel.Oracle"))
                    : string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", typeof(TEdm).Name);
            //entityBuilder.Metadata="res://DataModel.His.Oracle, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null/";
            db.EdmConnString = entityBuilder.ToString();

            TEdm edm = Activator.CreateInstance(typeof(TEdm), new object[] { db.EdmConnString }) as TEdm;

            return edm;
        }

        public TEdm CreateEdmEx<TEdm>(DBConnection db) where TEdm : ObjectContext
        {
            if (db == null)
                Loger.Log(Assembly.GetExecutingAssembly().FullName + ":" + MethodBase.GetCurrentMethod(), "数据库连接DBConnection为NULL");

            //{"无法将类型为“Oracle.DataAccess.Client.OracleConnection”的对象强制转换为类型“System.Data.SqlClient.SqlConnection”。"}
            string providerName = db.DbType == DbsTypeEnum.Oracle ? "Oracle.DataAccess.Client" : "System.Data.SqlClient";

            //{"尝试加载 Oracle 客户端库时引发 BadImageFormatException。如果在安装 32 位 Oracle 客户端组件的情况下以 64 位模式运行，将出现此问题。"}
            //string providerName = db.DbType == DbsTypeEnum.Oracle ? "System.Data.OracleClient" : "System.Data.SqlClient";

            //{"指定的存储区提供程序在配置中找不到，或者无效。"}{"找不到请求的 .Net Framework Data Provider。可能没有安装。"}
            //string providerName = db.DbType == DbsTypeEnum.Oracle ? "System.Data.EntityClient" : "System.Data.SqlClient";


            string providerString = db.ConnString;
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;

            entityBuilder.ProviderConnectionString = providerString;

            entityBuilder.Metadata = db.DbType == DbsTypeEnum.Oracle ? string.Format("res://{0}/", typeof(TEdm).Assembly.FullName.Replace(".DataModel", ".DataModel.Oracle"))
                : string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", typeof(TEdm).Name);

            db.EdmConnString = entityBuilder.ToString();

            TEdm edm = Activator.CreateInstance(typeof(TEdm), new object[] { db.EdmConnString }) as TEdm;

            return edm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public T GetByKey<T>(object key, string schema = "")
            where T : class
        {
            return GetByKey<T>(CurrentDBConnection, key, schema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbc"></param>
        /// <param name="key"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public T GetByKey<T>(DBConnection dbc, object key, string schema = "")
            where T : class
        {
            T result = default(T);
            //取当前实例的 主键属性集
            PropertyInfo[] primaryKeys = GetPrimaryKey<T>();
            try
            {
                DbParameter[] para = new DbParameter[]
                    {
                        this.CreateDbParameter("Key", key)
                    };
                if (primaryKeys.Count() > 0)
                {
                    string sql = string.Format(@"select * from {0}{1}  ", CreateSchemaByDbType(dbc, schema), GetTableNameByDtoName(typeof(T)));
                    sql = sql + string.Format(" where {0}=@Key", primaryKeys.First().Name);
                    result = DbFactory.GetOrCreateDbScrpitExecuter(dbc).GetReader(sql, para, false).ToList<T>().First();
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveDto<T>(T entity)
        {
            return SaveDto<T>(entity, CurrentDBConnection);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveDto<T>(T entity, DBConnection dbc)
        {
            return SaveDto<T>(entity, dbc, "dbo");
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveDto<T>(T entity, string schema)
        {
            return SaveDto<T>(entity, CurrentDBConnection, schema);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveDto<T>(T entity, DBConnection dbc, string schema)
        {
            bool f = false;
            int i = 0;
            StringBuilder sql = new StringBuilder();  //将要执行的Sql
            try
            {
                //取当前实例的 主键属性集
                PropertyInfo[] primaryKeys = GetPrimaryKey<T>();
                //取当前实例的 非主键属性集
                PropertyInfo[] propertys = typeof(T).GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null && !a.GetAttribute<EdmScalarPropertyAttribute>().EntityKeyProperty).ToArray();
                //参数集
                DbParameter[] paras = DbFactory.CreateDbParameters(dbc, typeof(T).GetProperties().Count());

                bool isExist = false;  //数据库是否已存当前记录
                sql.Append(string.Format(@"select count(1) from {0}{1} ", CreateSchemaByDbType(dbc, schema), GetTableNameByDtoName(typeof(T))));
                sql = CreateSqlWhereByPrimaryKes(dbc, entity, sql, ref paras, primaryKeys);
                isExist = DbFactory.GetOrCreateDbScrpitExecuter(dbc).ExecuteScalar(sql.ToString(), GetAvailableDbParas(paras), false) > 0;

                if (isExist)  //如果存在相同的主键，执行更新操作
                {
                    //Ole类型库没有架构名schema
                    sql.Clear();
                    paras = DbFactory.CreateDbParameters(dbc, typeof(T).GetProperties().Count());
                    sql.Append(string.Format(@"update {0}{1} set ", CreateSchemaByDbType(dbc, schema), GetTableNameByDtoName(typeof(T))));

                    i = 0;
                    foreach (var item in propertys)
                    {
                        i++;
                        paras[i - 1] = this.CreateDbParameter(CreateDbParameterName(item.Name), item.GetValue(entity));
                        sql.AppendLine(string.Format(@"{0}={1}{2}", item.Name, CreateDbParameterName(item.Name), i == propertys.Count() ? string.Empty : ","));
                    }

                    sql = CreateSqlWhereByPrimaryKes(dbc, entity, sql, ref paras, primaryKeys);

                    DbFactory.GetOrCreateDbScrpitExecuter(dbc).ExecuteCommand(sql.ToString(), GetAvailableDbParas(paras), false);

                    f = true;
                }
                else  //否则执行新增
                {
                    sql.Clear();
                    paras = DbFactory.CreateDbParameters(dbc, typeof(T).GetProperties().Count());

                    sql.Append(string.Format(@" insert into {0}{1} ( ", CreateSchemaByDbType(dbc, schema), GetTableNameByDtoName(typeof(T))));
                    i = 0;
                    if (primaryKeys.Count() > 1 || primaryKeys.Count(a => a.PropertyType.Name == typeof(string).Name) > 0)
                    {
                        foreach (var item in primaryKeys)
                        {
                            i++;
                            sql.Append(string.Format(@" {0},", item.Name));
                        }
                    }
                    i = 0;
                    foreach (var item in propertys)
                    {
                        i++;
                        sql.Append(string.Format(@" {0}{1}", item.Name, i == propertys.Count() ? ")" : ","));
                    }
                    sql.AppendLine(" values( ");
                    i = 0;
                    if (primaryKeys.Count() > 1 || primaryKeys.Count(a => a.PropertyType.Name == typeof(string).Name) > 0)
                    {
                        foreach (var item in primaryKeys)
                        {
                            i++;
                            paras[i - 1] = (DbParameter)Activator.CreateInstance(DbFactory.CreateDbParameter(dbc).GetType(), new object[] { CreateDbParameterName(item.Name), item.GetValue(entity) });
                            sql.Append(string.Format(@" {0},", CreateDbParameterName(item.Name)));
                        }
                    }
                    i = 0;
                    int upi = paras.Where(a => a != null).Count();
                    foreach (var item in propertys)
                    {
                        i++;
                        paras[upi + i - 1] = (DbParameter)Activator.CreateInstance(DbFactory.CreateDbParameter(dbc).GetType(), new object[] { CreateDbParameterName(item.Name), item.GetValue(entity) });
                        sql.Append(string.Format(@" {0}{1}", CreateDbParameterName(item.Name), i == propertys.Count() ? ")" : ","));
                    }
                    DbParameter[] p = GetAvailableDbParas(paras);
                    DbFactory.GetOrCreateDbScrpitExecuter(dbc).ExecuteCommand(sql.ToString(), p, false);

                    sql.Clear();
                    sql.Append("   select @@identity; ");  // oracle sql="select seq_atable.currval from dual"
                    if (primaryKeys.Count() > 0)
                        primaryKeys.First().SetValue(entity, DbFactory.GetOrCreateDbScrpitExecuter(dbc).ExecuteScalar(sql.ToString(), false), null);
                }
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }

            return f;
        }

        /// <summary>
        /// 取得主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static PropertyInfo[] GetPrimaryKey<T>()
        {
            return GetPrimaryKey(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static PropertyInfo[] GetPrimaryKey(Type type)
        {
            return type.GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null && a.GetAttribute<EdmScalarPropertyAttribute>().EntityKeyProperty).ToArray();
        }

        /// <summary>
        /// 检查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbc"></param>
        /// <param name="entity"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public bool CheckDbIsExistCurrentEntityPrimarykey<T>(DBConnection dbc, T entity, string schema)
        {
            bool isExist;
            StringBuilder sql = new StringBuilder();
            //取当前实例的 主键属性集
            PropertyInfo[] primaryKeys = typeof(T).GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null && a.GetAttribute<EdmScalarPropertyAttribute>().EntityKeyProperty).ToArray();
            sql.Append(string.Format(@"select count(1) from {0}{1} ", CreateSchemaByDbType(dbc, schema), GetTableNameByDtoName(typeof(T))));
            DbParameter[] paras = DbFactory.CreateDbParameters(dbc, typeof(T).GetProperties().Count());
            sql = CreateSqlWhereByPrimaryKes(dbc, entity, sql, ref paras, primaryKeys);
            isExist = DbFactory.GetOrCreateDbScrpitExecuter(dbc).ExecuteScalar(sql.ToString(), GetAvailableDbParas(paras), false) > 0;
            return isExist;
        }

        /// <summary>
        /// 取得有效的DbParameter集
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public DbParameter[] GetAvailableDbParas(DbParameter[] paras)
        {
            return paras.Where(a => a != null).ToArray();
        }

        public DbConnection GetConnection()
        {
            return CurrentDbScriptExecuter.GetConnection();
        }


        public bool SaveDataTable(DataTable dataTable, string tableName)
        {
            string sql = string.Format("select * from {0}", tableName);

            DbDataAdapter sda = CurrentDBConnection.CreateDbDataAdapter(sql, GetConnection());
            DbCommandBuilder scb = CurrentDBConnection.CreateCommandBuilder();
            sda.InsertCommand = scb.GetInsertCommand();
            sda.UpdateCommand = scb.GetUpdateCommand();
            sda.DeleteCommand = scb.GetDeleteCommand();

            sda.Fill(dataTable);
            var r = sda.Update(dataTable);
            return true;
        }

        /// <summary>
        /// 根据主键集生成 T-Sql的Where语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        private StringBuilder CreateSqlWhereByPrimaryKes<T>(DBConnection dbc, T entity, StringBuilder sql, ref DbParameter[] paras, PropertyInfo[] primaryKeys)
        {
            int i = 0;
            int upi = paras.Where(a => a != null).Count();
            foreach (var item in primaryKeys)
            {
                i++;
                paras[upi + i - 1] = (DbParameter)Activator.CreateInstance(DbFactory.CreateDbParameter(dbc).GetType(), new object[] { CreateDbParameterName(item.Name), item.GetValue(entity) });
                if (i == 1)
                    sql.AppendLine(string.Format(@"where {0}={1}", item.Name, CreateDbParameterName(item.Name)));
                else
                {
                    sql.Append(string.Format(@" and {0}={1}", item.Name, CreateDbParameterName(item.Name)));
                }
            }
            return sql;
        }

        /// <summary>
        /// 创建表名前缀（Ole访问时不需要架构名）
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private string CreateSchemaByDbType(DBConnection dbc, string schema)
        {
            return dbc.DbType == DbsTypeEnum.Ole ? "" : schema + ".";
        }

        /// <summary>
        /// 创建型如@Para的参数名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private string CreateDbParameterName(string fieldName)
        {
            return string.Format(@"@{0}", fieldName);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public bool DeleteDto<T>(T entity, DBConnection dbc)
        {
            bool f = false;

            return f;
        }

        /// <summary>
        /// 取得当前实体在数据库中对应的表名
        /// </summary>
        /// <param name="type"></param>
        public string GetTableNameByDtoName(Type type)
        {
            string name = type.Name;
            if (name.StartsWith("Dto", StringComparison.CurrentCultureIgnoreCase))
            {
                name = name.Replace("Dto", string.Empty);
            }
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">对应映射表的架构名 默认为repc</param>
        /// <returns></returns>
        private string GetTableName<T>(string schema)
        {
            string name = typeof(T).Name;
            if (name.StartsWith("Dto", StringComparison.CurrentCultureIgnoreCase))
            {
                name = name.Replace("Dto", string.Empty);
            }
            return string.Format("{0}.{1}", schema, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ConnectionDatabase()
        {
            if (CurrentDBConnection == null) return false;
            return ConnectionDatabase(CurrentDBConnection);
        }

        public IDbScriptExecuter CurrentDbScriptExecuter { get; set; }

        public bool ConnectionDatabase(DBConnection db)
        {
            CurrentDbScriptExecuter = DbFactory.GetOrCreateDbScrpitExecuter(db);
            return CurrentDbScriptExecuter.ConnectionDatabase(db);
        }

        public void StartTransaction()
        {
            CurrentDbScriptExecuter.StartTransaction();
        }

        public int ExecuteCommand(string sqlOrProcedureName, bool isProcedure)
        {
            return CurrentDbScriptExecuter.ExecuteCommand(sqlOrProcedureName, isProcedure);
        }

        public int ExecuteCommand(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure)
        {
            return CurrentDbScriptExecuter.ExecuteCommand(sqlOrProcedureName, paras, isProcedure);
        }

        public int ExecuteCommand(string sqlOrProcedureName, DbParameter para, bool isProcedure)
        {
            return CurrentDbScriptExecuter.ExecuteCommand(sqlOrProcedureName, para, isProcedure);
        }

        public int ExecuteScalar(string sqlOrProcedureName, bool isProcedure)
        {
            return CurrentDbScriptExecuter.ExecuteScalar(sqlOrProcedureName, isProcedure);
        }

        public int ExecuteScalar(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure)
        {
            return CurrentDbScriptExecuter.ExecuteScalar(sqlOrProcedureName, paras, isProcedure);
        }

        public int ExecuteScalar(string sqlOrProcedureName, DbParameter para, bool isProcedure)
        {
            return CurrentDbScriptExecuter.ExecuteScalar(sqlOrProcedureName, para, isProcedure);
        }

        public System.Data.DataTable GetDataTable(string sqlOrProcedureName, bool isProcedure)
        {
            return CurrentDbScriptExecuter.GetDataTable(sqlOrProcedureName, isProcedure);
        }

        public System.Data.DataTable GetDataTable(string sqlOrProcedureName, DbParameter para, bool isProcedure)
        {
            return CurrentDbScriptExecuter.GetDataTable(sqlOrProcedureName, para, isProcedure);
        }

        public System.Data.DataTable GetDataTable(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure)
        {
            return CurrentDbScriptExecuter.GetDataTable(sqlOrProcedureName, paras, isProcedure);
        }

        public DbDataReader GetReader(string sqlOrProcedureName, bool isProcedure)
        {
            return CurrentDbScriptExecuter.GetReader(sqlOrProcedureName, isProcedure);
        }

        public DbDataReader GetReader(string sqlOrProcedureName, DbParameter para, bool isProcedure)
        {
            return CurrentDbScriptExecuter.GetReader(sqlOrProcedureName, para, isProcedure);
        }

        public DbDataReader GetReader(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure)
        {
            return CurrentDbScriptExecuter.GetReader(sqlOrProcedureName, paras, isProcedure);
        }

        public DbDataReader GetReader(string sqlOrProcedureName, bool isProcedure, params DbParameter[] paras)
        {
            return CurrentDbScriptExecuter.GetReader(sqlOrProcedureName, paras, isProcedure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public DbDataReader GetReader(ref PagerPara pager)
        {
            int skipCount = (pager.PageIndex - 1) * pager.PageSize;
            DbParameter[] para = new DbParameter[]
                    {
                        this.CreateDbParameter("pageIndex",pager.PageIndex ),
                        this.CreateDbParameter("pageSize",pager.PageSize ),
                        this.CreateDbParameter("skipCount",skipCount )
                    };

            pager.TotalCount = this.ExecuteScalar(string.Format(@"select count(1) from {0} where 1>0 {1};", pager.TableName, pager.Condition), false);

            if (this.CurrentDBConnection.DbType.IsOle())
            {
                pager.Order = string.Format(" {0} {1} ", pager.PrimaryKey, pager.IsDESC ? "DESC" : "ASC");
            }

            if (this.CurrentDBConnection.DbType.IsOle() && pager.TotalCount > pager.PageSize && pager.PageIndex - 1 > 0)  //数据超过一页
            {
                if (!pager.Order.Contains("DESC"))
                    pager.MaxID = this.ExecuteScalar(string.Format(@"select max({1}) from ( select top {2} {1} from {0} where 1>0 {4} {3}) as t", pager.TableName, pager.PrimaryKey, pager.PageSize * (pager.PageIndex - 1), string.IsNullOrEmpty(pager.Order) ? "" : string.Format("order by {0}", pager.Order), pager.Condition), false);
                else
                    pager.MinID = this.ExecuteScalar(string.Format(@"select min({1}) from ( select top {2} {1} from {0} where 1>0 {4} {3}) as t", pager.TableName, pager.PrimaryKey, pager.PageSize * (pager.PageIndex - 1), string.IsNullOrEmpty(pager.Order) ? "" : string.Format("order by {0}", pager.Order), pager.Condition), false);
            }

            StringBuilder sql = new StringBuilder(string.Format(@" select top {2} {1} from {0} where 1>0", pager.TableName, pager.Fields, pager.PageSize));
            if (!this.CurrentDBConnection.DbType.IsOle() && skipCount > 0)
            {
                sql.Append(string.Format(" and {1} not in ( select top {2} {1} from {3}  where 1>0 {0} {4}  )", pager.Condition, pager.PrimaryKey, skipCount, pager.TableName, string.IsNullOrEmpty(pager.Order) ? "" : string.Format("order by {0}", pager.Order)));
            }
            if (this.CurrentDBConnection.DbType.IsOle() && pager.MaxID > 0)
            {
                sql.Append(string.Format(" and {0}>{1}", pager.PrimaryKey, pager.MaxID));
            }
            if (this.CurrentDBConnection.DbType.IsOle() && pager.MinID > 0)
            {
                sql.Append(string.Format(" and {0}<{1}", pager.PrimaryKey, pager.MinID));
            }
            if (!string.IsNullOrEmpty(pager.Condition))
            {
                sql.Append(string.Format(" {0}", pager.Condition));
            }
            if (!string.IsNullOrEmpty(pager.Order))
            {
                sql.Append(string.Format(" order by {0}", pager.Order));
            }
            return this.GetReader(sql.ToString(), para, false);
        }

        public void Commit()
        {
            CurrentDbScriptExecuter.Commit();
        }

        public void RollBack()
        {
            CurrentDbScriptExecuter.RollBack();
        }

        public void CloseDatabase()
        {
            CurrentDbScriptExecuter.CloseDatabase();
        }

        public void Dispose()
        {
            if (CurrentDBConnection == null) return;
            CloseDatabase();
            CurrentDbScriptExecuter.Dispose();
            _cachedAccessor.Remove(this.AccessorKey);
        }

    }
}
