using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Base.Ex;

namespace Base
{
    /// <summary>
    /// 指定可管理的命名属性
    /// </summary>
    [DataContract]
    public enum ManagedProperty
    {
        /// <summary>
        /// 未知的
        /// </summary>
        [EnumMember]
        Unknown,
        /// <summary>
        /// 主键
        /// </summary>
        [EnumMember]
        Key,
        /// <summary>
        /// 名称
        /// </summary>
        [EnumMember]
        Name,
        /// <summary>
        /// 编码
        /// </summary>
        [EnumMember]
        Code,
        ///// <summary>
        ///// 助记符
        ///// </summary>
        //[EnumMember]
        //MnemonicCode,
        /// <summary>
        /// 拼音码
        /// </summary>
        [EnumMember]
        SpellCode,
        /// <summary>
        /// 五笔码
        /// </summary>
        [EnumMember]
        WBCode,
        /// <summary>
        /// 状态
        /// </summary>
        [EnumMember]
        Status,
        /// <summary>
        /// 工作流
        /// </summary>
        [EnumMember]
        Workflow,
    }

    /// <summary>
    /// 标注一个类型的默认的显示成员。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]    
    public class DisplayMemberAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public DisplayMemberAttribute(ManagedProperty property)
        {
            this.Member = property;
        }
        /// <summary>
        /// 
        /// </summary>
        public ManagedProperty Member { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ManagedProperty GetDisplayMember(Type type)
        {
            DisplayMemberAttribute dma = type.GetAttribute<DisplayMemberAttribute>(true);
            if (dma == null)
                return ManagedProperty.Name;
            else
                return dma.Member;
        }
    }
}
