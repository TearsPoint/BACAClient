using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace Base.DBAccess
{
    /// <summary>
    /// 数据库脚本执行器操作规范
    /// </summary>
    public interface IDbScriptExecuter : IDisposable
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        bool ConnectionDatabase(DBConnection db);

        /// <summary>
        /// 开始事务
        /// </summary>
        void StartTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DbConnection GetConnection();


        #region 执行ExecuteNonQuery方法，返回执行结果
        /// <summary>
        /// 执行ExecuteNonQuery方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        int ExecuteCommand(string sqlOrProcedureName, bool isProcedure);

        /// <summary>
        /// 执行ExecuteNonQuery方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="paras">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        int ExecuteCommand(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure);

        /// <summary>
        /// 执行ExecuteNonQuery方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="value">参数/param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        int ExecuteCommand(string sqlOrProcedureName, DbParameter para, bool isProcedure);
        #endregion

        #region 执行ExecuteScalar方法，返回执行结果
        /// <summary>
        /// 执行ExecuteScalar方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        int ExecuteScalar(string sqlOrProcedureName, bool isProcedure);

        /// <summary>
        /// 执行ExecuteScalar方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="paras">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        int ExecuteScalar(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure);

        /// <summary>
        /// 执行ExecuteScalar方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="para">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        int ExecuteScalar(string sqlOrProcedureName, DbParameter para, bool isProcedure);
        #endregion

        #region 返回DataTable
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        DataTable GetDataTable(string sqlOrProcedureName, bool isProcedure);

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="para">参数</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        DataTable GetDataTable(string sqlOrProcedureName, DbParameter para, bool isProcedure);

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="paras">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        DataTable GetDataTable(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure);
        #endregion

        #region 返回DataReader
        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        DbDataReader GetReader(string sqlOrProcedureName, bool isProcedure);

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="values">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        DbDataReader GetReader(string sqlOrProcedureName, DbParameter para, bool isProcedure);

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="paras">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        DbDataReader GetReader(string sqlOrProcedureName, DbParameter[] paras, bool isProcedure);

        #endregion

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollBack();

        /// <summary>
        /// 关闭连接
        /// </summary>
        void CloseDatabase();
    }

    /// <summary>
    /// 数据库脚本执行器
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    /// <typeparam name="TTransaction"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TDataReader"></typeparam>
    /// <typeparam name="TDataAdapter"></typeparam>
    public class DbScriptExecuter<TConnection, TTransaction, TCommand, TParameter, TDataReader, TDataAdapter> : IDbScriptExecuter, IDisposable
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TDataReader : DbDataReader
        where TDataAdapter : DbDataAdapter
    {
        protected TConnection connection;

        protected string connString;

        protected bool isConnected;

        protected bool isTransaction;

        protected TTransaction transaction;

        protected TCommand command;

        /// <summary>
        /// 实例化一个数据帮助对象，并且与数据库建立连接
        /// </summary>
        /// <param name="dbc"></param>
        public DbScriptExecuter(DBConnection dbc)
        {
            isConnected = false;
            isTransaction = false;
            ConnectionDatabase(dbc);
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        public bool ConnectionDatabase(DBConnection db)
        {
            if (!isConnected)
            {
                try
                {
                    if (connection == null)
                    {
                        connString = db.ConnString;
                        //connection = new TConnection(connString);
                        connection = Activator.CreateInstance(typeof(TConnection), new object[] { connString }) as TConnection;
                        connection.Open();
                    }
                    else if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    if (command == null)
                    {
                        //command = new TCommand();
                        command = Activator.CreateInstance<TCommand>();
                    }
                    command.Connection = connection;
                }
                catch (Exception ex)
                {
                    Loger.Log("Core", "ConnectionDatabase(..)", ex);
                    isConnected = false;
                    return false;
                }
            }
            isConnected = true;
            return true;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void StartTransaction()
        {
            if (!isConnected)
            {
                throw new Exception("连接数据库失败");
            }
            isTransaction = true;
            transaction = connection.BeginTransaction() as TTransaction;
            command.Transaction = transaction;
        }

        #region 执行ExecuteNonQuery方法，返回执行结果

        protected virtual void OnAddParameter(TCommand command)
        {

        }


        /// <summary>
        /// 执行ExecuteNonQuery方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public int ExecuteCommand(string sqlOrProcedureName, bool isProcedure)
        {
            int result = 0;
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        /// <summary>
        /// 执行ExecuteNonQuery方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="values">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public int ExecuteCommand(string sqlOrProcedureName, DbParameter[] values, bool isProcedure)
        {
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                OnAddParameter(command);
                command.Parameters.AddRange(values);
            }
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 执行ExecuteNonQuery方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="value">参数/param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public int ExecuteCommand(string sqlOrProcedureName, DbParameter value, bool isProcedure)
        {
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                OnAddParameter(command);
                command.Parameters.Add(value);
            }
            return command.ExecuteNonQuery();
        }
        #endregion

        #region 执行ExecuteScalar方法，返回执行结果
        /// <summary>
        /// 执行ExecuteScalar方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public int ExecuteScalar(string sqlOrProcedureName, bool isProcedure)
        {
            int result = 0;
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                OnAddParameter(command);
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }
        /// <summary>
        /// 执行ExecuteScalar方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="values">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public int ExecuteScalar(string sqlOrProcedureName, DbParameter[] values, bool isProcedure)
        {
            int result = 0;
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                command.Parameters.AddRange(values);
                OnAddParameter(command);
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }
        /// <summary>
        /// 执行ExecuteScalar方法，返回执行结果
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="value">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public int ExecuteScalar(string sqlOrProcedureName, DbParameter value, bool isProcedure)
        {
            int result = 0;
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                command.Parameters.Add(value);
                OnAddParameter(command);
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }
        #endregion

        #region 返回DataTable
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public DataTable GetDataTable(string sqlOrProcedureName, bool isProcedure)
        {
            //SIMPLIFIED CHINESE_CHINA.ZHS16GBK

            //if ((connection is OracleConnection))
            //{
            //    OracleGlobalization info = (connection as OracleConnection).GetSessionInfo();
            //    info.Language=("SIMPLIFIED CHINESE_CHINA.ZHS16GBK");
            //    (connection as OracleConnection).SetSessionInfo(info);
            //}

            DataSet ds = new DataSet();
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;

                OnAddParameter(command);
                //SqlDataAdapter da = new SqlDataAdapter(command);
                TDataAdapter da = Activator.CreateInstance(typeof(TDataAdapter), new object[] { command }) as TDataAdapter;
                da.Fill(ds);
            }
            return ds.Tables[0];
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="value">参数</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public DataTable GetDataTable(string sqlOrProcedureName, DbParameter value, bool isProcedure)
        {
            DataSet ds = new DataSet();
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                OnAddParameter(command);
                command.Parameters.Add(value);
                //TDataAdapter da = new TDataAdapter(command);
                TDataAdapter da = Activator.CreateInstance(typeof(TDataAdapter), new object[] { command }) as TDataAdapter;
                da.Fill(ds);
            }
            return ds.Tables[0];
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="values">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public DataTable GetDataTable(string sqlOrProcedureName, DbParameter[] values, bool isProcedure)
        {
            DataSet ds = new DataSet();
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                OnAddParameter(command);
                command.Parameters.AddRange(values);
                //SqlDataAdapter da = new SqlDataAdapter(command);
                TDataAdapter da = Activator.CreateInstance(typeof(TDataAdapter), new object[] { command }) as TDataAdapter;
                da.Fill(ds);
            }
            return ds.Tables[0];
        }
        #endregion

        #region 返回DataReader
        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public DbDataReader GetReader(string sqlOrProcedureName, bool isProcedure)
        {
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                OnAddParameter(command);
            }
            return command.ExecuteReader() as TDataReader;
        }

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="values">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public DbDataReader GetReader(string sqlOrProcedureName, DbParameter value, bool isProcedure)
        {
            if (isConnected)
            {
                if (isProcedure)
                    command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                OnAddParameter(command);
                command.Parameters.Add(value);
            }
            return command.ExecuteReader() as TDataReader;
        }

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sqlOrProcedureName">Sql语句或者存储过程名</param>
        /// <param name="values">参数集合</param>
        /// <param name="isProcedure">是否存储过程，存储过程（true）</param>
        public DbDataReader GetReader(string sqlOrProcedureName, DbParameter[] values, bool isProcedure)
        {
            if (isConnected)
            {
                if (isProcedure)
                {
                    command.CommandType = CommandType.StoredProcedure;
                }
                command.CommandText = sqlOrProcedureName;
                command.Parameters.Clear();
                OnAddParameter(command);
                command.Parameters.AddRange(values);
            }
            return command.ExecuteReader() as TDataReader;
        }

        #endregion

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            transaction.Commit();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            transaction.Rollback();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseDatabase()
        {
            this.Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (isConnected)
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                    connection.Dispose();
                    isConnected = false;

                    if (command.Parameters != null && command.Parameters.Count > 0)
                    {
                        for (int i = 0; i < command.Parameters.Count; i++)
                        {
                            if (command.Parameters[i] is OracleParameter)
                                (command.Parameters[i] as OracleParameter).Dispose();
                            command.Parameters[i] = null;
                        }
                    }
                    command = null;
                    connection = null;
                    transaction = null;
                }
                GC.SuppressFinalize(true);
            }
        }


        public DbConnection GetConnection()
        {
            return connection;
        }


    }


    /// <summary>
    /// SqlServer数据库脚本执行器
    /// </summary>
    public class SqlExecuter : DbScriptExecuter<SqlConnection, SqlTransaction, SqlCommand, SqlParameter, SqlDataReader, SqlDataAdapter>, IDisposable
    {
        /// <summary>
        /// 实例化一个数据帮助对象，并且与数据库建立连接
        /// </summary>
        /// <param name="dbc"></param>
        public SqlExecuter(DBConnection dbc)
            : base(dbc)
        {
        }
    }

    /// <summary>
    /// Ole数据库脚本执行器
    /// </summary>
    public class OleExecuter : DbScriptExecuter<OleDbConnection, OleDbTransaction, OleDbCommand, OleDbParameter, OleDbDataReader, OleDbDataAdapter>, IDisposable
    {
        /// <summary>
        /// 实例化一个数据帮助对象，并且与数据库建立连接
        /// </summary>
        /// <param name="dbc"></param>
        public OleExecuter(DBConnection dbc)
            : base(dbc)
        {
        }
    }

    /// <summary>
    /// Oracle数据库脚本执行器
    /// </summary>
    public class OracleExecuter : DbScriptExecuter<OracleConnection, OracleTransaction, OracleCommand, OracleParameter, OracleDataReader, OracleDataAdapter>, IDisposable
    {
        /// <summary>
        /// 实例化一个数据帮助对象，并且与数据库建立连接
        /// </summary>
        /// <param name="dbc"></param>
        public OracleExecuter(DBConnection dbc)
            : base(dbc)
        {
        }

        protected override void OnAddParameter(OracleCommand command)
        {
            base.OnAddParameter(command);
            if (command.CommandText.Contains(":rs_1"))
                command.Parameters.Add("rs_1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        }
    }

    ///// <summary>
    ///// MySql数据库脚本执行器
    ///// </summary>
    //public class MySqlExecuter : DbScriptExecuter<MySqlConnection, MySqlTransaction, MySqlCommand, MySqlParameter, MySqlDataReader, MySqlDataAdapter>, IDisposable
    //{
    //    /// <summary>
    //    /// 实例化一个数据帮助对象，并且与数据库建立连接
    //    /// </summary>
    //    /// <param name="dbc"></param>
    //    public MySqlExecuter(DBConnection dbc)
    //        : base(dbc)
    //    {
    //    }
    //}
}