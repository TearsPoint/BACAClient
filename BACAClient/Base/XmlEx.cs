using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;
using Base.Ex;

namespace Base
{
    public static class XmlEx
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            Indent = true,
            //NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = true,
            NewLineOnAttributes = false,
            CloseOutput = true,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static XElement GetElement(this XElement root, params string[] names)
        {
            XElement elt = null;
            XElement parent = root;
            foreach (string n in names)
            {
                elt = null;
                foreach (XElement x in GetElements(parent, n))
                {
                    elt = x;
                    break;
                }
                if (elt == null)
                {
                    return null;
                }
                parent = elt;
            }
            return elt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IList<XElement> GetElements(this XElement elt, string name)
        {
            List<XElement> items = new List<XElement>();
            if (elt != null)
            {
                foreach (XElement x in elt.Elements())
                {
                    if ((x.Name != null) && string.Equals(x.Name.LocalName, name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        items.Add(x);
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(this XElement element, string name, T defaultValue)
        {
            XAttribute item = element.FindAttribute(name);
            if (item == null)
                return defaultValue;

            return item.Value.Convert<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XAttribute FindAttribute(this XElement element, string name)
        {
            foreach (XAttribute item in element.Attributes())
            {
                if (item.Name.LocalName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetChildElements(this XElement element, string name)
        {
            XElement item = element.FindElement(name);
            if (item == null)
            {
                yield break;
            }
            else
            {
                foreach (XElement e in item.Elements())
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement FindElement(this XElement element, XName name)
        {
            if (element == null) return null;

            if (element.Name == name) return element;

            foreach (XElement item in element.Elements())
            {
                XElement result = FindElement(item, name);
                if (result != null) return result;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement FindElement(this XElement element, string name)
        {
            if (element == null) return null;

            XName xname = XName.Get(name, element.GetDefaultNamespace().NamespaceName);
            return FindElement(element, xname);
        }

        /// <summary>
        /// 从Xml文件中加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IList<T> LoadEntitysFromXml<T>(string xml)
        {
            IList<T> data = new List<T>();

            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(xml);
                data = LoadEntitysFromXml<T>(xd);
            }
            catch (Exception ex)
            {
                throw new Exception("从xml中获取实体失败," + ex.Message + ex.StackTrace);
            }
            return data;
        }


        /// <summary>
        /// 从XmlDocument加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xd"></param>
        /// <returns></returns>
        public static IList<T> LoadEntitysFromXml<T>(XmlDocument xd)
        {
            IList<T> results = new List<T>();
            PropertyInfo[] propertys = typeof(T).GetProperties().Where(a => a.PropertyType.IsPublic).ToArray();
            if (propertys.Count() == 0) return results;

            XmlNode root = xd.DocumentElement;
            //外层
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name == typeof(T).Name)
                {
                    T result = (T)Activator.CreateInstance(typeof(T));
                    //内层循环
                    foreach (XmlNode subNode in node.ChildNodes)
                    {
                        PropertyInfo pi = propertys.Where(a => a.Name == subNode.Name).First();
                        if (pi != null && subNode.InnerText.ConvertByPropertyInfo(pi) != null)
                        {
                            pi.SetValue(result, subNode.InnerText.ConvertByPropertyInfo(pi), null);
                        }

                    }
                    results.Add(result);
                }
            }
            return results;
        }

        public static IList<XPath> GetXpathList(this string xml)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);// 载入XML文件数据
                foreach (XmlNode item in xmldoc.ChildNodes)
                {
                    DoVisitXmlNode(item);
                }
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return _currentXPaths;
        }


        static int index = 0;
        static IList<XPath> _currentXPaths = new List<XPath>();
        public static void DoVisitXmlNode(XmlNode node)
        {
            var path = GetNodePath(node, node.Name);

            if (node.Name == "#comment")
            {
                //注释 
                //_txtR.AppendText(string.Format("<!--{0}-->\r\n", node.Value));
                _currentXPaths.Add(new XPath() { No = index++, Type = "注释", Path = string.Format("<!--{0}-->", node.Value), Value = "" });
                return;
            }
            else
            {
                if ((node.ChildNodes == null || node.ChildNodes.Count == 0)
                    && (node.OuterXml.LastIndexOf(node.LocalName) > 3))
                {
                    //if (string.IsNullOrWhiteSpace(node.InnerText))
                    //{
                    //_txtR.AppendText(path + "/#text\t\r");
                    _currentXPaths.Add(new XPath()
                    {
                        No = index++,
                        Type = "★Empty",
                        ParaName = node.NodeType == XmlNodeType.Text ? string.Format("@{0}", node.ParentNode.LocalName) : node.LocalName,
                        NodeName = node.NodeType == XmlNodeType.Text ? node.ParentNode.LocalName : node.LocalName,
                        Path = string.Format("{0}/#text", path),
                        Value = ""
                    });
                    //}
                }
                else
                {
                    //节点
                    //_txtR.AppendText(path + "\t\r");
                    //if ((node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType == XmlNodeType.Text))
                    //    _currentXPaths.Add(new XPath() { No = index++, Type = "★Empty", Path = path, Value = node.Value });
                    //else
                    _currentXPaths.Add(new XPath()
                    {
                        No = index++,
                        Type = node.NodeType == XmlNodeType.Text ? "★Empty" : "节点",
                        ParaName = node.NodeType == XmlNodeType.Text ? string.Format("@{0}", node.ParentNode.LocalName) : node.LocalName,
                        NodeName = node.NodeType == XmlNodeType.Text ? node.ParentNode.LocalName : node.LocalName,
                        Path = path,
                        Value = node.Value
                    });
                }
            }

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                //属性
                foreach (XmlAttribute att in node.Attributes)
                {
                    //_txtR.AppendText(string.Format("{0}/@{1}\r\n", path, att.Name));
                    _currentXPaths.Add(new XPath() { No = index++, Type = "属性", Path = string.Format("{0}/@{1}", path, att.Name), Value = att.Value });
                }
            }

            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
            {
                foreach (XmlNode item in node.ChildNodes)
                {
                    DoVisitXmlNode(item);
                }
            }

        }


        public static string GetNodePath(XmlNode node, string currentPath)
        {
            var index = 0;
            if (node.ParentNode != null && node.ParentNode.Name != "#document")
            {
                if (node.NodeType == XmlNodeType.Element && node.ParentNode.ChildNodes.Cast<XmlNode>().Count(a => a.NodeType == XmlNodeType.Element && a.LocalName == node.LocalName) > 1
                    && !currentPath.Contains("#comment"))
                {
                    index = node.ParentNode.ChildNodes.Cast<XmlNode>().Where(a => a.NodeType == XmlNodeType.Element && a.LocalName == node.LocalName)
                        .Select(a => a.OuterXml).ToList().IndexOf(node.OuterXml) + 1;
                }
                if (index > 1)
                {
                    if (currentPath.IndexOf("/") == -1)
                        currentPath = string.Format("{0}/{1}[{2}]", node.ParentNode.Name, currentPath, index);
                    else
                    {
                        var t1 = currentPath.IndexOf("/");
                        var end = currentPath.Substring(t1);
                        var start = currentPath.Substring(0, t1) + string.Format("[{0}]", index);
                        currentPath = start + end;
                        currentPath = string.Format("{0}/{1}", node.ParentNode.Name, currentPath);
                    }
                }
                else
                    currentPath = string.Format("{0}/{1}", node.ParentNode.Name, currentPath);
                index = 0;
            }
            else
                return currentPath;
            return GetNodePath(node.ParentNode, currentPath);
        }
    }


    public class XPath
    {
        public int No { get; set; }

        // 注释   、  节点   、 属性
        public string Type { get; set; }

        public string ParaName { get; set; }

        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string JsonPath { get { return Path.Replace("/", "."); } set { } }

        public string NodeName { get; set; }

        public string Value { get; set; }

    }

}
