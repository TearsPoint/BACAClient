using System;
using System.Net;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml.Linq;

namespace Base
{
    /// <summary>
    /// 封装和反射相关的一些方法
    /// </summary>
    public static class ReflectionEx
    {
        /// <summary>
        /// 获取成员的全名。
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string FullName(this MemberInfo member)
        {
            if (member == null)
                return "Unknown";

            return string.Concat(member.DeclaringType.FullName, ".", member.Name);
        }

    }
}

namespace Base.Ex
{
    /// <summary>
    /// 封装和反射相关的一些方法
    /// </summary>
    public static class ReflectionEx
    {
        #region 反射处理  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Expression GetOriginalExpression(this Expression instance)
        {
            Expression expr = instance;
            while (expr != null)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                    case ExpressionType.Quote:
                    case ExpressionType.TypeAs:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    default:
                        return expr;
                }
            }
            return instance;
        }

        /// <summary>
        /// 返回是否可变参数。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsParams(this ParameterInfo param)
        {
            if (param == null)
                return false;

            return param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
        }

        /// <summary>
        /// 返回参数集合的条件。
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Predicate(this ParameterInfo[] ps, Func<ParameterInfo[], bool> predicate)
        {
            return predicate(ps);
        }

        /// <summary>
        /// 获取一个指定名称和条件的方法。
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="fromType"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type fromType, string name, Func<MethodInfo, bool> predicate)
        {
            return fromType.GetMember((t, f) => t.GetMethods(f), name, predicate);
        }

        /// <summary>
        /// 获取一个指定名称和条件的属性。
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="fromType"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(Type fromType, string name, Func<PropertyInfo, bool> predicate)
        {
            return fromType.GetMember((t, f) => t.GetProperties(f), name, predicate);
        }
        /// <summary>
        /// 获取一个指定名称和条件的字段。
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static FieldInfo GetField(Type fromType, string name, Func<FieldInfo, bool> predicate)
        {
            return fromType.GetMember((t, f) => t.GetFields(f), name, predicate);
        }

        private static T GetMember<T>(this Type type, Func<Type, BindingFlags, T[]> getMembers, string name, Func<T, bool> predicate)
            where T : MemberInfo
        {
            T[] items = getMembers(type, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

            T member = items.Select(a => new KeyValuePair<NameMappingAttribute, T>(a.GetAttribute<NameMappingAttribute>(), a))
                            .FirstOrDefault(a => a.Key != null && string.Equals(a.Key.Name, name, StringComparison.CurrentCultureIgnoreCase) && predicate(a.Value)).Value
                   ?? items.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.CurrentCultureIgnoreCase) && predicate(a));

            if (member == null)
            {
                string error = string.Format("类型 {0} 找不到成员 {1}", type.FullName, name);
                throw new MemberAccessException(error);
            }

            return member;
        }

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static MemberInfo[] GetMemberInfo(Type type, string member)
        {
            List<MemberInfo> list = new List<MemberInfo>();
            string[] name = member.Split(new char[] { '.' });
            for (int i = 0; i < name.Length; i++)
            {
                MemberInfo memberInfo = GetMember(type, name[i]);
                if (memberInfo == null) return new MemberInfo[0];

                list.Add(memberInfo);
                if (memberInfo is PropertyInfo)
                    type = ((PropertyInfo)memberInfo).PropertyType;
                else if (memberInfo is FieldInfo)
                    type = ((FieldInfo)memberInfo).FieldType;
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回指定成员路径是否指定类型的属性路径。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberPath"></param>
        /// <returns></returns>
        public static bool IsPropertyPath(Type type, string memberPath)
        {
            MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property;
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;

            string[] name = memberPath.Split(new char[] { '.' });
            for (int i = 0; i < name.Length; i++)
            {
                MemberInfo memberInfo = type.GetMemberInherited(name[i], memberTypes, bindingAttr);
                if (memberInfo == null) return false;

                type = memberInfo.GetDataType();
            }
            return true;
        }

        static Regex _propPathRegex = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*([.][_a-zA-Z][_a-zA-Z0-9]*)*$");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberPath"></param>
        /// <returns></returns>
        public static bool IsPropertyPath(string memberPath)
        {
            return _propPathRegex.IsMatch(memberPath);
        }



        /// <summary>
        /// 获取对象指定字段/属性的值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031")]
        public static bool TryGetValue<TValue>(object target, string memberName, out TValue value)
        {
            value = default(TValue);

            if (target == null) return false;

            MemberInfo member = GetMember(target.GetType(), memberName);
            if (member == null) return false;

            try
            {
                value = (TValue)member.GetValue(target);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 从类型中获取指定成员。查找时按照继承层次进行搜索
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="memberType"></param>
        /// <param name="bindings"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInherited(this Type type, string name, MemberTypes memberType, BindingFlags bindings)
        {
            if (type == null) return null;

            MemberInfo member = type.GetMember(name, memberType, bindings).FirstOrDefault();
            if (member != null) return member;

            member = GetMemberInherited(type.BaseType, name, memberType, bindings);
            if (member != null) return member;

            foreach (Type tf in type.GetInterfaces())
            {
                member = GetMemberInherited(tf, name, memberType, bindings);
                if (member != null) return member;
            }

            return null;
        }

        /// <summary>
        /// 获取指定特性标注的成员
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static MemberInfo[] GetMembers<TAttribute>(this Type type) where TAttribute : Attribute
        {
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

            return type.GetMembers<TAttribute>(bindingAttr);
        }

        /// <summary>
        /// 获取指定特性标注的成员
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static MemberInfo[] GetMembers<TAttribute>(this Type type, BindingFlags bindingAttr) where TAttribute : Attribute
        {
            List<MemberInfo> list = new List<MemberInfo>();

            Type attributeType = typeof(TAttribute);

            MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
            AttributeUsageAttribute usage = attributeType.GetAttribute<AttributeUsageAttribute>();
            if (usage != null)
            {
                memberTypes = 0;
                if (IsValidOn(usage.ValidOn, AttributeTargets.Method)) memberTypes |= MemberTypes.Method;
                if (IsValidOn(usage.ValidOn, AttributeTargets.Property)) memberTypes |= MemberTypes.Property;
                if (IsValidOn(usage.ValidOn, AttributeTargets.Field)) memberTypes |= MemberTypes.Field;
            }

            MemberFilter filter = new MemberFilter(MemberFilterByAttribute);

            list.AddRange(type.FindMembers(memberTypes, bindingAttr, filter, attributeType));

            foreach (Type itf in type.GetInterfaces())
            {
                list.AddRange(itf.FindMembers(memberTypes, bindingAttr, filter, attributeType));
            }

            return list.ToArray();
        }


        private static bool IsValidOn(AttributeTargets source, AttributeTargets target)
        {
            return (source & target) == target;
        }

        private static MemberInfo GetMember(Type type, string name)
        {
            MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;
            return type.GetMemberInherited(name, memberTypes, bindingAttr);
        }

        private static MemberInfo[] FindMemberFromInterface(Type type, MemberTypes memberTypes, BindingFlags bindingFlags, MemberFilter filter, FindMemberInfo info)
        {
            List<MemberInfo> list = new List<MemberInfo>();
            foreach (Type tf in type.GetInterfaces())
            {
                MemberInfo[] members = tf.FindMembers(memberTypes, bindingFlags, filter, info);

                if (members.Length > 0) list.AddRange(members.AsEnumerable());
            }
            return list.ToArray();
        }

        private static object InvokeField(object obj, object[] arguments, FieldInfo field)
        {
            if (arguments.Length == 0) return field.GetValue(obj);

            return null;
        }


        private static MemberInfo BuildActualMember(MemberInfo member, Type[] genericArguments)
        {
            if (genericArguments == null || genericArguments.Length == 0)
                return member;
            else
                return ((MethodInfo)member).MakeGenericMethod(genericArguments);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static bool MemberFilterByFindMemberInfo(MemberInfo objMemberInfo, Object objSearch)
        {
            FindMemberInfo info = (FindMemberInfo)objSearch;

            if (!MemberNameIsMatch(objMemberInfo, info.Names)) return false;

            if (objMemberInfo is MethodInfo)
            {
                MethodInfo method = objMemberInfo as MethodInfo;

                if (method.ReturnType == typeof(void)) return false;

                if (!MemberGenericIsMatch(method, info.GenericCount)) return false;

                //if (!MemberParameterIsMatch(method.GetParameters(), info.ArgTypes)) return false;
            }

            return true;
        }

        //private static bool MemberFilterByName(MemberInfo objMemberInfo, Object objSearch)
        //{
        //    return objMemberInfo.Name.Equals((string)objSearch, StringComparison.CurrentCultureIgnoreCase);
        //}

        private static bool MemberFilterByAttribute(MemberInfo objMemberInfo, Object objSearch)
        {
            return objMemberInfo.GetCustomAttributes((Type)objSearch, true).Length > 0;
        }

        private static bool MemberGenericIsMatch(MethodInfo method, int genericCount)
        {
            if (method.IsGenericMethod)
                return method.GetGenericArguments().Length == genericCount;
            else
                return genericCount == 0;
        }

        private static bool MemberNameIsMatch(MemberInfo objMemberInfo, string[] names)
        {
            return Array.IndexOf<string>(names, objMemberInfo.Name.ToLower()) > -1;
        }


        #endregion


        /// <summary>
        /// 返回类型中实现指定类型的类型列表。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementTypes(this Type source, Type type)
        {
            if (source == null) yield break;

            Func<Type, string> getName = a => a.IsGenericType ? a.GetGenericTypeDefinition().FullName + '[' : a.FullName;
            Func<Type, bool> predicate;
            if (type.IsGenericType)
                predicate = a => getName(a).StartsWith(getName(type), StringComparison.CurrentCulture);
            else
                predicate = a => a == type;

            if (type.IsInterface)
            {
                if (predicate(source)) yield return source;

                foreach (Type item in source.GetInterfaces().Where(a => predicate(a)))
                {
                    yield return item;
                }
            }
            else
            {
                Type baseType = source;
                while (baseType != null)
                {
                    if (predicate(baseType)) yield return baseType;

                    baseType = baseType.BaseType;
                }
            }

            yield break;
        }

        /// <summary>
        /// 返回类型关联的元素类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Type FindElementType(this Type source)
        {
            if (source == null) return null;
            Type ienum = FindTheIEnumerable(source);
            if (ienum == null) return source;
            return ienum.GetGenericArguments()[0];
        }


        private static Type FindTheIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindTheIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindTheIEnumerable(seqType.BaseType);
            }

            return null;
        }

        #region MemberInfo 扩展
        /// <summary>
        /// 获取成员的说明。取<see cref="DescriptionAttribute"/>的标注结果。不存在标注时返回string.Empty
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo member)
        {
            if (member == null) return string.Empty;

            DescriptionAttribute da = member.GetAttribute<DescriptionAttribute>();
            if (da != null) return da.Description;

            return string.Empty;
        }
        /// <summary>
        /// 获取成员关联的指定特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <returns>不存在时返回null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return member.GetAttribute<TAttribute>(a => true);
        }

        /// <summary>
        /// 获取成员关联的指定特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member, Func<TAttribute, bool> filter) where TAttribute : Attribute
        {
            if (member == null) return default(TAttribute);

            return member.GetCustomAttributes(typeof(TAttribute), true).CastAs<TAttribute>().FirstOrDefault(a => filter(a));
        }

        /// <summary>
        /// 获取类型包含的指定类型的特性实例
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="source"></param>
        /// <param name="inherit"></param>
        /// <returns>不包含指定特性时返回null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public static TAttribute GetAttribute<TAttribute>(this Type source, bool inherit) where TAttribute : Attribute
        {
            if (source == null) return default(TAttribute);
            return source.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() as TAttribute;
        }


        /// <summary>
        /// 获取成员对应的数据类型
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static Type GetDataType(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return ((PropertyInfo)member).PropertyType;
            else if (member is FieldInfo)
                return ((FieldInfo)member).FieldType;

            return typeof(object);
        }


        /// <summary>
        /// 创建 IList&lt;entityType&gt; 的实例
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static IList CreateGenericList(this Type entityType)
        {
            Type type = typeof(List<>).MakeGenericType(entityType);
            return (IList)Activator.CreateInstance(type);
        }

        /// <summary>
        /// 创建一个指定元素类型的空数组
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static object CreateEmptyArray(this Type entityType)
        {
            return Array.CreateInstance(entityType, 0);
        }


        /// <summary>
        /// 返回 System.Type 类型对应的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Parse("1900-01-01");
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                return Enum.GetValues(type).GetValue(0);
            }
            else if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else if (type.IsArray)
            {
                return CreateEmptyArray(type.GetElementType());
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 数据转换
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="typeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Convert(string typeName, object value)
        {
            return Convert(Runtime.FindTypeInCurrentDomain(typeName), value);
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static object Convert(this Type type, object source)
        {
            try
            {
                if (source == null)
                {
                    if (type.IsValueType) return type.GetDefaultValue();

                    return null;
                }

                Type sourceType = source.GetType();
                if (type.IsAssignableFrom(sourceType)) return source;

                TypeConverter sourceConverter = TypeDescriptor.GetConverter(source);
                TypeConverter targetConverter = TypeDescriptor.GetConverter(type);

                object value = source;
                string str;
                if (type.IsValueType && (str = value as string) != null)
                {
                    if (string.IsNullOrEmpty(str))
                        return Activator.CreateInstance(type);
                }

                if (source is DateTime && (type != typeof(string) && type != typeof(DateTime)))
                    value = ((DateTime)source).ToOADate();
                else if (type == typeof(DateTime))
                {
                    if (sourceType.IsPrimitive)
                        return DateTime.Today.AddDays(System.Convert.ToDouble(source));
                }
                else if (type == typeof(Boolean))
                {
                    if (value is string && ((string)value).IsNumeric())
                    {
                        double dbl = value.Convert<double>();
                        return System.Convert.ToBoolean(dbl);
                    }
                    return System.Convert.ToBoolean(value);
                }

                if (sourceConverter.CanConvertTo(type))
                    return sourceConverter.ConvertTo(value, type);

                if (targetConverter.CanConvertFrom(sourceType))
                    return targetConverter.ConvertFrom(source);

                if (type.IsArray)
                    return value;


                //if (typeof(Enum).IsAssignableFrom(type))
                //    return Enum.Parse(type, value.ToString());

                //if (type == typeof(Binary))
                //{
                //    if (value.GetType() == typeof(byte[])) return new Binary((byte[])value);

                //    return new Binary(new byte[8]);
                //}

                if (type == typeof(System.Xml.Linq.XElement))
                {
                    if (string.IsNullOrEmpty(value.ToString())) return null;

                    return XElement.Parse(value.ToString());
                }

                if (type == typeof(string)) return value.ToString();

                return ConvertWithSystemConvert(type, value);
            }
            catch
            {
                return source;
            }
        }

        private static object ConvertWithSystemConvert(Type type, object value)
        {
            MethodInfo method;

            string targetName = string.Empty;
            if (!type.IsGenericType)
                targetName = type.Name;

            if (type.IsValueType)
            {
                Type bt = Nullable.GetUnderlyingType(type);
                if (bt != null && !bt.IsGenericType)
                {
                    targetName = bt.Name;
                }
            }

            method = typeof(System.Convert).GetMethod("To" + targetName, new Type[] { value.GetType() });

            if (method == null) return value;

            return method.Invoke(null, new object[] { value });
        }


        /// <summary>
        /// 返回源类型是否可转换为目标类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CanConvertTo(this Type source, Type target)
        {
            if (target.IsAssignableFrom(source)) return true;

            if (target == typeof(string)) return true;

            if (TypeDescriptor.GetConverter(source).CanConvertTo(target)) return true;

            if (TypeDescriptor.GetConverter(target).CanConvertFrom(source)) return true;

            if (source.IsEnum) return target.IsPrimitive && target.IsValueType;

            if (target.IsArray) return source.IsArray;

            if (target.IsPrimitive) return source == typeof(string) || source.IsPrimitive;

            if (target.IsValueType) return source == typeof(string) || source.IsValueType;

            return false;
        }


        /// <summary>
        /// 设置字段/属性的值
        /// </summary>
        /// <param name="member"></param>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public static void SetValue(this MemberInfo member, object component, object value)
        {
            if (component == null) return;

            PropertyInfo prop = member as PropertyInfo;
            if (prop != null)
            {
                prop.SetValue(component, prop.PropertyType.Convert(value), null);
                return;
            }

            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                field.SetValue(component, field.FieldType.Convert(value));
                return;
            }
        }

        /// <summary>
        /// 获取字段/属性的值
        /// </summary>
        /// <param name="member"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static object GetValue(this MemberInfo member, object component)
        {
            if (component == null && !member.CanGetFromNullEx()) return null;

            if (member is PropertyInfo)
                return ((PropertyInfo)member).GetValue(component, null);
            else if (member is FieldInfo)
                return ((FieldInfo)member).GetValue(component);

            MethodInfo method = member as MethodInfo;
            if (method != null && typeof(void) != method.ReturnType && method.GetParameters().Length == 0)
            {
                return method.Invoke(component, null);
            }

            return null;
        }

        private static bool CanGetFromNullEx(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return ((PropertyInfo)member).GetGetMethod().IsStatic;
            else if (member is FieldInfo)
                return ((FieldInfo)member).IsStatic;
            else if (member is MethodBase)
                return ((MethodBase)member).IsStatic;
            else
                return false;
        }

        /// <summary>
        /// 获取字段/属性的是否只读
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static bool IsReadOnlyEx(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return !((PropertyInfo)member).CanWrite;
            else if (member is FieldInfo)
                return ((FieldInfo)member).IsInitOnly;

            return true;
        }

        #endregion

        #region help classes
        private struct FindMemberInfo
        {
            public int GenericCount;
            public Type[] ArgTypes;
            public string[] Names;

            public FindMemberInfo(string[] names, Type[] argTypes, int genericCount)
            {
                Names = names.Select(n => n.ToLower()).ToArray();

                if (argTypes == null)
                    ArgTypes = new Type[0];
                else
                    ArgTypes = argTypes.ToArray();

                GenericCount = genericCount;
            }
        }
        #endregion

        #region 表达式相关
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static IList GetMembers(object source, MemberInfo member)
        {
            List<object> list = new List<object>();

            if (source is IList)
            {
                foreach (object item in (IList)source)
                {
                    list.Add(GetValue(item, member));
                }
            }
            else
                list.Add(GetValue(source, member));

            return list;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static object GetValue(object obj, MemberInfo member)
        {
            if (member is PropertyInfo)
                return ((PropertyInfo)member).GetValue(obj, null);
            else if (member is FieldInfo)
                return ((FieldInfo)member).GetValue(obj);

            return null;
        }

        /// <summary>
        /// 获取实体表达式对应的成员路径
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string GetMemberPath<TEntity, TValue>(this Expression<Func<TEntity, TValue>> member)
        {
            if (member == null) return string.Empty;

            return BuildMemberPath(GetMemberInfo(member));
        }

        /// <summary>
        /// 获取实体表达式对应的成员路径
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetMemberPath(this LambdaExpression member)
        {

            return member.IsPropertyPath() ? BuildMemberPath(GetMemberInfo(member)) : string.Empty;
        }

        /// <summary>
        /// 返回表达式是否成员路径。
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsPropertyPath(this LambdaExpression expression)
        {
            Guard.Assert(expression.Parameters.Count == 1, "表达式只能有一个参数。");
            Guard.Assert(expression.Body.Type != typeof(void), "表达式必须有返回值。");

            return DoGetMemberInfo(expression, false).Length > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetMemberPath<T>(Expression<Func<T, object>> member)
        {
            return BuildMemberPath(GetMemberInfo(member));
        }

        /// <summary>
        /// 获取实体成员 lambda 表达式包含的成员反射信息。
        /// </summary>
        /// <exception cref="ArgumentException">在表达式参数不等于1，或包含非成员表达式时触发</exception>
        /// <param name="member"></param>
        /// <returns></returns>
        public static MemberInfo[] GetMemberInfo(this LambdaExpression member)
        {
            if (member.Parameters.Count != 1)
                throw new ArgumentException("源表达式参数个数不等于1");

            return DoGetMemberInfo(member, true);
        }

        /// <summary>
        /// 取表达式对应的成员。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static MemberInfo GetMember<T>(Expression<Func<T, object>> member)
        {
            return DoGetMemberInfo(member, false).FirstOrDefault();
        }

        /// <summary>
        /// 将一个单级属性成员的表达式转换为属性信息。
        /// </summary>
        /// <exception cref="ArgumentNullException">member</exception>
        /// <exception cref="ArgumentException"></exception>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="member"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static PropertyInfo ToProperty<T, TKey>(this Expression<Func<T, TKey>> member, string description)
        {
            PropertyInfo prop = member.ToPropertyOrField(description) as PropertyInfo;

            if (prop == null)
                throw new ArgumentException(string.Format("{0}只能是属性成员", description));

            return prop;
        }

        /// <summary>
        /// 将一个单级属性成员的表达式转换为属性或字段信息。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="member"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static MemberInfo ToPropertyOrField<T, TKey>(this Expression<Func<T, TKey>> member, string description)
        {
            Guard.ArgumentNotNull(member, "member");

            MemberInfo[] members = member.GetMemberInfo();
            if (members.Length != 1) throw new ArgumentException(string.Format("{0}不能是下级成员", description));

            if (members[0].MemberType != MemberTypes.Property && members[0].MemberType != MemberTypes.Field)
                throw new ArgumentException(string.Format("{0}只能是属性或字段成员", description));

            return members[0];
        }

        private static MemberInfo[] DoGetMemberInfo(System.Linq.Expressions.Expression expr, bool withExtensionMethod)
        {
            List<MemberInfo> list = new List<MemberInfo>();
            while (expr != null)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                    case ExpressionType.Quote:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpr = (MemberExpression)expr;
                        list.Add(memberExpr.Member);
                        expr = memberExpr.Expression;
                        break;
                    case ExpressionType.Call:
                        MethodCallExpression methodCallExpression = (MethodCallExpression)expr;
                        list.Add(methodCallExpression.Method);

                        expr = methodCallExpression.Object;

                        //扩展方法处理
                        if (expr == null && methodCallExpression.Method.IsStatic && methodCallExpression.Arguments.Count > 0)
                        {
                            if (withExtensionMethod)
                                expr = methodCallExpression.Arguments[0];
                            else
                                return new MemberInfo[0];
                        }
                        break;
                    case ExpressionType.Parameter:
                        expr = null;
                        break;
                    default:
                        return new MemberInfo[0];
                }
            }

            return list.Reverse<MemberInfo>().ToArray();
        }

        /// <summary>
        /// 转换为成员反射信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<PropertyInfo> ToProperties<TEntity>(this IEnumerable<Expression<Func<TEntity, object>>> source)
        {
            Expression<Func<TEntity, object>>[] members = source.ToArray();

            List<PropertyInfo> infos = new List<PropertyInfo>();
            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo[] mis = members[i].GetMemberInfo();
                if (mis.Length == 1 && mis[0] is PropertyInfo)
                    infos.Add((PropertyInfo)mis[0]);
            }
            return infos;
        }

        private static string BuildMemberPath(MemberInfo[] members)
        {
            if (members.Length == 0)
                return null;

            if (members.Length == 1)
                return members[0].Name;

            List<string> list = new List<string>();
            foreach (MemberInfo info in members)
            {
                list.Add(info.Name);
            }
            return string.Join(".", list.ToArray());
        }

        /// <summary>
        /// 获取表达式的结果类型。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Type GetResultType(this System.Linq.Expressions.Expression instance)
        {
            ConstantExpression ce = instance as ConstantExpression;
            if (ce != null && ce.Value is Type)
                return ce.Value as Type;
            else
                return instance.Type;
        }
        #endregion
    }

    /// <summary>
    /// 封装参数校验相关的一些功能。
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// 在指定参数为null是触发 <see cref="ArgumentNullException"/> 异常。
        /// </summary>
        /// <exception cref="ArgumentNullException">验证值为空时触发</exception>
        /// <param name="argumentValue">待验证参数值</param>
        /// <param name="argumentName">待验证参数名称</param>
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// 在指定字符参数为null或Emtpy时触发异常。
        /// </summary>
        /// <exception cref="ArgumentNullException">验证值为空时触发</exception>
        /// <exception cref="ArgumentException">验证值为Emtpy时触发</exception>
        /// <param name="argumentValue">待验证参数值</param>
        /// <param name="argumentName">待验证参数名称</param>
        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
            if (argumentValue.Length == 0) throw new ArgumentException("参数不能为空串。", argumentName);
        }

        /// <summary>
        /// 验证目标类型是否可从值类型中分配(即实现了接口，或包含在类的继承层次中).
        /// </summary>
        /// <exception cref="ArgumentNullException">验证值为空时触发</exception>
        /// <exception cref="ArgumentException">在类型不能进行分配时触发</exception>
        /// <param name="assignmentTargetType">待分配的目标类型</param>
        /// <param name="assignmentValueType">供分配的值类型</param>
        /// <param name="argumentName">参数名称</param>
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == null) throw new ArgumentNullException("assignmentTargetType");
            if (assignmentValueType == null) throw new ArgumentNullException("assignmentValueType");

            if (!assignmentTargetType.IsAssignableFrom(assignmentValueType))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "类型 {1} 不能分配到类型 {0}。",
                    assignmentTargetType,
                    assignmentValueType),
                    argumentName);
            }
        }

        /// <summary>
        /// 类型校验。
        /// </summary>
        /// <exception cref="InvalidCastException">在实例不是指定类型时触发</exception>
        /// <exception cref="NullReferenceException">实例为空时触发</exception>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public static void Cast<T>(object source)
        {
            if (source == null) throw new NullReferenceException();

            if (source is T) return;

            throw new InvalidCastException(string.Format("不是{0}类型的对象", typeof(T).Name));
        }

        /// <summary>
        /// 类型校验。
        /// </summary>
        /// <exception cref="InvalidCastException">在实例不是指定类型时触发</exception>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="allowNull"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public static void GuardCast<T>(this object source, bool allowNull) where T : class
        {
            if (source == null)
            {
                if (allowNull) return;
            }

            Cast<T>(source);
        }

        /// <summary>
        /// 校验两实例的类型是否可转换。
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void GuardCast(object first, object second)
        {
            Type type1 = first.GetType();
            Type type2 = second.GetType();

            if (!type1.IsAssignableFrom(type2) && !type2.IsAssignableFrom(type1))
                throw new InvalidCastException(string.Format("类型{0}和{1}不能进行转换", type1.Name, type2.Name));
        }

        /// <summary>
        /// 断言条件是否为true。为false时触发<see cref="ArgumentException"/>异常。
        /// </summary>
        /// <exception cref="ArgumentException">条件为false时触发</exception>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void Assert(bool condition, string message)
        {
            if (!condition) throw new ArgumentException(message);
        }
    }
}
