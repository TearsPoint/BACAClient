using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using DataService.ServiceModel;
using Base;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;
using System.Xml;
using DomainModel;
using Base.DBAccess;
using System.Data.Common;
using System.Data;
using Base.IO;
using System.Reflection;
using fastJSON;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.ServiceModel.Configuration;
using Newtonsoft.Json;
using System.Data.Linq.Mapping;
using Base.Ex;
using JsonModel;
using Oracle.ManagedDataAccess.Client;

namespace DataService.ServiceLibrary
{
    /// <summary>
    /// 
    /// </summary> 
    public abstract class SvcBase
    {
        public static string TableName<T>()
        {
            return TableName(typeof(T));
        }
        /// <summary>
        /// 表名
        /// </summary> 
        public static string TableName(Type t)
        {
            if (t.GetCustomAttribute<TableAttribute>() != null)
                return t.GetCustomAttribute<TableAttribute>().Name;
            else
                throw new Exception(string.Format("{0}类型没有TableAttribute标注", t.FullName));
        }

        public static string SeqName<T>()
        {
            return SeqName(typeof(T));
        }

        /// <summary>
        /// 序列名
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SeqName(Type t)
        {
            if (t.GetCustomAttribute<SeqAttribute>() != null)
                return t.GetCustomAttribute<SeqAttribute>().Name;
            else
                return string.Empty;
            //throw new Exception(string.Format("{0}类型没有SeqAttribute标注", t.FullName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual DBConnection OnGetDBConnection() { return Dbs.CIS; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetPostData()
        {
            string pd = string.Empty;
            if (OperationContext.Current.RequestContext.RequestMessage.Properties.ContainsKey("Body"))
                pd = OperationContext.Current.RequestContext.RequestMessage.Properties["Body"].ToString();
            return pd;
        }

        /// <summary>
        /// 获取postData中操作的目标模型定义
        /// </summary>
        /// <param name="dj"></param>
        /// <returns></returns>
        public JMI GetJMI(DynamicJson dj)
        {
            JMI jmi = new JMI();
            if (dj.IsList && dj.GetStr(JModelNo.AccessTableKey).IsNullOrWhiteSpace())
                dj = (dj as dynamic)[0];
            if (dj.GetStr(JModelNo.AccessTableKey) != null && dj.GetStr(JModelNo.AccessTableKey).Length > 0)
            {
                jmi = JModelList.Models[dj.GetStr(JModelNo.AccessTableKey).ToUpper()];
            }
            if (!dj.GetStr(JModelNo.AccessDbsKey).IsNullOrEmpty())
                jmi.DbsName = dj.GetStr(JModelNo.AccessDbsKey);
            if (!dj.GetStr(JModelNo.AccessProcKey).IsNullOrEmpty())
                jmi.ProcName = dj.GetStr(JModelNo.AccessProcKey);
            if (!dj.GetStr(JModelNo.AccessContextKey).IsNullOrEmpty())
                jmi.Context = dj.GetStr(JModelNo.AccessContextKey);
            if (!dj.GetStr(JModelNo.AccessParaKey).IsNullOrEmpty())
                jmi.Paras = dj.Get(JModelNo.AccessParaKey).ToJson();
            if (!dj.GetStr(JModelNo.AccessBlockQueryKey).IsNullOrEmpty())
                jmi.BlockQueryCode = dj.GetStr(JModelNo.AccessBlockQueryKey);

            return jmi;
        }

        /// <summary>
        /// 取得客户端 post 的json数据转为DynamicJson
        /// </summary>
        /// <returns></returns>
        public DynamicJson GetParam()
        {
            var data = GetPostData();
            return data.AsDynamicJson();
        }

        /// <summary>
        /// 取得客户端 post 的json数据转为实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetParam<T>()
        {
            return GetPostData().ToObject<T>();
        }

        /// <summary>
        /// 取主键字名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static PropertyInfo[] GetPrimaryKey<T>()
        {
            return GetPrimaryKey(typeof(T));
        }

        /// <summary>
        /// 取主键名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static PropertyInfo[] GetPrimaryKey(Type type)
        {
            return type.GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null && a.GetAttribute<EdmScalarPropertyAttribute>().EntityKeyProperty)
                .Union(type.GetProperties().Where(a => a.GetAttribute<ColumnAttribute>() != null && a.GetAttribute<ColumnAttribute>().IsPrimaryKey)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dba"></param>
        /// <param name="dynamicJson"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DbParameter[] BuildParameter<T>(DBAccessor dba, DynamicJson dynamicJson, ref IList<string> columns)
        {
            return BuildParameter(typeof(T), dba, dynamicJson, ref columns);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dba"></param>
        /// <param name="dynamicJson"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DbParameter[] BuildParameter(Type t, DBAccessor dba, DynamicJson dynamicJson, ref IList<string> columns)
        {
            IList<DbParameter> paras = new List<DbParameter>();
            PropertyInfo[] propertys = t.GetProperties().Where(a => a.GetAttribute<EdmScalarPropertyAttribute>() != null).ToArray();
            if (propertys.Count() == 0)
                propertys = t.GetProperties().Where(a => a.GetAttribute<ColumnAttribute>() != null).ToArray();

            foreach (var columnName in dynamicJson.Keys)
            {
                try
                {
                    bool isDate = false;
                    var p = propertys.FirstOrDefault(b => b.Name.ToUpper() == columnName.ToUpper());
                    if (p != null && p.PropertyType == typeof(DateTime))
                        isDate = true;
                    if (columnName.StartsWith("_") || propertys.Count(b => b.Name.ToUpper() == columnName.ToUpper()) == 0) continue;
                    if (dynamicJson.ContainsKey(columnName))
                    {
                        object v = dynamicJson.Get(columnName);
                        if (isDate)
                            v = v.Convert<DateTime>();
                        else if (v.ToString().ToUpper() == "TRUE" || v.ToString().ToUpper() == "FALSE")
                            v = v.Convert<bool>() ? 1 : 0;
                        paras.Add(dba.CreateDbParameter(columnName, v));
                    }
                    columns.Add(columnName);
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName() + "\r\n列名->\r\n" + columnName, ex);
                }
            }
            return paras.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dba"></param>
        /// <param name="dynamicJson">以下划线开头的key不会构建参数</param>
        /// <returns></returns>
        public DbParameter[] BuildParameter(DBAccessor dba, DynamicJson dynamicJson)
        {
            IList<DbParameter> paras = new List<DbParameter>();

            foreach (var columnName in dynamicJson.Keys)
            {
                try
                {
                    bool isDate = false;
                    if (dynamicJson.GetStr(columnName) != null && columnName.Contains("Date"))
                        isDate = true;
                    if (columnName.StartsWith("_")) continue;

                    if (dynamicJson.ContainsKey(columnName))
                    {
                        var dp = dba.CreateDbParameter(columnName, isDate ? dynamicJson.Get(columnName).Convert<DateTime>() : dynamicJson.Get(columnName));
                        if (columnName == "rs_1" && dp.GetType().FullName.Contains("Oracle"))
                        {
                            (dp as OracleParameter).OracleDbType = OracleDbType.RefCursor;
                            dp.Direction = ParameterDirection.Output;
                        }
                        paras.Add(dp);
                    }
                }
                catch (Exception ex)
                {
                    Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName() + "\r\n列名->\r\n" + columnName, ex);
                }
            }
            return paras.ToArray();
        }

        protected string SaveOne<T>(DBAccessor dba, DynamicJson dj, string primaryKey = "Id")
            where T : class
        {
            return SaveOne(typeof(T), dba, dj, primaryKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dba"></param>
        /// <param name="dj"></param>
        /// <param name="primaryKey"></param>
        /// <param name="tableName"></param>
        /// <param name="seqName"></param>
        /// <returns></returns>
        protected string SaveOne(Type t, DBAccessor dba, DynamicJson dj, string primaryKey = "Id")
        {
            int rows = 0;
            string sql = string.Empty;
            string json = string.Empty;
            string tableName = TableName(t);
            IList<string> cols = new List<string>();

            try
            {
                if (GetPrimaryKey(t).Count() > 0)
                    primaryKey = GetPrimaryKey(t).FirstOrDefault().Name;
                if (dj.Get<int>(primaryKey) >= 0)
                {
                    json = this.Update(t, dba, dj);
                    return json;
                }

                string seqName = SeqName(t);
                int id = dj.Get<int>(primaryKey);
                if (!seqName.IsNullOrWhiteSpace())
                {
                    id = dba.ExecuteScalar(string.Format(" select {0}.NEXTVAL from dual", seqName), false);
                    dj.Set(primaryKey, id);
                }

                dj.Set("RowVersion", DateTime.Now);
                DbParameter[] paras = BuildParameter(t, dba, dj, ref cols);
                sql = string.Format(@"
                    insert into {0} ( {1})
                    values ( {2} )", tableName, cols.ToString(" ,"), cols.Select(a => ":" + a).ToString(","));
                rows = dba.ExecuteCommand(sql, paras, false);
                json = dba.GetReader(string.Format("select * from {0} where {1}=:{1}", tableName, primaryKey), dba.CreateDbParameter(primaryKey, dj.Get(primaryKey)), false)
                    .ToList(t).FirstOrDefault().ToJson();
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return json;
        }

        protected string Update<T>(DBAccessor dba, DynamicJson dj, string primaryKey = "Id")
            where T : class
        {
            return Update(typeof(T), dba, dj, primaryKey);
            //int rows = 0;
            //string sql = string.Empty;
            //string json = string.Empty;
            //string tableName = TableName<T>();
            //IList<string> cols = new List<string>();
            //dj.Set("RowVersion", DateTime.Now);
            //DbParameter[] paras = BuildParameter<T>(dba, dj, ref cols);
            //sql = string.Format(" update {0} set {1} where {2}=:{2}", tableName, cols.Select(a => a + "= :" + a).ToString(","), primaryKey);
            //rows = dba.ExecuteCommand(sql, paras, false);
            //json = dba.GetReader(string.Format("select * from {0} where {1}=:{1}", tableName, primaryKey), dba.CreateDbParameter(primaryKey, dj.Get(primaryKey)), false)
            //    .ToList<T>().FirstOrDefault().ToJson();
            //return json;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dba"></param>
        /// <param name="dj"></param>
        /// <param name="primaryKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected string Update(Type t, DBAccessor dba, DynamicJson dj, string primaryKey = "Id")
        {
            int rows = 0;
            string sql = string.Empty;
            string json = string.Empty;
            if (GetPrimaryKey(t).Count() > 0)
                primaryKey = GetPrimaryKey(t).FirstOrDefault().Name;
            string tableName = TableName(t);
            IList<string> cols = new List<string>();
            dj.Set("RowVersion", DateTime.Now);
            DbParameter[] paras = BuildParameter(t, dba, dj, ref cols);
            sql = string.Format(" update {0} set {1} where {2}=:{2}", tableName, cols.Select(a => a + "= :" + a).ToString(","), primaryKey);
            rows = dba.ExecuteCommand(sql, paras, false);
            json = dba.GetReader(string.Format("select * from {0} where {1}=:{1}", tableName, primaryKey), dba.CreateDbParameter(primaryKey, dj.Get(primaryKey)), false)
                .ToList(t).FirstOrDefault().ToJson();
            return json;
        }

        public virtual string Save()
        {
            try
            {
                DynamicJson dj = GetParam();
                JMI jmi = GetJMI(dj);
                if (!dj.IsList && !dj.GetStr(JModelNo.AccessSaveListKey).IsNullOrEmpty())
                {
                    dj = dj.Get(JModelNo.AccessSaveListKey).ToJson().ToDynamicJson();
                    if (!jmi.TBCode.IsNullOrWhiteSpace())
                    {
                        foreach (DynamicJson djItem in dj)
                        {
                            djItem.Set(JModelNo.AccessTableKey, jmi.TBCode);
                        }
                    }
                }
                using (DBAccessor dba = DBAccessor.Instance(Dbs.Get(jmi.DbsName)))
                {
                    return OnSave(dj, dba);
                }
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dJson"></param>
        /// <param name="dba"></param>
        /// <returns></returns>
        protected string OnSave(dynamic dJson, DBAccessor dba, bool isDelete = false)
        {
            string json = string.Empty;
            DynamicJson dj = dJson as DynamicJson;

            if (dj.IsList)
            {
                json = "[";
                int i = 0;
                foreach (DynamicJson djItem in dj)
                {
                    if (isDelete)
                        djItem.Set("IsDeleted", true);
                    json += DoSaveOne(dJson[i], dba) + ",";
                    i++;
                }
                json.TrimEnd(',');
                json += "]";
            }
            else if (dj.PropertyCount > 0)
            {
                if (isDelete)
                    dj.Set("IsDeleted", true);
                json = DoSaveOne(dj, dba);
            }
            return json;
        }

        public string SaveList<T>(dynamic dj, DBAccessor dba, bool isDelete = false)
            where T : class
        {
            string json = string.Empty;
            json = "[";
            int i = 0;
            foreach (DynamicJson djItem in dj)
            {
                if (isDelete)
                    djItem.Set("IsDeleted", true);
                json += SaveOne<T>(dba, dj[i]) + ",";
                i++;
            }
            json += "]";
            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dJson"></param>
        /// <param name="dba"></param>
        /// <returns></returns>
        protected virtual string DoSaveOne(DynamicJson dJson, DBAccessor dba)
        {
            string json = string.Empty;
            json = this.SaveOne(GetJMI(dJson).DtoType, dba, dJson);
            return json;
        }

        /// <summary>
        /// 逻辑删除  post 数据格式类似为  { primaryKey: 1,2 }
        /// </summary>
        /// <returns></returns>
        public string Delete()
        {
            string json = GetPostData();
            DynamicJson dj = GetParam();
            JMI jmi = GetJMI(dj);
            if (!dj.IsList && !dj.GetStr(JModelNo.AccessSaveListKey).IsNullOrEmpty())
            {
                dj = dj.Get(JModelNo.AccessSaveListKey).ToJson().ToDynamicJson();
                if (!jmi.TBCode.IsNullOrWhiteSpace())
                {
                    foreach (DynamicJson djItem in dj)
                    {
                        djItem.Set(JModelNo.AccessTableKey, jmi.TBCode);
                    }
                }
            }
            using (DBAccessor dba = DBAccessor.Instance(Dbs.Get(jmi.DbsName)))
            {
                return OnSave(dj, dba, true);
            }
        }

        public string DoDelete(Type dtotype, DBAccessor dba, DynamicJson dj)
        {
            try
            {
                return SaveOne(dtotype, dba, dj);
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
        }

        public virtual string Remove()
        {
            DynamicJson dj = GetParam();
            string resJson = string.Empty;
            string primaryKey = "Id";
            try
            {
                JMI jmi = GetJMI(dj);
                using (DBAccessor dba = DBAccessor.Instance(Dbs.Get(jmi.DbsName)))
                {
                    string condition = string.Empty;
                    string sql = string.Empty;
                    string tableName = TableName(jmi.DtoType);
                    IList<string> cols = new List<string>();

                    if (GetPrimaryKey(jmi.DtoType).Count() > 0)
                        primaryKey = GetPrimaryKey(jmi.DtoType).FirstOrDefault().Name;

                    DbParameter[] paras = BuildParameter(jmi.DtoType, dba, dj, ref cols);
                    int rows = dba.ExecuteCommand(string.Format("delete from {0} where {1}=:{1}", tableName, primaryKey), dba.CreateDbParameter(primaryKey, dj.Get(primaryKey)), false);
                    if (rows > 0)
                        resJson = new { result = 1 }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return resJson;
        }


        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string DoRemove(DBAccessor dba, string condition, string tableName = null)
        {
            int returnValue = -1;
            try
            {
                StringBuilder sql = new StringBuilder(string.Format("delete from {0} where {1}", tableName, condition));
                returnValue = dba.ExecuteCommand(sql.ToString(), false);
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return new { result = returnValue }.ToJson();
        }


        public virtual string GetByKey()
        {
            DynamicJson dj = GetParam();
            string resJson = string.Empty;
            string primaryKey = "Id";
            try
            {
                JMI jmi = GetJMI(dj);
                using (DBAccessor dba = DBAccessor.Instance(Dbs.Get(jmi.DbsName)))
                {
                    string condition = string.Empty;
                    string sql = string.Empty;
                    string tableName = TableName(jmi.DtoType);
                    IList<string> cols = new List<string>();

                    if (GetPrimaryKey(jmi.DtoType).Count() > 0)
                        primaryKey = GetPrimaryKey(jmi.DtoType).FirstOrDefault().Name;

                    DbParameter[] paras = BuildParameter(jmi.DtoType, dba, dj, ref cols);
                    resJson = dba.GetReader(string.Format("select * from {0} where {1}=:{1}", tableName, primaryKey), dba.CreateDbParameter(primaryKey, dj.Get(primaryKey)), false)
                        .ToList(jmi.DtoType).FirstOrDefault().ToJson();
                }
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return resJson;
        }


        public virtual string DoGet<T>(DBAccessor dba, string condition = "")
            where T : class
        {
            return DoGet(dba, typeof(T), condition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual string DoGet(DBAccessor dba, Type dtoType, string condition = "")
        {
            string result = null;
            try
            {
                IList<object> dtos = dba.GetReader(string.Format("SELECT * FROM {0} {1} {2}", TableName(dtoType), condition.IsNullOrWhiteSpace() ? "" : "where", condition), false).ToList(dtoType);
                result = JSON.ToJSON(dtos);
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return result;
        }

        /// <summary>
        ///  支持 
        ///  1.表查询  reqJson: { "TBCode" : xxx , "Id":123}
        ///  2.proc 查询  reqJson { "proc":"data.proctest" , para:{"name": "tom" } , "dbs":"Dbs1"}
        ///  3.语句块查询 reqJson { "block": "" }
        /// </summary>
        /// <returns></returns>
        public string Query()
        {
            DynamicJson dj = GetParam();
            string resJson = string.Empty;
            string primaryKey = "Id";
            try
            {
                JMI jmi = GetJMI(dj);
                using (DBAccessor dba = DBAccessor.Instance(Dbs.Get(jmi.DbsName)))
                {
                    string condition = string.Empty;
                    string sql = string.Empty;

                    DbParameter[] paras = null;
                    if (!jmi.TableName.IsNullOrWhiteSpace())
                    {
                        string tableName = TableName(jmi.DtoType);
                        IList<string> cols = new List<string>();

                        if (GetPrimaryKey(jmi.DtoType).Count() > 0)
                            primaryKey = GetPrimaryKey(jmi.DtoType).FirstOrDefault().Name;

                        paras = BuildParameter(jmi.DtoType, dba, dj, ref cols);
                        resJson = DoQuery(dj, jmi, dba, tableName, cols, paras);
                    }
                    else if (!jmi.ProcName.IsNullOrWhiteSpace())
                    {
                        paras = BuildParameter(dba, jmi.Paras.AsDynamicJson());
                        resJson = dba.GetDataTable(jmi.ProcName, paras, true).ToJson();
                    }
                    else if (!jmi.BlockQueryCode.IsNullOrWhiteSpace())
                    {
                        paras = BuildParameter(dba, jmi.Paras.AsDynamicJson());
                        resJson = dba.GetDataTable(jmi.BlockQueryCode, paras, false).ToJson();
                    }
                }
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return resJson;
        }

        public virtual string DoQuery(DynamicJson dj, JMI jmi, DBAccessor dba, string tableName, IList<string> cols, DbParameter[] paras)
        {
            return dba.GetReader(string.Format("select * from {0} where {1}", tableName, cols.Select(a => a + "= :" + a).ToString(",")), paras, false)
                .ToList(jmi.DtoType).ToJson();
        }
    }


    /// <summary>
    /// 标注操作契约方法自行选择数据契约的序列化器。不可继承此类
    /// <para>对于类型使用特性[NetDataContractFormat]标注的参数，使用NetDataContractSerializer</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AdaptiveDataContractSerializerAttribute : Attribute, IOperationBehavior
    {
        /// <summary>
        /// 获取/设置是否使用大数据块的DataContractSerializer。
        /// </summary>
        public bool UseBlobDataContractSerializer { get; set; }

        private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description, bool useBlobDataContractSerializer)
        {
            DataContractSerializerOperationBehavior dcsOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if (dcsOperationBehavior != null)
            {
                description.Behaviors.Remove(dcsOperationBehavior);
                description.Behaviors.Add(new AdaptiveDataContractSerializerOperationBehavior(description) { UseBlobDataContractSerializer = useBlobDataContractSerializer });
            }
        }

        #region IOperationBehavior 成员

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription, this.UseBlobDataContractSerializer);
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription, this.UseBlobDataContractSerializer);
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }

        #endregion

        /// <summary>
        /// Represents the run-time behavior of the <see cref="DataContractSerializer"/>.
        /// </summary>
        private class AdaptiveDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
        {
            public AdaptiveDataContractSerializerOperationBehavior(OperationDescription operationDescription) :
                base(operationDescription)
            { }

            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                if (type.GetAttribute<NetDataContractFormatAttribute>() != null)
                    return new NetDataContractSerializer(name, ns);

                if (UseBlobDataContractSerializer)
                    return new DataContractSerializer(type, name, ns, knownTypes,
                    int.MaxValue /*maxItemsInObjectGraph*/,
                    false/*ignoreExtensionDataObject*/,
                    false/*preserveObjectReferences*/,
                    null/*dataContractSurrogate*/);


                return base.CreateSerializer(type, name, ns, knownTypes);
            }

            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                if (type.GetAttribute<NetDataContractFormatAttribute>() != null)
                    return new NetDataContractSerializer(name, ns);

                if (UseBlobDataContractSerializer)
                    return new DataContractSerializer(type, name, ns, knownTypes,
                    int.MaxValue /*maxItemsInObjectGraph*/,
                    false/*ignoreExtensionDataObject*/,
                    false/*preserveObjectReferences*/,
                    null/*dataContractSurrogate*/);

                return base.CreateSerializer(type, name, ns, knownTypes);
            }

            public bool UseBlobDataContractSerializer { get; set; }
        }

    }

    /// <summary>
    /// 指示类型采用"NetDataContractSerializer"序列化器。不可继承此类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NetDataContractFormatAttribute : Attribute
    {
    }


    public class RequestLogUtils
    {
        public static int logOperation(string url, string payload)
        {
            //var url = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.OriginalString;
            //var method = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.Method;
            //var userAgent = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UserAgent;
            int key = 0;

            // Do stuff to insert url, method, user agent and request payload in the database
            // the generated key from the insertion will be returned as the key variable

            return key;

        }

        public static void logResponse(int resCode, string resPayload)
        {
            int traceID = 0;
            if (OperationContext.Current.RequestContext.RequestMessage.Properties.ContainsKey("traceID"))
                traceID = (int)System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Properties["traceID"];

            // Do stuff to update the log record in the database based on the ID
            // This method updates response code and response payload
        }

        public static void logSuccess()
        {
            int traceID = 0;
            if (OperationContext.Current.RequestContext.RequestMessage.Properties.ContainsKey("traceID"))
                traceID = (int)System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Properties["traceID"];

            // Do stuff to update the log record in the database based on the ID
            // This method just updates log status to success
        }

        public static void logException(string error)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;

            int traceID = 0;
            if (OperationContext.Current.RequestContext.RequestMessage.Properties.ContainsKey("traceID"))
                traceID = (int)System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Properties["traceID"];

            // Do stuff to update the log record in the database based on the ID
            // This method just updates log status to error and log the error message
        }


        public RequestLogUtils()
        {
        }
    }

    /// <summary>
    /// 请求特殊处理
    /// </summary>
    public class IncomingMessageLogger : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // Set up the message and stuff
            Uri requestUri = request.Headers.To;
            HttpRequestMessageProperty httpReq = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(ms);
            request.WriteMessage(writer);
            writer.Flush();
            // Log the message in the Database
            string messageBody = Encoding.UTF8.GetString(ms.ToArray());
            var traceID = RequestLogUtils.logOperation(request.Headers.To.ToString(), messageBody);

            // Reinitialize readers and stuff
            ms.Position = 0;
            XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
            Message newMessage = Message.CreateMessage(reader, int.MaxValue, request.Version);

            // Put the ID generated at insertion time in a property
            // in order to use it over again to update the log record
            // with the response payload and, OK or error status
            request.Properties.Add("traceID", traceID);
            request.Properties.Add("Body", messageBody);
            newMessage.Properties.CopyProperties(request.Properties);

            writer.Close();
            reader.Close();
            ms.Close();

            request = newMessage;
            return requestUri;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(ms);
                reply.WriteMessage(writer);
                writer.Flush();

                // Log the response in the Database         
                HttpResponseMessageProperty prop = (HttpResponseMessageProperty)reply.Properties["httpResponse"];
                WebBodyFormatMessageProperty propBodyFomat = reply.Properties["WebBodyFormatMessageProperty"] as WebBodyFormatMessageProperty;

                int statusCode = (int)prop.StatusCode;
                string messageBody = Encoding.UTF8.GetString(ms.ToArray());
                RequestLogUtils.logResponse(statusCode, messageBody);

                // Reinitialize readers and stuff
                ms.Position = 0;

                if (messageBody.StartsWith("\"") && !messageBody.StartsWith("\"<") && propBodyFomat != null && propBodyFomat.Format == WebContentFormat.Json)
                    messageBody = JsonConvert.DeserializeObject<string>(messageBody);
                XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(System.Text.Encoding.UTF8.GetBytes(messageBody), XmlDictionaryReaderQuotas.Max);
                Message newMessage = Message.CreateMessage(reader, int.MaxValue, reply.Version);
                newMessage.Properties.CopyProperties(reply.Properties);
                reply = newMessage;

                writer.Close();
                ms.Close();
                //reader.Close();  //reader 不能close ,否则客户端无法响应
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
        }

    }

    public class InsepctMessageBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new IncomingMessageLogger());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class InspectMessageBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(InsepctMessageBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new InsepctMessageBehavior();
        }
    }

}
