using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Base.DBAccess
{
    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DbsTypeEnum
    {
        [Description("微软的Sql Server数据库")]
        SqlServer = 1,
        [Description("Access小型库")]
        Ole = 2,
        [Description("Oracle数据库")]
        Oracle = 3, 
        [Description("MySQL数据库")]
        MySql=4
    }

    public static class DbsTypeEx
    {
        public static bool IsOle(this DbsTypeEnum te)
        {
            if (te == DbsTypeEnum.Ole)
                return true;
            return false;
        }
    }
}
