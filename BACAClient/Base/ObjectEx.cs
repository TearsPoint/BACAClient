using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Xml.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.Objects.DataClasses;
using DataService.ServiceLibrary;
using Base.IO;

namespace Base.Ex
{
    /// <summary>
    /// 提供对象的常用方法
    /// </summary>
    public static class ObjectEx
    {
        internal static readonly DateTime ListSerializeAsDataContractFrom = new DateTime(2012, 8, 27);

        static readonly string _machineHashKey;

        static ObjectEx()
        {
            _machineHashKey = ConfigurationManager.AppSettings.Get("machineHashKey") ?? "5CFBE522-FB01-4398-A6D1-8E101C16949E";
        }

        public static void Serialize(Type type, XmlWriter writer, object obj)
        {
            MyXmlSerializer serializer = GetSerializerFromList(type);
            serializer.Serialize(writer, obj);
        }

        public static object Deserialize(Type type, Stream s)
        {
            MyXmlSerializer serializer = GetSerializerFromList(type);
            return serializer.Deserialize(s);
        }

        static object _xmlSerializerListLock = new object();
        static List<MyXmlSerializer> _xmlSerializerList = new List<MyXmlSerializer>();
        private static MyXmlSerializer GetSerializerFromList(Type clrType)
        {
            MyXmlSerializer serializer = null;
            serializer = _xmlSerializerList.FirstOrDefault(a => !a.IsLock && string.Equals(a.type.FullName, clrType.FullName, StringComparison.CurrentCultureIgnoreCase));

            if (serializer == null)
            {
                serializer = new MyXmlSerializer(clrType);
                lock (_xmlSerializerListLock)
                {
                    _xmlSerializerList.Add(serializer);
                }
            }
            return serializer;
        }

        #region 序列化
        /// <summary>
        /// 返回类型对应的<see cref="XmlObjectSerializer"/>实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XmlObjectSerializer GetXmlObjectSerializer(Type type)
        {
            if (type.GetAttribute<NetDataContractFormatAttribute>(true) != null)
                return new NetDataContractSerializer();
            else if (type.GetAttribute<DataContractAttribute>(true) != null
                || type.FindElementType().GetAttribute<DataContractAttribute>(true) != null)
                return new DataContractSerializer(type);
            else
                return new NetDataContractSerializer();
        }

        /// <summary>
        /// 从xml序列化数据中恢复对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public static T ReadObject<T>(string xmlData)
        {
            if (xmlData.IsNullOrWhiteSpace()) return default(T);

            XmlObjectSerializer serializer = ObjectEx.GetXmlObjectSerializer(typeof(T));

            using (StringReader strread = new StringReader(xmlData))
            {
                using (XmlReader reader = XmlReader.Create(strread))
                {
                    return (T)serializer.ReadObject(reader);
                }
            }
        }

        /// <summary>
        /// 返回一个对象的序列化流
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MemoryStream Serialized(object source)
        {
            if (source == null) return null;

            XmlObjectSerializer serializer = ObjectEx.GetXmlObjectSerializer(source.GetType());
            return Serialized(source, serializer);
        }

        /// <summary>
        /// 返回一个对象的序列化流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static MemoryStream Serialized(object source, XmlObjectSerializer serializer)
        {
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, source);
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// 返回一个对象的XML序列化字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SerializeAsXml(object source)
        {
            XmlObjectSerializer serializer = ObjectEx.GetXmlObjectSerializer(source.GetType());
            return SerializeAsXml(source, serializer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static string SerializeAsXml(object source, XmlObjectSerializer serializer)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter write = XmlWriter.Create(sb))
            {
                serializer.WriteObject(write, source);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将一个流反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Deserialized<T>(Stream source) where T : class
        {
            object obj = Deserialized(source, typeof(T));
            return obj as T;
        }

        /// <summary>
        /// 将一个流反序列化为指定类型的对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialized(Stream source, Type type, bool hideException)
        {
            if (source == null) return null;

            if (type == typeof(XElement))
            {
                source.Position = 0;
                using (XmlReader reader = XmlReader.Create(source))
                {
                    return XElement.Load(reader);
                }
            }
            else
            {
                XmlObjectSerializer serializer = ObjectEx.GetXmlObjectSerializer(type);
                return Deserialized(source, serializer, hideException);
            }
        }

        /// <summary>
        /// 将一个流反序列化为指定类型的对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialized(Stream source, Type type)
        {
            return Deserialized(source, type, true);
        }

        /// <summary>
        /// 将一个字节块反序列化为指定类型的对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialized(byte[] source, Type type)
        {
            using (MemoryStream stream = new MemoryStream(source))
            {
                return Deserialized(stream, type);
            }
        }
        /// <summary>
        /// 将一个字节块反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Deserialized<T>(byte[] source) where T : class
        {
            return Deserialized(source, typeof(T)) as T;
        }
        /// <summary>
        /// 将一个流反序列化为指定类型的对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static object Deserialized(Stream source, XmlObjectSerializer serializer, bool hideException)
        {
            try
            {
                source.Position = 0;
                return serializer.ReadObject(source);
            }
            catch (Exception ex)
            {
                Loger.Log("ObjectEx", "Deserialized", ex);
                if (!hideException)
                    throw ex;
                return null;
            }
        }

        /// <summary>
        /// 将一个流反序列化为指定类型的对象
        /// </summary>
        public static object Deserialized(Stream source, XmlObjectSerializer serializer)
        {
            return Deserialized(source, serializer, true);
        }

        /// <summary>
        /// 将一个xml记录反序列化为指定类型的对象
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialized(string xml, Type type)
        {
            XmlObjectSerializer serializer = ObjectEx.GetXmlObjectSerializer(type);

            return Deserialized(xml, serializer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static object Deserialized(string xml, XmlObjectSerializer serializer)
        {
            if (xml.IsNullOrWhiteSpace()) return null;

            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                return serializer.ReadObject(reader);
            }
        }

        /// <summary>
        /// 块缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="toStream"></param>
        /// <param name="blockSize"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static int SerializeByBlock<T>(ICollection<T> items, Stream toStream, int blockSize, bool isCompressed) where T : class
        {
            IList blkItems;
            int i = 0;
            if (blockSize <= 0) blockSize = int.MaxValue;

            while (true)
            {
                blkItems = items.Skip(i * blockSize).Take(blockSize).ToList();
                if (blkItems.Count == 0) break;

                using (MemoryStream ms = ObjectEx.Serialized(blkItems))
                {
                    if (isCompressed)
                    {
                        using (MemoryStream st = FileOperator.Compressed(ms))
                        {
                            AppendBlock(toStream, st.ToArray());
                        }
                    }
                    else
                    {
                        AppendBlock(toStream, ms.ToArray());
                    }
                }
                i++;
            }
            return i;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="buffer"></param>
        internal static void AppendBlock(Stream fs, byte[] buffer)
        {
            if (fs == null) return;

            //try
            //{
            byte[] lb = BitConverter.GetBytes(buffer.Length);
            fs.Write(lb, 0, lb.Length);

            fs.Write(buffer, 0, buffer.Length);
            //}
            //catch (Exception ex)
            //{
            //    HandleException(ex, string.Empty);
            //}
        }

        /// <summary>
        /// 从指定流中反序列化实体列表。
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="fromStream"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static IList<IList> DeserializeByBlock(Type entityType, Stream fromStream, bool isCompressed)
        {
            int count;
            int blen;
            byte[] buffer;
            byte[] lb = new byte[sizeof(int)];

            IList<IList> packages = new List<IList>();
            while (true)
            {
                count = fromStream.Read(lb, 0, lb.Length);
                if (count == 0) break;
                blen = BitConverter.ToInt32(lb, 0);

                buffer = new byte[blen];
                count = fromStream.Read(buffer, 0, buffer.Length);
                if (count == 0) break;

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    if (isCompressed)
                        packages.Add(GetCompressedEntities(ms, entityType));
                    else
                        packages.Add(Deserialized(ms, typeof(IList<>).MakeGenericType(entityType)) as IList);
                }
            }
            return packages;
        }

        /// <summary>
        /// 从压缩的流中读取实体列表。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        public static IList GetCompressedEntities(Stream stream, Type dtoType)
        {
            if (stream == null) return new List<object>();

            MemoryStream data;
            data = FileOperator.Decompressed(stream);
            return ObjectEx.Deserialized(data, typeof(IList<>).MakeGenericType(dtoType)) as IList;
        }
        #endregion

        #region 成员及属性管理
        /// <summary>
        ///  对实例的多个成员同时设置值
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <param name="members"></param>
        public static void SetMemberValue<TObj>(TObj obj, object value, params Expression<Func<TObj, object>>[] members)
             where TObj : class
        {
            foreach (Expression<Func<TObj, object>> member in members)
            {
                //MemberAccessor.Create<TObj>(member).SetValue(obj, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static TItem GetOrCreateItem<T, TItem>(this T source, Expression<Func<T, TItem>> member)
            where T : class
            where TItem : class, new()
        {
            MemberInfo[] props = member.GetMemberInfo();
            object target = source;
            object value = null;
            foreach (MemberInfo pf in props)
            {
                value = pf.GetValue(target);
                if (value == null)
                {
                    value = Activator.CreateInstance(pf.GetDataType());
                    pf.SetValue(target, value);
                }
                target = value;
            }

            return (TItem)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="memberPath"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public static object GetOrCreateItem(object source, string memberPath, bool autoCreate)
        {
            string[] names = memberPath.Split('.');
            object target = source;
            object value = null;
            foreach (string name in names)
            {
                if (target == null) return null;

                MemberInfo pf = target.GetType().GetMember(name).FirstOrDefault();
                if (pf == null) return null;

                value = pf.GetValue(target);
                if (value == null && autoCreate)
                {
                    value = Activator.CreateInstance(pf.GetDataType());
                    pf.SetValue(target, value);
                }
                target = value;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="member"></param>
        /// <param name="items"></param>
        public static void AddItems<T, TItem>(this T source, Expression<Func<T, TItem[]>> member, params TItem[] items)
        {
            MemberInfo pf = member.ToPropertyOrField(string.Empty);
            TItem[] vs = pf.GetValue(source) as TItem[];
            List<TItem> list;
            if (vs == null)
                list = new List<TItem>();
            else
                list = new List<TItem>(vs);

            list.AddRange(items);
            pf.SetValue(source, list.ToArray());
        }


        /// <summary>
        /// 返回对象的元素类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Type FindElementType(object source)
        {
            if (source == null) return null;

            return source.GetType().FindElementType();
        }


        /// <summary>
        /// 将一个对象的属性简单复制另一个对象的同名同类型属性。不支持子属性的处理。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyProperty(object source, object target)
        {
            if (source == null || target == null) return;

            IList<KeyValuePair<PropertyDescriptor, PropertyDescriptor>> descriptors = GetDescriptors(source.GetType(), target.GetType());
            foreach (KeyValuePair<PropertyDescriptor, PropertyDescriptor> kvp in descriptors)
            {
                object obj = kvp.Key.GetValue(source);
                if (obj != null) kvp.Value.SetValue(target, obj);
            }
        }

        /// <summary>
        /// 将同类型的实例的属性简单复制另一个对象的同名同类型属性。支持对属性进行筛选。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="filter"></param>
        public static void CopyProperty<T>(T source, T target, Func<PropertyDescriptor, bool> filter) where T : class
        {
            if (source == null || target == null) return;

            IList<KeyValuePair<PropertyDescriptor, PropertyDescriptor>> descriptors = GetDescriptors(typeof(T), typeof(T));
            foreach (PropertyDescriptor prop in descriptors.Select(a => a.Key).Where(filter))
            {
                object obj = prop.GetValue(source);
                if (obj != null) prop.SetValue(target, obj);
            }
        }

        private static IList<KeyValuePair<PropertyDescriptor, PropertyDescriptor>> GetDescriptors(Type sourceType, Type targetType)
        {
            PropertyDescriptorCollection sources = TypeDescriptor.GetProperties(sourceType, new Attribute[] { new BrowsableAttribute(true) });
            Func<PropertyDescriptor, PropertyDescriptor> matcher;
            if (sourceType == targetType)
            {
                matcher = new Func<PropertyDescriptor, PropertyDescriptor>(delegate(PropertyDescriptor d)
                {
                    if (!d.IsReadOnly) return d;

                    return null;
                });
            }
            else
            {
                PropertyDescriptorCollection targets = TypeDescriptor.GetProperties(targetType, new Attribute[] { new BrowsableAttribute(true) });
                matcher = new Func<PropertyDescriptor, PropertyDescriptor>(delegate(PropertyDescriptor d)
                {
                    PropertyDescriptor match = targets.Find(d.Name, true);
                    if (match != null && !match.IsReadOnly && match.PropertyType.IsAssignableFrom(d.PropertyType)) return match;

                    return null;
                });
            }

            List<KeyValuePair<PropertyDescriptor, PropertyDescriptor>> list = new List<KeyValuePair<PropertyDescriptor, PropertyDescriptor>>();
            foreach (PropertyDescriptor item in sources)
            {
                PropertyDescriptor match = matcher(item);
                if (match != null) list.Add(new KeyValuePair<PropertyDescriptor, PropertyDescriptor>(item, match));
            }
            return list;
        }
        #endregion

        #region 对象遍历

        /// <summary>
        /// 枚举目标实体中指定路径所包含的所有的指定类型的实体。返回 路径－实体 对的列表。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<string, T>> EnumObject<T>(object source, string path) where T : class
        {
            T obj;

            List<KeyValuePair<string, T>> list = new List<KeyValuePair<string, T>>();
            if ((obj = source as T) != null)
                list.Add(new KeyValuePair<string, T>(string.Empty, obj));

            StringBuilder sb = new StringBuilder();
            foreach (var item in path.Split('.'))
            {
                if (source == null) break;

                sb.AppendFormat(".{0}", item);

                PropertyInfo prop = null;
                try
                {
                    prop = source.GetType().GetProperty(item);
                }
                catch
                {
                }
                if (prop == null) break;

                object value = prop.GetValue(source);

                if ((obj = value as T) != null)
                    list.Add(new KeyValuePair<string, T>(sb.ToString(1), obj));

                source = value;
            }

            return list;
        }


        private static IEnumerable<PropertyDescriptor> GetProperties(Type type, Func<PropertyDescriptor, bool> filter)
        {
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(type))
            {
                if (filter(item)) yield return item;
            }
        }


        private interface IGetter
        {
            object GetValue(object entity);
        }
        [Obfuscation(Exclude = true)]
        private class Getter<TEntity, TValue> : IGetter
            where TEntity : class
        {
            Func<TEntity, TValue> _getter;

            public Getter(PropertyInfo prop)
            {
                ParameterExpression param = Expression.Parameter(typeof(TEntity), "a");

                Expression expr = Expression.MakeMemberAccess(param, prop);

                _getter = Expression.Lambda<Func<TEntity, TValue>>(expr, param).Compile();
            }

            #region IGetter 成员

            public object GetValue(object entity)
            {
                TEntity v = entity as TEntity;
                if (v == null) return default(TValue);

                return _getter(v);
            }

            #endregion
        }
        #endregion

        #region 哈希码相关处理
        /// <summary>
        /// 计算指定字节数组的哈希码。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetHashText(byte[] data)
        {
            try
            {
                SHA1Managed algo = new SHA1Managed();
                byte[] hash = algo.ComputeHash(data);

                return ToHashText(hash);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 计算指定 System.IO.Stream 对象的哈希码。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetHashText(Stream stream)
        {
            try
            {
                SHA1Managed algo = new SHA1Managed();
                byte[] hash = algo.ComputeHash(stream);

                return ToHashText(hash);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 计算指定 System.String 对象的哈希码。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetHashText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            byte[] bts = Encoding.Unicode.GetBytes(text);
            return GetHashText(bts);
        }

        /// <summary>
        /// 生成本机的Hash文本。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetLocalHostHashText(string text)
        {
            return GetHashText(text + _machineHashKey);
        }

        private static string ToHashText(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte byt in hash)
                sb.Append(byt.ToString("X2"));

            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// 产生一个随机排列的基于0的索引系列。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] CreateSeries(int count)
        {
            int[] ids = new int[count];

            for (int i = 0; i < count; i++)
            {
                ids[i] = i;
            }

            int j;
            int temp = 0;
            Random rnd = new Random(Environment.TickCount);
            for (int i = 0; i < count; i++)
            {
                j = rnd.Next(count);
                temp = ids[i];
                ids[i] = ids[j];
                ids[j] = temp;
            }
            return ids;
        }

        static object Format(object v)
        {
            if (v is string)
            {
                string s = v as string;
                v = s.Replace("\r\n", "\n");
            }
            return v;
        }

        /// <summary>
        /// 将指定的字节数组转换为 Base 64 编码字节数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToBase64(byte[] data)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(data));
        }

        /// <summary>
        /// 将指定的 Base 64 编码字节数组转换为普通的字节数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] FromBase64(byte[] data)
        {
            return Convert.FromBase64String(Encoding.UTF8.GetString(data));
        }

        private static byte[] Base64Transform(ICryptoTransform transform, byte[] data)
        {
            int inBlkSize = transform.InputBlockSize;
            int outBlkSize = transform.OutputBlockSize;
            int offset = 0;
            byte[] buffer = new byte[outBlkSize];
            using (MemoryStream output = new MemoryStream())
            {
                while (data.Length - offset > inBlkSize)
                {
                    transform.TransformBlock(data, offset, inBlkSize, buffer, 0);

                    offset += inBlkSize;

                    output.Write(buffer, 0, outBlkSize);
                }
                // Transform the final block of data.
                buffer = transform.TransformFinalBlock(data, offset, data.Length - offset);

                output.Write(buffer, 0, buffer.Length);

                return output.ToArray();
            }
        }

        #region XML 相关转换
        static IDictionary<Type, XmlSerializer> _xmlSerializers = new Dictionary<Type, XmlSerializer>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static XmlSerializer GetSerializer(Type type)
        {
            XmlSerializer sz;
            lock (_xmlSerializers)
            {
                if (!_xmlSerializers.TryGetValue(type, out sz))
                {
                    sz = new XmlSerializer(type);
                    _xmlSerializers[type] = sz;
                }
            }
            return sz;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="parseOutXml"></param>
        /// <returns></returns>
        public static T ToObj<T>(this XmlNode node, bool parseOutXml) where T : class
        {
            XmlSerializer sz = GetSerializer(typeof(T));
            string text = parseOutXml ? node.OuterXml : node.InnerXml;
            using (StringReader sr = new StringReader(text))
            {
                object o = sz.Deserialize(sr);
                return o as T;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ToObj<T>(string xml) where T : class
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.ToObj<T>(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument<T>(this T item) where T : class
        {
            XmlSerializer sz = GetSerializer(item.GetType());
            using (MemoryStream st = new MemoryStream())
            {
                sz.Serialize(st, item);
                st.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(st);
                return doc;
            }
        }

        static Dictionary<Type, XmlAttributeOverrides> _xmlOverrides = new Dictionary<Type, XmlAttributeOverrides>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XmlAttributeOverrides GetXmlOverrides(Type type)
        {
            XmlAttributeOverrides overrides;
            if (_xmlOverrides.TryGetValue(type, out overrides))
                return overrides;

            Type t = type.GetImplementTypes(typeof(IList<>)).FirstOrDefault();
            if (t != null)
                t = t.GetGenericArguments()[0];
            else
                t = type;

            if (typeof(EntityObject).IsAssignableFrom(t))
            {
                overrides = new XmlAttributeOverrides();
                overrides.Add(typeof(EntityObject), "EntityKey", new XmlAttributes { XmlIgnore = true });
                List<Type> dones = new List<Type>();
                LoadEdmTypeOverrides(overrides, t, dones);
            }

            _xmlOverrides[type] = overrides;
            _xmlOverrides[t] = overrides;

            return overrides;
        }

        private static void LoadEdmTypeOverrides(XmlAttributeOverrides overrides, Type type, List<Type> dones)
        {
            if (dones.Contains(type))
                return;

            Type ef = typeof(EntityReference);
            Type re = typeof(RelatedEnd);

            dones.Add(type);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (ef.IsAssignableFrom(prop.PropertyType))
                {
                    overrides.Add(type, prop.Name, new XmlAttributes { XmlIgnore = true });
                }
                else if (re.IsAssignableFrom(prop.PropertyType))
                {
                    overrides.Add(type, prop.Name, new XmlAttributes { XmlIgnore = false });

                    Type t = prop.PropertyType.GetImplementTypes(typeof(EntityCollection<>)).FirstOrDefault();
                    if (t != null)
                        LoadEdmTypeOverrides(overrides, t.GetGenericArguments()[0], dones);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] ToXmlArray<T>(this T item) where T : class
        {
            return ToXmlArray(item, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToXmlArray<T>(this T item, Encoding encoding) where T : class
        {
            return ToXmlArray<T>(item, encoding, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static byte[] ToXmlArray<T>(this T item, bool isCompressed) where T : class
        {
            return ToXmlArray(item, Encoding.UTF8, isCompressed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="encoding"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static byte[] ToXmlArray<T>(this T item, Encoding encoding, bool isCompressed) where T : class
        {
            XmlSerializer sz = GetSerializer(item.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    sz.Serialize(writer, item);
                    if (isCompressed)
                        return FileOperator.Compressed(stream).ToArray();
                    else
                        return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ToMsgXml<T>(this T item) where T : class
        {
            if (item == null)
                return null;

            Type t = item.GetType();
            XmlAttributeOverrides overrides = ObjectEx.GetXmlOverrides(t);

            XmlSerializer serializer = new XmlSerializer(t, overrides);
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, XmlEx.WriterSettings))
            {
                serializer.Serialize(writer, item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T item) where T : class
        {
            XmlDocument doc = item.ToXmlDocument();
            return doc.DocumentElement.OuterXml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="encoding"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static T FromXml<T>(byte[] array, Encoding encoding, bool isCompressed) where T : class
        {
            return FromXml(array, encoding, isCompressed, typeof(T)) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="encoding"></param>
        /// <param name="isCompressed"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXml(byte[] array, Encoding encoding, bool isCompressed, Type type)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                return FromXml(stream, type, encoding, isCompressed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T FromXml<T>(byte[] array) where T : class
        {
            return FromXml(array, typeof(T)) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXml(byte[] array, Type type)
        {
            return FromXml(array, type, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="type"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static object FromXml(byte[] array, Type type, bool isCompressed)
        {
            return FromXml(array, Encoding.UTF8, isCompressed, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static object FromXml(Stream stream, Type type, bool isCompressed)
        {
            return FromXml(stream, type, Encoding.UTF8, isCompressed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public static object FromXml(Stream stream, Type type, Encoding encoding, bool isCompressed)
        {
            if (isCompressed)
            {
                using (MemoryStream st = FileOperator.Decompressed(stream))
                {
                    using (StreamReader reader = new StreamReader(st, encoding))
                    {
                        return FromXml(reader, type);
                    }
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return FromXml(reader, type);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T FromXml<T>(Stream stream) where T : class
        {
            return FromXml(stream, typeof(T), false) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T FromXml<T>(string xml) where T : class
        {
            using (TextReader reader = new StringReader(xml))
            {
                return FromXml<T>(reader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T FromXml<T>(TextReader reader) where T : class
        {
            XmlSerializer sz = GetSerializer(typeof(T));
            return sz.Deserialize(reader) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXml(TextReader reader, Type type)
        {
            XmlSerializer sz = GetSerializer(type);
            return sz.Deserialize(reader);
        }
        #endregion

        class MyXmlSerializer
        {
            public MyXmlSerializer(Type clrType)
            {
                type = clrType;
                XmlAttributeOverrides overrides = ObjectEx.GetXmlOverrides(clrType);
                XmlSerializer = new XmlSerializer(clrType, overrides);
                Locker = new object();
                IsLock = false;
            }

            public Type type { get; set; }
            private XmlSerializer XmlSerializer { get; set; }
            public object Locker { get; private set; }
            public bool IsLock { get; set; }

            public void Serialize(XmlWriter writer, object obj)
            {
                IsLock = true;
                try
                {
                    lock (Locker)
                    {
                        XmlSerializer.Serialize(writer, obj);
                    }
                    IsLock = false;
                }
                catch (Exception ex)
                {
                    IsLock = false;
                    throw ex;
                }
            }

            public object Deserialize(Stream s)
            {
                IsLock = true;
                try
                {
                    lock (Locker)
                    {
                        object obj = XmlSerializer.Deserialize(s);
                        IsLock = false;
                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    IsLock = false;
                    throw ex;
                }
            }
        }
    }
}
