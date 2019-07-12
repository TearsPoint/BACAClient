using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Base.Ex
{
    public static class CollectionEx
    {

        /// <summary>
        /// 带验证地向集合中加入项目，成功返回true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        public static bool AddVerifiable<T>(this ICollection<T> source, T item)
        {
            lock (source)
            {
                int oldCount = source.Count;
                source.Add(item);
                return source.Count == oldCount + 1;
            }
        }

        /// <summary>
        /// 转换为可枚举类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this T source)
        {
            yield return source;
        }

    }
    
}
