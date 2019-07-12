using System;
using System.Collections;

namespace DomainModel
{
    /// <summary>
    /// ODictionary类，扩展Dictionary对象
    /// </summary>
    public class ODictionary : DictionaryBase 
    {
        // Fields
        private string _table = string.Empty;
        private readonly object syncRoot = new object();
         
        // Methods
        /// <summary>
        /// 在 ODictionary 对象中添加一个带有所提供的键和值的元素,如存在则修改该键的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            lock (this.syncRoot)
            {
                if (this.Contains(key))
                {
                    base.Dictionary[key] = value;
                }
                else
                {
                    base.Dictionary.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 确定 ODictionary 对象是否包含具有指定键的元素。
        /// </summary>
        /// <param name="key">要在 ODictionary 对象中定位的键</param>
        /// <returns>如果 ODictionary 包含带有该键的元素，则为 true；否则为 false。</returns>
        public bool Contains(string key)
        {
            lock (this.syncRoot)
            {
                return base.Dictionary.Contains(key);
            }
        }

        //判断插入对象
        protected override void OnInsert(object key, object value)
        {
            if (key.GetType() != typeof(string))
            {
                throw new ArgumentException("key must be of type String.", "key");
            }
            if (string.IsNullOrEmpty(key.ToString()) || (value == null))
            {
                throw new ArgumentException("key,value must be not null.", "key,value");
            }
        }

        //判断移除对象
        protected override void OnRemove(object key, object value)
        {
            if (key.GetType() != typeof(string))
            {
                throw new ArgumentException("key must be of type String.", "key");
            }
        }

        protected override void OnSet(object key, object oldValue, object newValue)
        {
            if (key.GetType() != typeof(string))
            {
                throw new ArgumentException("key must be of type String.", "key");
            }
        }

        protected override void OnValidate(object key, object value)
        {
            if (key.GetType() != typeof(string))
            {
                throw new ArgumentException("key must be of type String.", "key");
            }
        }

        /// <summary>
        /// 从 ODictionary 对象中移除带有指定键的元素。
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            lock (this.syncRoot)
            {
                base.Dictionary.Remove(key);
            }
        }

        // Properties
        public object this[string key]
        {
            get
            {
                lock (this.syncRoot)
                {
                    return base.Dictionary[key];
                }
            }
            set
            {
                lock (this.syncRoot)
                {
                    base.Dictionary[key] = value;
                }
            }
        }

        public ICollection Keys
        {
            get
            {
                lock (this.syncRoot)
                {
                    return base.Dictionary.Keys;
                }
            }
        }

        public string TableName
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this._table;
                }
            }
            set
            {
                lock (this.syncRoot)
                {
                    this._table = value;
                }
            }
        }

        public ICollection Values
        {
            get
            {
                lock (this.syncRoot)
                {
                    return base.Dictionary.Values;
                }
            }
        }
    }
}
