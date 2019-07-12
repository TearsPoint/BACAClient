using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;

namespace Base.DBAccess
{
    /// <summary>
    /// TP数据库连接相关信息
    /// </summary>
    public class DBConnection
    {
        public override string ToString()
        {
            return string.Format("数据库类型[{0}]", Type);
        }

        /// <summary>
        /// 连接名
        /// </summary>
        public string ConnName { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; set; }

        /// <summary>
        /// EF连接串
        /// </summary>
        public string EdmConnString { get; set; }

        /// <summary>
        /// 数据库类型配置 可取:Ole、Oracle  默认支持SqlServer
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 返回数据库类型（SqlServer\Oracle\Access\MySql）
        /// </summary>
        public DbsTypeEnum DbType
        {
            get
            {
                if (Type.Equals("Ole", StringComparison.CurrentCultureIgnoreCase))
                    return DbsTypeEnum.Ole;
                if (Type.Equals("Oracle", StringComparison.CurrentCultureIgnoreCase))
                    return DbsTypeEnum.Oracle;
                if (Type.Equals("MySql", StringComparison.CurrentCultureIgnoreCase))
                    return DbsTypeEnum.MySql;
                return DbsTypeEnum.SqlServer;
            }
        }
        
        /// <summary>
        /// 实体连接
        /// </summary>
        private EntityConnection _entityConnection;
        public EntityConnection EntityConnection
        {
            get
            {
                if (_entityConnection == null)
                    _entityConnection = new EntityConnection(EdmConnString);
                return _entityConnection;
            }
        }
    }
}
