using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Diagnostics;
using System.Data.Common;
using DomainModel;
using System.Data;
using System.Collections;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using Base.Ex;

namespace Base.DBAccess
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbDataReaderEx
    {
        public static IList<T> ToList<T>(this DbDataReader reader) where T : class
        {
            IList<T> result = new List<T>();
            IList<PropertyInfo> blackP = new List<PropertyInfo>();

            TypeConverter valueC = null;
            while (reader.Read())
            {
                T entity = Activator.CreateInstance<T>();
                try
                {
                    //导航属性EdmRelationshipNavigationPropertyAttribute 不参与
                    PropertyInfo[] propertys = typeof(T).GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null).ToArray();
                    if (propertys.Count() == 0)
                        propertys = typeof(T).GetProperties().Where(a => a.GetAttribute<ColumnAttribute>() != null).ToArray();
                    foreach (var item in propertys)
                    {
                        try
                        {
                            if (blackP.Count(a => a.Name == item.Name) == 0)
                            {
                                var a = reader[item.Name];
                                if (a is DBNull) continue;
                                if (a.ToString().IsNumeric())
                                {
                                    valueC = TypeDescriptor.GetConverter(a);
                                    if (a.GetType() != item.PropertyType)
                                        a = valueC.ConvertTo(a, item.PropertyType);
                                }
                                item.SetValue(entity, a, null);  //可直接设置实体的属性值 
                            }
                        }
                        catch (Exception ex)
                        {
                            Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName() + " 列名-> " + item.Name, ex);
                            if (blackP.Count(a => a.Name == item.Name) == 0)
                                blackP.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                result.Add(entity);
            }
            reader.Close();
            return result;
            //return reader.ToList(typeof(T)).CastAs<T>().ToList();
        }

        /// <summary>
        /// 将DataReader转换成集合类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns> 
        public static IList<object> ToList(this DbDataReader reader, Type t)
        {
            IList<object> result = new List<object>();
            IList<PropertyInfo> blackP = new List<PropertyInfo>();

            TypeConverter valueC = null;
            while (reader.Read())
            {
                var entity = Activator.CreateInstance(t);
                try
                {
                    //导航属性EdmRelationshipNavigationPropertyAttribute 不参与
                    PropertyInfo[] propertys = t.GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null).ToArray();
                    if (propertys.Count() == 0)
                        propertys = t.GetProperties().Where(a => a.GetAttribute<ColumnAttribute>() != null).ToArray();
                    foreach (var item in propertys)
                    {
                        try
                        {
                            if (blackP.Count(a => a.Name == item.Name) == 0)
                            {
                                var a = reader[item.Name];
                                if (a is DBNull) continue;
                                if (a.ToString().IsNumeric())
                                {
                                    valueC = TypeDescriptor.GetConverter(a);
                                    if (a.GetType() != item.PropertyType)
                                        a = valueC.ConvertTo(a, item.PropertyType);
                                }
                                item.SetValue(entity, a, null);  //可直接设置实体的属性值 
                            }
                        }
                        catch (Exception ex)
                        {
                            Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName() + " 列名-> " + item.Name, ex);
                            if (blackP.Count(a => a.Name == item.Name) == 0)
                                blackP.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                }
                result.Add(entity);
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// 根据IDataReader对象生成IList<ODictionary>的列表
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IList<ODictionary> ToODictionaryList(this IDataReader reader)
        {
            ODictionary oDict = null;
            IList<ODictionary> clsList = new List<ODictionary>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    DataRowCollection rows = reader.GetSchemaTable().Rows;
                    oDict = new ODictionary();
                    foreach (DataRow row in rows)
                    {
                        oDict.Add(row[0].ToString(), reader[row[0].ToString()].ToString());
                    }
                    clsList.Add(oDict);
                }
                reader.Close();
                reader.Dispose();
            }
            return clsList;
        }

        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class, new()
        {
            //创建一个属性的列表
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例
                TResult ob = new TResult();
                //找到对应的数据  并赋值
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.
                oblist.Add(ob);
            }
            return oblist;
        }

        /// <summary>
        /// 转换为一个DataTable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        ///// <param name="value"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IEnumerable list)
        {
            //创建属性的集合
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口
            Type type = list.AsQueryable().ElementType;
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in list)
            {
                //创建一个DataRow实例
                DataRow row = dt.NewRow();
                //给row 赋值
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable
                dt.Rows.Add(row);
            }
            return dt;
        }


        /// <summary>
        /// 转换为一个DataTable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        ///// <param name="value"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TResult>(this IEnumerable<TResult> value) where TResult : class
        {
            //创建属性的集合
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口
            Type type = typeof(TResult);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in value)
            {
                //创建一个DataRow实例
                DataRow row = dt.NewRow();
                //给row 赋值
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable
                dt.Rows.Add(row);
            }
            return dt;
        }

    }


}
