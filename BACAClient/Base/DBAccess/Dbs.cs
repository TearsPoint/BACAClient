using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Base.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using System.Web.Configuration;
using JetSun.Core.DBAccess;
using Base.Ex;
using JsonModel;

namespace Base.DBAccess
{
    /// <summary>
    /// 数据库连接集
    /// </summary>
    public class Dbs
    {
        static IDictionary<string, DBConnection> _dbsList = new Dictionary<string, DBConnection>();
        static Dbs()
        {
            DBAccessor.Load(Runtime.ConfigFilePath);
            string path = Runtime.DbsFilePath;
            if (Runtime.DbsFilePath != null && File.Exists(Runtime.DbsFilePath))
            {
                DbsManager dbsManager = null;
                if (dbsManager == null)
                    dbsManager = DbsManager.Instance;
                _dbsList.Clear();
                DBAccessor.ClearCache();
                if (dbsManager.DbsList != null)
                {
                    foreach (var d in dbsManager.DbsList)
                    {
                        if (d.Value.CurrentConnectionString.IsNullOrWhiteSpace()) continue;
                        DBConnection dbc = new DBConnection()
                        {
                            ConnName = d.Key.QualifiedName,
                            ConnString = d.Value.CurrentConnectionString,
                            EdmConnString = d.Value.CurrentConnectionString,
                            Type = GetDbsType(d.Value.Provider.Code)
                        };
                        _dbsList.Add(new KeyValuePair<string, DBConnection>(dbc.ConnName, dbc));
                    }
                }
                Runtime.Dbs = _dbsList;
                Dbs.DBConnections = _dbsList;
            }

        }

        public static string GetDbsType(byte code)
        {
            if (code == 2) return "Oracle";
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            if (DBConnections != null)
            {
                DBConnections.Clear();
            }
            DBConnections = new Dictionary<string, DBConnection>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static IDictionary<string, DBConnection> DBConnections;

        /// <summary>
        /// 默认返cis
        /// </summary>
        /// <param name="dbsName"></param>
        /// <returns></returns>
        public static DBConnection Get(string dbsName)
        {
            if (DBConnections.ContainsKey(dbsName))
                return DBConnections[dbsName];
            else
                return DBConnections[DbsNames.CIS];
        }

        public static DBConnection CoreDB
        {
            get
            {
                return DBConnections[DbsNames.CoreDB];
            }
        }

        #region JetSun DBS

        public static DBConnection HIS
        {
            get
            {
                return DBConnections[DbsNames.HIS];
            }
        }

        public static DBConnection CIS
        {
            get
            {
                return DBConnections[DbsNames.CIS];
            }
        }

        public static DBConnection MPI
        {
            get
            {
                return DBConnections[DbsNames.MPI];
            }
        }

        public static DBConnection EHR
        {
            get
            {
                return DBConnections[DbsNames.EHR];
            }
        }

        public static DBConnection XDS
        {
            get
            {
                return DBConnections[DbsNames.XDS];
            }
        }

        public static DBConnection RHIN
        {
            get
            {
                return DBConnections[DbsNames.RHIN];
            }
        }

        public static DBConnection IXS
        {
            get
            {
                return DBConnections[DbsNames.IXS];
            }
        }

        public static DBConnection DXS
        {
            get
            {
                return DBConnections[DbsNames.DXS];
            }
        }

        public static DBConnection MIS
        {
            get
            {
                return DBConnections[DbsNames.MIS];
            }
        }

        public static DBConnection CDR
        {
            get
            {
                return DBConnections[DbsNames.CDR];
            }
        }

        public static DBConnection PIS
        {
            get
            {
                return DBConnections[DbsNames.PIS];
            }
        }

        public static DBConnection HealthCase
        {
            get
            {
                return DBConnections[DbsNames.HealthCase];
            }
        }

        #endregion
    }


}
