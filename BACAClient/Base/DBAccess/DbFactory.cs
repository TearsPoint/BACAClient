using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Base.DBAccess
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public static class DbFactory
    {
        /// <summary>
        /// 取得当前实体在数据库中对应的表名
        /// </summary>
        /// <param name="type"></param>
        public static string GetTableName(this Type type)
        {
            string name = type.Name;
            if (name.StartsWith("Dto", StringComparison.CurrentCultureIgnoreCase))
            {
                name = name.Replace("Dto", string.Empty);
            }
            return name;
        }


        /// <summary>
        ///  返回命令是否为存储过程
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static bool IsProc(this CommandType ct)
        {
            return ct == CommandType.StoredProcedure;
        }

        /// <summary>
        /// 生产对应数据库类型的参数数组
        /// </summary>
        /// <param name="dbc"></param>
        /// <returns></returns>
        public static DbParameter[] CreateDbParameters(DBConnection dbc, int arrayLength)
        {
            switch (dbc.DbType)
            {
                case DbsTypeEnum.SqlServer:
                    return new SqlParameter[arrayLength];
                case DbsTypeEnum.Ole:
                    return new OleDbParameter[arrayLength];
                case DbsTypeEnum.Oracle:
                    return new OracleParameter[arrayLength];
                //case DbsTypeEnum.MySql:
                //    return new MySqlParameter[arrayLength];
                default:
                    return new SqlParameter[arrayLength];
            }
        }

        /// <summary>
        /// 生产对应数据库类型的参数数组
        /// </summary>
        /// <param name="dba"></param>
        /// <param name="direction"></param>
        /// <param name="paraName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(this DBAccessor dba, ParameterDirection direction, string paraName = "", object value = null)
        {
            return dba.CurrentDBConnection.CreateDbParameter(direction, paraName, value);
        }

        /// <summary>
        /// 生产对应数据库类型的参数数组
        /// </summary>
        /// <param name="dbc"></param>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(this DBAccessor dba, string paraName = "", object value = null)
        {
            return dba.CurrentDBConnection.CreateDbParameter(ParameterDirection.Input, paraName, value);
        }

        /// <summary>
        /// 生产对应数据库类型的参数数组
        /// </summary>
        /// <param name="dbc"></param>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(this DBConnection dbc, string paraName = "", object value = null)
        {
            return dbc.CreateDbParameter(ParameterDirection.Input, paraName, value);
        }

        /// <summary>
        /// 生产对应数据库类型的参数数组
        /// </summary>
        /// <param name="dbc"></param>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(this DBConnection dbc, ParameterDirection direction, string paraName = "", object value = null)
        {
            if (dbc == null) return new SqlParameter() { ParameterName = paraName, Value = value, Direction = direction };
            switch (dbc.DbType)
            {
                case DbsTypeEnum.SqlServer:
                    return new SqlParameter() { ParameterName = paraName, Value = value, Direction = direction };
                case DbsTypeEnum.Ole:
                    return new OleDbParameter() { ParameterName = paraName, Value = value, Direction = direction };
                case DbsTypeEnum.Oracle:
                    return new OracleParameter() { ParameterName = paraName, Value = value, Direction = direction };
                //case DbsTypeEnum.MySql:
                //    return new MySqlParameter() { ParameterName = paraName, Value = value, Direction = direction };
                default:
                    return new SqlParameter() { ParameterName = paraName, Value = value, Direction = direction };
            }
        }

        public static DbDataAdapter CreateDbDataAdapter(this DBConnection dbc, string sql, DbConnection connection)
        {
            if (dbc == null) return new SqlDataAdapter(sql, (SqlConnection)connection);
            switch (dbc.DbType)
            {
                case DbsTypeEnum.SqlServer:
                    return new SqlDataAdapter(sql, (SqlConnection)connection);
                case DbsTypeEnum.Ole:
                    return new OleDbDataAdapter(sql, (OleDbConnection)connection);
                case DbsTypeEnum.Oracle:
                    return new OracleDataAdapter(sql, (OracleConnection)connection);
                //case DbsTypeEnum.MySql:
                //    return new MySqlDataAdapter(sql, (MySqlConnection)connection);
                default:
                    return new SqlDataAdapter(sql, (SqlConnection)connection);
            }
        }

        public static DbCommandBuilder CreateCommandBuilder(this DBConnection dbc)
        {
            if (dbc == null) return new SqlCommandBuilder();
            switch (dbc.DbType)
            {
                case DbsTypeEnum.SqlServer:
                    return new SqlCommandBuilder();
                case DbsTypeEnum.Ole:
                    return new OleDbCommandBuilder();
                case DbsTypeEnum.Oracle:
                    return new OracleCommandBuilder();
                //case DbsTypeEnum.MySql:
                //    return new MySqlCommandBuilder();
                default:
                    return new SqlCommandBuilder();
            }
        }
        static IDictionary<DBConnection, IDbScriptExecuter> _cachedExecuter = new Dictionary<DBConnection, IDbScriptExecuter>();
        /// <summary>
        /// 根据数据库连接生产对应的脚本执行器
        /// </summary>
        /// <param name="dbc"></param>
        /// <returns></returns>
        public static IDbScriptExecuter GetOrCreateDbScrpitExecuter(DBConnection dbc)
        {
            //if (!_cachedExecuter.Keys.Contains(dbc))
            //    _cachedExecuter[dbc] = GetExecuterByDbType(dbc);
            return GetExecuterByDbType(dbc);
        }

        public static void ClearCache()
        {
            foreach (var item in _cachedExecuter)
            {
                item.Value.CloseDatabase();
            }
            _cachedExecuter.Clear();
        }

        private static IDbScriptExecuter GetExecuterByDbType(DBConnection dbc)
        {
            switch (dbc.DbType)
            {
                case DbsTypeEnum.SqlServer:
                    return new SqlExecuter(dbc) as IDbScriptExecuter;
                case DbsTypeEnum.Ole:
                    return new OleExecuter(dbc) as IDbScriptExecuter;
                case DbsTypeEnum.Oracle:
                    return new OracleExecuter(dbc) as IDbScriptExecuter;
                //case DbsTypeEnum.MySql:
                //    return new MySqlExecuter(dbc) as IDbScriptExecuter;
                default:
                    return new SqlExecuter(dbc) as IDbScriptExecuter;
            }
        }
    }
}
