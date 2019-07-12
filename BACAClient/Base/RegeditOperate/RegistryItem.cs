using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Base.RegeditOperate
{
    /// <summary>
    /// 注册表项
    /// </summary>
    public class RegistryItem
    {
        public RegistryItem()
        { 
        }

        public RegistryItem(RegistryHive hive,string path,string name)
        {
            this.Hive = hive;
            this.Path = path;
            this.Name = name; 
        }

        /// <summary>
        /// 返回注册表项顶级节点类型
        /// </summary>
        public RegistryHive Hive { get; private set; }

        /// <summary>
        /// 注册表项路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 注册表项名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 返回注册表项的顶级节点
        /// </summary>
        public RegistryKey RootKey { get { return GetRootKey(this.Hive); } }


        private static RegistryKey GetRootKey(RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;
                case RegistryHive.CurrentConfig:
                    return Registry.CurrentConfig;
                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;
                //case RegistryHive.DynData:
                //    return Registry.DynData;
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;
                case RegistryHive.PerformanceData:
                    return Registry.PerformanceData;
                case RegistryHive.Users:
                    return Registry.Users;
                default:
                    return null;
            }
        }
    }
}

