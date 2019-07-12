using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonModel
{
    /// <summary>
    /// JModelItem 成员为TBCode、TableName、SeqName 、DtoType 、DbsName
    /// </summary>
    public class JMI
    {
        public JMI()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tbCode"></param>
        /// <param name="tableName"></param>
        /// <param name="seqName"></param>
        /// <param name="dtoType"></param>
        /// <param name="dbsName">默认cis</param>
        public JMI(string tbCode, string tableName, string seqName, Type dtoType, string dbsName = null)
        {
            this.TBCode = tbCode;
            this.TableName = tableName;
            this.SeqName = seqName;
            this.DtoType = dtoType;
            this.DbsName = dbsName ?? DbsNames.CIS;
        }
        /// <summary>
        /// 
        /// </summary>
        public string TBCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SeqName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type DtoType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DbsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProcName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Paras { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BlockQueryCode { get; set; }
    }

}
