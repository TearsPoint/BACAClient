using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACAClient.Common
{
    public class ConfigerHelper
    {
        public static string GetAppSettingsConfig(string strKey)
        {
            try
            {
                foreach (string str in ConfigurationManager.AppSettings)
                {
                    if (str == strKey)
                    {
                        return ConfigurationManager.AppSettings[strKey];
                    }
                }
                return null;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetAutoCloseTimer()
        {
            try
            {
                string configer = GetConfiger(new ConfigerParameterName().AutoCloseTimer);
                if (string.IsNullOrEmpty(configer))
                {
                    configer = SystemEnum.Acquiescence.AutoCloseTimer.ToString();
                }
                return configer;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetConfiger(string Key)
        {
            try
            {
                string str = string.Empty;
                if (ConfigurationManager.AppSettings[Key] != null)
                {
                    str = ConfigurationManager.AppSettings[Key];
                }
                return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetConnectionStringsConfig(string connectionName)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool SetAppSettingsConfig(string newKey, string newValue)
        {
            try
            {
                bool flag = false;
                foreach (string str in ConfigurationManager.AppSettings)
                {
                    if (str == newKey)
                    {
                        flag = true;
                    }
                }
                System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (flag)
                {
                    configuration.AppSettings.Settings.Remove(newKey);
                }
                configuration.AppSettings.Settings.Add(newKey, newValue);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SetConnectionStringsConfig(string newName, string newConString, string newProviderName)
        {
            try
            {
                bool flag = false;
                if (ConfigurationManager.ConnectionStrings[newName] != null)
                {
                    flag = true;
                }
                ConnectionStringSettings settings = new ConnectionStringSettings(newName, newConString, newProviderName);
                System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (flag)
                {
                    configuration.ConnectionStrings.ConnectionStrings.Remove(newName);
                }
                configuration.ConnectionStrings.ConnectionStrings.Add(settings);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("ConnectionStrings");
            }
            catch
            {
            }
        }
    }

}
