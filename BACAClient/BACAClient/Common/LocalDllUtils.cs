using BACAClient.Model;
using Base;
using Base.DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BACAClient.Common
{
    public class LocalDllUtils
    {
        public static bool dllInfoToXML(DllInfo model)
        {
            try
            {
                string path = GetPath();
                if (File.Exists(path))
                {
                    XmlHelper.RemoveChildNodes(path);
                }
                else
                {
                    XmlHelper.CreateXmlByPath(path);
                }
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlNode node = document.SelectSingleNode("response");
                XmlNode newChild = document.CreateElement("record");
                node.AppendChild(newChild);
                PropertyInfo[] properties = typeof(DllInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo info in properties)
                {
                    if (info != null)
                    {
                        string str2 = string.Empty;
                        if (info.GetValue(model, null) != null)
                        {
                            str2 = info.GetValue(model, null).ToString();
                        }
                        XmlNode node3 = document.CreateElement(info.Name);
                        node3.InnerText = str2;
                        newChild.AppendChild(node3);
                    }
                }
                document.Save(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DllInfo GetModelByProcessArguments()
        {
            try
            {
                string str = Console.ReadLine().ToUpper(CultureInfo.InvariantCulture);
                DllInfo model = new DllInfo();
                if (string.IsNullOrEmpty(str))
                {
                    model.IsOpen = -1;
                }
                else
                {
                    char[] separator = new char[] { '+' };
                    string[] strArray = str.Split(separator);
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        model.UserId = strArray[0];
                        model.UserName = strArray[1];
                        model.DepId = strArray[2];
                        model.DepName = strArray[3];
                        model.TypeId = int.Parse(strArray[4]);
                        model.KeyWord = strArray[5];
                        model.IsOpen = int.Parse(strArray[6]);
                    }
                    else
                    {
                        return null;
                    }
                }
                if (model.IsOpen != 1)
                {
                    dllInfoToXML(model);
                }
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DllInfo GetModelByXML()
        {
            try
            {
                string path = GetPath();
                DataSet set = new DataSet();
                if (File.Exists(path))
                {
                    set.ReadXml(path);
                }
                File.Delete(path);
                if ((set.Tables.Count > 0) && (set.Tables[0].Rows.Count > 0))
                {
                    List<DllInfo> list = set.Tables[0].ToList<DllInfo>();
                    if (list == null)
                    {
                        return null;
                    }
                    return list[0];
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetPath()
        {
            try
            {
                return string.Format(@"{0}\dllInfo.xml", Runtime.ExecutablePathParent);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
