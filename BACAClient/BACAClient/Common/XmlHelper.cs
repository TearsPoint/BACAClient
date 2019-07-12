using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BACAClient.Common
{
    public class XmlHelper
    {
        private static StreamWriter writer = null;

        public static void Create(string path)
        {
            StreamWriter writer = null;
            try
            {
                if (!File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    writer = new StreamWriter(info.FullName, true);
                    writer.WriteLine("<?xml version='1.0' encoding='utf-8'?>");
                    writer.WriteLine("<response>");
                    writer.WriteLine("</response>");
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static string CreateXml(string directory)
        {
            try
            {
                string path = GetDirectory(directory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = GetPath(path, DateTime.Now.ToString("yyyy-MM-dd"));
                Create(str2);
                return str2;
            }
            catch
            {
                if (writer != null)
                {
                    writer.Close();
                }
                return string.Empty;
            }
        }

        public static string CreateXmlByPath(string path)
        {
            try
            {
                Create(path);
                return path;
            }
            catch
            {
                if (writer != null)
                {
                    writer.Close();
                }
                return string.Empty;
            }
        }

        public static string GetDirectory(string directory)
        {
            try
            {
                string str = string.Format(@"{0}\Local", System.Windows.Forms.Application.StartupPath);
                return string.Format(@"{0}\{1}", str, directory);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetPath(string directory, string xmlname)
        {
            try
            {
                return string.Format(@"{0}\{1}.xml", directory, xmlname);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void RemoveChildNodes(string path)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                foreach (XmlNode node in document.GetElementsByTagName("response"))
                {
                    node.ParentNode.RemoveChild(node);
                    break;
                }
                document.Save(path);
            }
            catch (Exception)
            {
            }
        }
    }

}
