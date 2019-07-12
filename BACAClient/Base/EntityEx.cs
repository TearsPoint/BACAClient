using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using Base.Ex;

namespace Base.Ex
{
    /// <summary>
    /// 实体辅助类
    /// </summary>
    public static class EntityEx
    {
        /// <summary>
        /// 用updatedItems中的数据覆盖当前实体集 （不会改变实体集状态EntityState）
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="currentItems">当前实体集</param>
        /// <param name="updatedItems">已更新实体集</param>
        public static void CoverWithData<TDto>(this EntityCollection<TDto> currentItems, IEnumerable<TDto> updatedItems)
            where TDto : class, IEntityWithRelationships
        {
            try
            {
                //如果实体集没有数据，则不需要进行数据覆盖
                if (currentItems == null || currentItems.Count == 0 || updatedItems == null || updatedItems.Count() == 0) return;
                foreach (var dto in currentItems)
                {
                    if (dto == null) continue;
                    //取当前实例的 主键属性集 (导航属性EdmRelationshipNavigationPropertyAttribute 不参与 )
                    PropertyInfo[] primaryKeys = typeof(TDto).GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null && a.GetAttribute<EdmScalarPropertyAttribute>().EntityKeyProperty).ToArray();
                    //取当前实例的 非主键属性集(导航属性EdmRelationshipNavigationPropertyAttribute 不参与 )
                    PropertyInfo[] propertys = typeof(TDto).GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null && !a.GetAttribute<EdmScalarPropertyAttribute>().EntityKeyProperty).ToArray();

                    string filterStr = string.Empty;
                    int i = 0;
                    int upi = primaryKeys.Count();
                    foreach (var key in primaryKeys)
                    {
                        i++;
                        filterStr += string.Format("{0} a.{1}=={2}", i == 1 ? "" : " && ", key.Name, key.GetValue(dto));
                    }

                    //TODO: WH 待实现  ExpressionEx 
                    //Func<TDto, bool> filter = ExpressionEx.ParsePredicate<TDto>(filterStr).Compile();
                    //TDto updatedDto = updatedItems.ToList().Where<TDto>(filter).FirstOrDefault();
                    //if (updatedDto == null) continue;

                    //if (!dto.ReferenceEquals(updatedDto))
                    //{
                    //    foreach (var item in propertys)
                    //    {
                    //        var edmScalarPropertyAttr = item.GetAttribute<EdmScalarPropertyAttribute>();
                    //        item.SetValue(dto, item.GetValue(updatedDto));
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
        }
    }

    /// <summary>
    /// 层级实体
    /// </summary>
    public interface IHierarchicalEntity<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IList<TEntity> Children { get; set; }
    }

    /// <summary>
    /// 数据库对像实体
    /// </summary>
    public class DBObject : IHierarchicalEntity<DBObject>
    {
        public DBObject()
        {

        }

        public string DisplayName
        {
            get;
            set;
        }

        public string SchemaName { get; set; }

        public string Name { get; set; }

        string _dBObjectName;
        public string DBObjectName
        {
            get
            {
                if (DBObjectType == DBObjectType.Schema) _dBObjectName = Name;
                else if (string.IsNullOrEmpty(_dBObjectName))
                    _dBObjectName = string.Format("{0}.{1}", SchemaName, Name);
                return _dBObjectName;
            }
            set
            {
                _dBObjectName = value;
            }
        }

        public DBObjectType DBObjectType { get; set; }

        IList<DBObject> _children;
        public IList<DBObject> Children
        {
            get { if (_children == null) _children = new List<DBObject>(); return _children; }
            set { _children = value; }
        }
    }

    /// <summary>
    /// 数据库对象类型
    /// </summary>
    public enum DBObjectType
    {
        Schema,
        Table,
        View,
        Column
    }
}
