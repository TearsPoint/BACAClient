using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BACAClient.Common
{
    public class ReplaceUtils
    {
        public static string DivToNewLine(string data)
        {
            return Regex.Replace(Regex.Replace(data, "<div>", string.Empty), "</div>", "\n");
        }

        public static string DotToAsterisk(string data)
        {
            return data.Replace(".", "*");
        }

        public static string GetXmlDate(string file, string path)
        {
            return file.Replace(path + @"\", string.Empty).Replace(".xml", string.Empty);
        }

        public static string Img(string html)
        {
            string str = Regex.Replace(html, "src=\"..", "src=\"");
            return Regex.Replace(html, "src=\"", "src=\"" + ConfigerHelper.GetConfiger(new ConfigerParameterName().Resource) + @"\");
        }

        public static string InstallationPath(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    url = System.Windows.Forms.Application.StartupPath;
                }
                string str = Regex.Replace(Regex.Replace(url, ".EXE", ".exe"), "BACAClient.exe", string.Empty);
                return string.Format(@"{0}\TemporaryFiles\DLL", str);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Intro(string data)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(data, "&amp;nbsp", string.Empty), "&nbsp", string.Empty), "nbsp", string.Empty), "&amp;", string.Empty), "&nbsp;", string.Empty);
        }

        public static string NewLineToDiv(string data)
        {
            return Regex.Replace(data, "\n", "</div><div>");
        }

        public static string NoHTML(string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, "<script[^>]*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<(.[^>]*)>", string.Empty, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", string.Empty, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "-->", string.Empty, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<!--.*", string.Empty, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(iexcl|#161);", "\x00a1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(cent|#162);", "\x00a2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(pound|#163);", "\x00a3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(copy|#169);", "\x00a9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", string.Empty, RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", string.Empty);
            Htmlstring.Replace(">", string.Empty);
            Htmlstring.Replace("\r\n", string.Empty);
            return Htmlstring;
        }

        public static string RemoveHtml(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            string pattern = "<[^>]+>";
            data = Regex.Replace(data, pattern, string.Empty);
            data = Regex.Replace(data, "&[^;]+;", string.Empty);
            data = Intro(data);
            data = Regex.Replace(data, "\\s*|\t|\r|\n", string.Empty);
            data = data.Replace(" ", string.Empty);
            return data.Trim();
        }

        public static string ReplaceAHerf(string href)
        {
            if (string.IsNullOrEmpty(href))
            {
                return string.Empty;
            }
            char[] separator = new char[] { '?' };
            return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(href.Split(separator)[1], "lngid=", ""), "typeid=", ""), "strName=", ""), "id=", ""), "&", "/");
        }

        public static string ReplaceHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }
            return HttpUtility.HtmlDecode(Img(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(html, "DIV", "div"), "SUB>", "sub>"), "SUP>", "sup>"), "<IMG", "<img"), "href=?", "name='"), " target=_blank", "'")));
        }

        public static string SolrSeacrhInAND(string data)
        {
            return Regex.Replace(Regex.Replace(data, "=", ":"), "&", " AND ");
        }

        public static string SolrSeacrhInOR(string data)
        {
            return Regex.Replace(Regex.Replace(data, "=", ":"), "&", " OR ");
        }

        public static string TextBoxNewline(string data)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(data, ",", "\n\n\n"), ";", "：\n"), "&", "：\n\n");
        }

        public static string XMLNodeName(string data, string compare)
        {
            char[] trimChars = new char[] { '\n' };
            char[] chArray2 = new char[] { '\n' };
            return Regex.Replace(data, compare, string.Empty).TrimStart(trimChars).TrimEnd(chArray2);
        }
    }


}
