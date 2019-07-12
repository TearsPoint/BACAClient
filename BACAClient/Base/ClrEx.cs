using System;
using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;

namespace Base.Ex
{
    public static class ClrEx
    {

        /// <summary>
        /// 将 IEnumerable 的元素转换为指定的类型。不返回不能转换的元素
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<TResult> CastAs<TResult>(this IEnumerable source) where TResult : class
        {
            if (source == null)
                yield break;
            else
                foreach (var item in source)
                {
                    if (item is TResult) yield return item as TResult;
                }
        }
    }


    public static class TypeEx
    {
        //private static Dictionary<string, Type> _typeAlias;
        private static readonly MethodInfo _methodGenericConvertTo;
        private static readonly MethodInfo _methodToStringEx;

        static TypeEx()
        {
            _methodGenericConvertTo = ReflectionEx.GetMethod(typeof(TypeEx), "ConvertTo", a => a.IsGenericMethod && a.GetParameters().Length == 1);
            _methodToStringEx = ReflectionEx.GetMethod(typeof(StringEx), "ToStringEx", a => a.GetParameters().Length == 1);
        }
        /// <summary>
        /// 返回指定类型是否数值类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        /// <summary>
        /// 返回指定类型是否有符合的整型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        /// <summary>
        /// 返回指定类型是否无符合的整型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        } 

        static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// 返回指定值类型是否可空类型。非值类型返回false。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(Type type)
        {
            if (type == null)
                return false;

            if (type.IsValueType)
                return Nullable.GetUnderlyingType(type) != null;

            return false;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// 返回源类型是否兼容目标类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// 返回指定类型是否枚举类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        /// <summary>
        /// 返回指定类型及其基类型列表。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                if (type.IsInterface)
                    types.Add(type);

                foreach (Type t in type.GetInterfaces())
                    AddInterface(types, t);
            }
        }

        /// <summary>
        /// 表达式转换。不能转换时返回null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Expression Convert(Expression source, Type targetType)
        {
            MethodInfo method;
            if (source == null || targetType == null || source.Type == targetType)
            {
                return source;
            }

            Type sourceType = source.Type;
            if (sourceType.IsValueType && (targetType == typeof(object) || IsNullable(targetType)))
            {
                return Expression.TypeAs(source, targetType);
            }
            else if (targetType.IsAssignableFrom(sourceType))
            {
                return source;
            }
            else if (targetType == typeof(string))
            {
                return ConvertToString(source);
            }
            else if (sourceType.CanConvertTo(targetType))
            {
                method = TypeEx._methodGenericConvertTo.MakeGenericMethod(targetType);
                if (sourceType.IsValueType)
                    return Expression.Call(null, method, Expression.Convert(source, typeof(object)));
                else
                    return Expression.Call(null, method, source);
            }
            else if (sourceType == typeof(object))
            {
                return Convert(ConvertToString(source), targetType);
            }
            else
            {
                return null;
            }
        }
        
        private static MethodInfo GetToStringMethod(Type type)
        {
            return ReflectionEx.GetMethod(type, "ToString", a => a.GetParameters().Length == 0);
        }

        /// <summary>
        /// 将表达式转换为结果为string的表达式。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Expression ConvertToString(Expression source)
        {
            if (source.Type.IsValueType)
                return Expression.Call(source, GetToStringMethod(source.Type));
            else
                return Expression.Call(null, _methodToStringEx, source);
        }

    }
}
