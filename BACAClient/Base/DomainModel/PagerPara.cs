using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Base.DBAccess;
using Base;

namespace DomainModel
{
    [DataContract]
    public class PagerPara
    {
        private string dtoTypeFullName;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DtoTypeFullName
        {
            get { return dtoTypeFullName; }
            set { dtoTypeFullName = value; }
        }

        private Type dtoType;
        public Type DtoType
        {
            get
            {
                if (dtoType == null)
                    dtoType = Runtime.FindTypeInCurrentDomain(DtoTypeFullName);
                return dtoType;
            }
        }


        private string tableName;
        /// <summary>
        /// 表名
        /// </summary>
        [DataMember]
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(DtoTypeFullName))
                {
                    tableName = DtoType.GetTableName();
                }
                return tableName;
            }
            set { tableName = value; }
        }

        private string fields = " * ";
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Fields
        {
            get { return fields; }
            set { fields = value; }
        }


        private int pageIndex = 1;
        /// <summary>
        /// 页索引  从1弄始
        /// </summary>
        [DataMember]
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }


        private int pageCount;
        /// <summary>
        /// 页数
        /// </summary>
        [DataMember]
        public int PageCount
        {
            get
            {
                if (PageSize > 0 && TotalCount > 0)
                {
                    pageCount = TotalCount / PageSize;
                    pageCount = (TotalCount % PageSize) > 0 ? pageCount + 1 : pageCount;
                }
                return pageCount;
            }
            set { pageCount = value; }
        }

        private int totalCount;
        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int TotalCount
        {
            get { return totalCount; }
            set { totalCount = value; }
        }

        private int _maxId;

        public int MaxID
        {
            get { return _maxId; }
            set { _maxId = value; }
        }

        private int _minId;

        public int MinID
        {
            get { return _minId; }
            set { _minId = value; }
        }


        private int pageSize = 1000;
        /// <summary>
        /// 每页长度
        /// </summary>
        [DataMember]
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }


        private string condition;
        /// <summary>
        /// 过虑条件
        /// </summary>
        [DataMember]
        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        private string primaryKey;
        public string PrimaryKey
        {
            get
            {
                if (string.IsNullOrEmpty(primaryKey))
                {
                    var pKeys = DBAccessor.GetPrimaryKey(DtoType);
                    if (pKeys.Count() == 1)
                    {
                        primaryKey = pKeys[0].Name;
                    }
                    else
                    {
                        throw new Exception(string.Format(" 类型{0}没主键或主键数大于1，不能进行分页查询 ", DtoType));
                    }
                }
                if (string.IsNullOrEmpty(primaryKey)) return "ID";
                return primaryKey;
            }
            set { primaryKey = value; }
        }

        private string order;
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public string Order
        {
            get
            {
                if (string.IsNullOrEmpty(order))
                    order = primaryKey;
                return order;
            }
            set { order = value; }
        }

        private bool _isDesc;
        /// <summary>
        /// 是否降序
        /// </summary>
        [DataMember]
        public bool IsDESC
        {
            get
            {
                return _isDesc;
            }
            set { _isDesc = value; }
        }

        public override string ToString()
        {
            return string.Format("DtoType:{0}  PageSize:{1}  PageIndex:{2} TotalCount:{3}  PageCount:{4}", DtoTypeFullName, PageSize, pageIndex, TotalCount, PageCount);
        }
    }
}
