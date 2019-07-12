using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;

namespace DomainModel 
{ 
    public interface IBaseSelectItem 
    {
        int Id { get; set; }
        string DisplayName { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class BaseSelectItem<T> : IBaseSelectItem
        where T : BaseSelectItem<T>
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null) _instance = typeof(T).Intance() as T;
                return _instance;
            }
        }

        public static IEnumerable<T> GetList()
        {
            return Instance.GetInnterList();
        }

        protected virtual IEnumerable<T> GetInnterList()
        {
            return null;
        }

        public int Id
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }
    }
}
