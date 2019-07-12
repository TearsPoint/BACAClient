using System;
using System.Net; 

namespace Base
{
    /// <summary>
    /// 表示类型、成员的名称的映射名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class NameMappingAttribute : Attribute
    {
        internal static readonly NameMappingAttribute Empty = new NameMappingAttribute(string.Empty);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public NameMappingAttribute(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 返回映射的名称。
        /// </summary>
        public string Name { get; private set; }
    }
}
