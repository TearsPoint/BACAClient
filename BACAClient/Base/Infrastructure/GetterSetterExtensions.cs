using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JetSun.Base.Infrastructure
{
    public static class DefaultValueHelper
    {
        public static object GetDefaultValue(Type parameter)
        {
            Type defaultGenerator = typeof(DefaultGenerator<>).MakeGenericType(parameter);
            return defaultGenerator.InvokeMember("GetDefaultValue", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[0]);
        }

        private static class DefaultGenerator<T>
        {
            static DateTime _DefaultDateTime = new DateTime(1900, 1, 1);
            public static object GetDefaultValue()
            {
                if (typeof(T) == typeof(DateTime)) return _DefaultDateTime;
                if (typeof(T) == typeof(string)) return string.Empty;
                return default(T);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IGetValue
    {
        object Get(object entity);
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ISetValue
    {
        void Set(object target, object value);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class StaticGetterWrapper<TValue> : IGetValue
    {
        private Func<TValue> _getter;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        public StaticGetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetGetMethod(true);
            _getter = (Func<TValue>)Delegate.CreateDelegate(typeof(Func<TValue>), null, m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _getter();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        object IGetValue.Get(object entity)
        {
            return GetValue();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class StaticSetterWrapper<TValue> : ISetValue
    {
        private Action<TValue> _setter;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        public StaticSetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TValue>)Delegate.CreateDelegate(typeof(Action<TValue>), null, m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(TValue value)
        {
            _setter(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void Set(object target, object value)
        {
            SetValue((TValue)value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SetterWarrper<TTarget, TValue> : ISetValue
    {
        Action<TTarget, TValue> _setter;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        public SetterWarrper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void SetValue(TTarget target, TValue value)
        {
            _setter(target, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        void ISetValue.Set(object target, object value)
        {
            SetValue((TTarget)target, (TValue)value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class GetterWarrper<TTarget, TValue> : IGetValue
    {
        Func<TTarget, TValue> _getter;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        public GetterWarrper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetGetMethod(true);
            _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public TValue GetValue(TTarget target)
        {
            return _getter(target);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        object IGetValue.Get(object entity)
        {
            return GetValue((TTarget)entity);
        }
    }

    public class DynamicMethodFactory
    {
        private static ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, IGetValue>> _typeGetterRepository = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, IGetValue>>();
        private static ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, ISetValue>> _typeSetterRepository = new ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, ISetValue>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IGetValue GetGetter(PropertyInfo property)
        {
            RegistertGetterType(property.ReflectedType);
            var repository = GetGetRepository(property.ReflectedType);
            return repository.GetOrAdd(property, (p) => { return null; });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ISetValue GetSetter(PropertyInfo property)
        {
            RegistertSetterType(property.ReflectedType);
            var repository = GetSetRepository(property.ReflectedType);
            return repository.GetOrAdd(property, (p) => { return null; });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ConcurrentDictionary<PropertyInfo, IGetValue> GetGetRepository(Type type)
        {
            return _typeGetterRepository.GetOrAdd(type, (t) => { return new ConcurrentDictionary<PropertyInfo, IGetValue>(); });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ConcurrentDictionary<PropertyInfo, ISetValue> GetSetRepository(Type type)
        {
            return _typeSetterRepository.GetOrAdd(type, (t) => { return new ConcurrentDictionary<PropertyInfo, ISetValue>(); });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public static void RegistertSetterType(Type type)
        {
            if (_typeSetterRepository.ContainsKey(type)) return;
            var setRepository = GetSetRepository(type);
            foreach (var property in type.GetProperties())
            {
                RegistertSetter(setRepository, property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public static void RegistertGetterType(Type type)
        {
            if (_typeGetterRepository.ContainsKey(type)) return;
            var getRepository = GetGetRepository(type);
            foreach (var property in type.GetProperties())
            {
                RegistertGetter(getRepository, property);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getRepository"></param>
        /// <param name="property"></param>
        private static void RegistertGetter(ConcurrentDictionary<PropertyInfo, IGetValue> getRepository, PropertyInfo property)
        {
            var warrper = property.CreatePropertyGetValueWarrper();
            if (warrper == null) return;
            getRepository.GetOrAdd(property, warrper);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setRepository"></param>
        /// <param name="property"></param>
        public static void RegistertSetter(ConcurrentDictionary<PropertyInfo, ISetValue> setRepository, PropertyInfo property)
        {
            var warrper = property.CreatePropertySetValueWarrper();
            if (warrper == null) return;
            setRepository.GetOrAdd(property, warrper);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ISetValue CreatePropertySetValueWarrper(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("PropertyInfo");

            if (!property.CanWrite)
                throw new NotSupportedException(string.Format("{0}属性不支持写操作。", property.Name));

            var mi = property.GetGetMethod(true);
            if (mi.GetParameters().Length > 1)
                throw new NotSupportedException("不支持构造索引器属性的委托。");
            if (mi.IsStatic)
            {
                Type instanceType = typeof(StaticSetterWrapper<>).MakeGenericType(property.PropertyType);
                return (ISetValue)Activator.CreateInstance(instanceType, property);
            }
            else
            {
                Type delType = typeof(SetterWarrper<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
                return Activator.CreateInstance(delType, property) as ISetValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IGetValue CreatePropertyGetValueWarrper(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("PropertyInfo");

            if (!property.CanRead)
                throw new NotSupportedException(string.Format("{0}属性不支持读操作。", property.Name));

            var mi = property.GetGetMethod(true);
            if (mi.GetParameters().Length > 1)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            Type delType = typeof(GetterWarrper<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            return Activator.CreateInstance(delType, property) as IGetValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object FastGet(this PropertyInfo property, object entity)
        {
            if (property == null) throw new ArgumentNullException("FastGetValue.property");

            return DynamicMethodFactory.GetGetter(property).Get(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="entity"></param>
        public static void FastSet(this PropertyInfo property, object entity, object val)
        {
            if (property == null) throw new ArgumentNullException("FastSetValue.property");

            DynamicMethodFactory.GetSetter(property).Set(entity, val);
        }
    }
}
