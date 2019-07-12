using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonModel
{
    /// <summary>
    /// JModel编号
    /// </summary>
    public static class JModelNo
    {
        /// <summary>
        /// table 代码
        /// </summary>
        public readonly static string AccessTableKey = "tbcode";
        /// <summary>
        /// dbs
        /// </summary>
        public readonly static string AccessDbsKey = "dbs";
        /// <summary>
        /// 存储过程
        /// </summary>
        public readonly static string AccessProcKey = "proc";
        /// <summary>
        /// 块查询语句代码
        /// </summary>
        public readonly static string AccessBlockQueryKey = "block";
        /// <summary>
        /// 上下文
        /// </summary>
        public readonly static string AccessContextKey = "context";
        /// <summary>
        /// 用来构建 DbParameter  如请求 { "para": { "name":小明 ,"isboy":true } }
        /// </summary>
        public readonly static string AccessParaKey = "para";
        public readonly static string AccessSaveListKey = "list";

        public readonly static string IsDeleted = "IsDeleted";
        public readonly static string RowVersion = "RowVersion";
       



        #region table code

        /// <summary>
        /// mdt.Test
        /// </summary>
        public readonly static string TB0000 = "TB0000";
        /// <summary>
        /// mdt.ManagementUnit
        /// </summary>
        public readonly static string TB6666 = "TB6666";
        /// <summary>
        /// mdt.ManagementUnitAttribute
        /// </summary>
        public readonly static string TB6667 = "TB6667";

        #endregion




        #region proc code
        
        public readonly static string PROC0000 = "proc.test";

        #endregion


        #region block query code

        public readonly static string BLOCK0000 = " select * from core.domainsystem "; 

        #endregion
    }
}
