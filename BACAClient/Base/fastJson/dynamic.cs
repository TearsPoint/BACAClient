
using Base;
using Base.Ex;
using Base.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace fastJSON
{
    public class DynamicJson : DynamicObject, IEnumerable, IEntityWithKey
    {
        private IDictionary<string, object> _dictionary { get; set; }
        private IDictionary<string, object> _upperKeyDictionary
        {
            get
            {
                return _dictionary.Select(a => new { Key = a.Key.ToUpper(), a.Value }).ToDictionary(a => a.Key, a => a.Value);
            }
        }
        private List<object> _list { get; set; }

        public DynamicJson(string json)
        {
            var parse = fastJSON.JSON.Parse(json);

            if (parse is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)parse;
            else
                _list = (List<object>)parse;
        }

        private DynamicJson(object dictionary)
        {
            if (dictionary is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)dictionary;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys.ToList();
        }

        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result)
        {
            var index = indexes[0];
            if (index is int)
            {
                result = _list[(int)index];
            }
            else
            {
                result = _dictionary[(string)index];
            }
            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                if (_dictionary.TryGetValue(binder.Name, out result) == false)
                    if (_dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                        if (_upperKeyDictionary.TryGetValue(binder.Name.ToUpper(), out result) == false)
                            throw new Exception("property not found " + binder.Name);

                if (result is IDictionary<string, object>)
                {
                    result = new DynamicJson(result as IDictionary<string, object>);
                }
                else if (result is List<object>)
                {
                    List<object> list = new List<object>();
                    foreach (object item in (List<object>)result)
                    {
                        if (item is IDictionary<string, object>)
                            list.Add(new DynamicJson(item as IDictionary<string, object>));
                        else
                            list.Add(item);
                    }
                    result = list;
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var o in _list)
            {
                yield return new DynamicJson(o as IDictionary<string, object>);
            }
        }


        public ICollection<string> Keys
        {
            get
            {
                if (_dictionary == null) return new List<string>();
                return _dictionary.Keys;
            }
        }

        public bool ContainsKey(string propertyName)
        {
            if (_dictionary != null && _dictionary.ContainsKey(propertyName))
                return true;
            else if (_upperKeyDictionary != null && _upperKeyDictionary.ContainsKey(propertyName.ToUpper()))
                return true;
            return false;
        }

        public T Get<T>(string propertyName)
        {
            return Get(propertyName).Convert<T>();
        }

        public string GetStr(string propertyName)
        {
            return Get(propertyName).ToStringEx();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T GetChild<T>(string propertyName)
            where T : class
        {
            object result = default(T);
            if (_dictionary.TryGetValue(propertyName, out result) == false)
                if (_dictionary.TryGetValue(propertyName.ToLower(), out result) == false)
                    return result as T;// throw new Exception("property not found " + binder.Name);

            if (result is IDictionary<string, object>)
            {
                result = new DynamicJson(result as IDictionary<string, object>);
            }
            else if (result is List<object>)
            {
                List<object> list = new List<object>();
                foreach (object item in (List<object>)result)
                {
                    if (item is IDictionary<string, object>)
                        list.Add(new DynamicJson(item as IDictionary<string, object>));
                    else
                        list.Add(item);
                }
                result = list;
            }
            return result as T;
        }

        public static DynamicJson Default()
        {
            return "{}".AsDynamicJson();
        }

        public object Get(string propertyName)
        {
            if (_dictionary != null)
            {
                if (_dictionary.ContainsKey(propertyName))
                    return _dictionary[propertyName];
                if (_upperKeyDictionary.ContainsKey(propertyName.ToUpper()))
                    return _upperKeyDictionary[propertyName.ToUpper()];
            }
            return string.Empty;
        }

        public DynamicJson Set(string propertyName, object value)
        {
            try
            {
                propertyName = _dictionary.Keys.FirstOrDefault(a => a.ToUpper() == propertyName.ToUpper()) ?? propertyName;
                _dictionary[propertyName] = value;
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                return this;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsList { get { return _list != null && _list.Count > 0; } }

        /// <summary>
        /// 
        /// </summary>
        public int PropertyCount { get { if (_dictionary != null) return this._dictionary.Count; else return -1; } }

        public EntityKey EntityKey { get; set; }

        public override string ToString()
        {
            if (_list != null && _list.Count() > 0) return _list.ToJson();
            if (_dictionary != null && _dictionary.Count() > 0) return _dictionary.ToJson();
            return base.ToString();
        }
    }
}