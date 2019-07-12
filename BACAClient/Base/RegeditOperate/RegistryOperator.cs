using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Base.RegeditOperate
{
    public class RegistryOperator
    {
        public static readonly string RegisterRoot = @"HKEY_LOCAL_MACHINE\SOFTWARE\BACAClient\1.0";

        /// <summary>
        /// 获取注册表项的值
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object GetValue(string keyName,string valueName,object defaultValue)
        {
            return Registry.GetValue(keyName, valueName, defaultValue);
        }

        /// <summary>
        /// 返回限定类型的注册表项值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(string keyPath,T defaultValue)
        {
            string keyName;
            string valueName;
            SplitKey(keyPath, out keyName, out valueName);
            object value = GetValue(keyName, valueName, null);

            if (value != null) return (T)value;
            else return default(T);
        }

        /// <summary>
        /// 保存注册表
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="value"></param>
        public static void SetValue(string keyPath, object value)
        {
            string keyName;
            string valueName;
            SplitKey(keyPath, out keyName, out valueName);
            SetValue(keyName, valueName, value);
        }

        /// <summary>
        /// 保存注册表
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        public static void SetValue(string keyName, string valueName, object value)
        {
            Registry.SetValue(keyName, valueName, value);
        }

        /// <summary>
        /// 拆分路径
        /// </summary>
        /// <param name="keyPath">完整路径</param>
        /// <param name="keyName">不包括键名的路径</param>
        /// <param name="valueName">键名</param>
        public static void SplitKey(string keyPath, out string keyName, out string valueName)
        {
            int position = keyPath.LastIndexOf("\\");
            keyName = keyPath.Substring(0, position);
            valueName = keyPath.Substring(position + 1);
        }

        /// <summary>
        /// 返回 .NETFramework 的安装路径
        /// </summary>
        public static string NETFrameworkInstallRoot
        {
            get
            {
                return GetValue<String>(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\InstallRoot", string.Empty);
            }
        }
    }
}
