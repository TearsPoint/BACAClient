using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BACAClient.Common
{
    public class HtmlUtils
    {
        private static string htmlResult = "";
        private static string url = string.Format(@"{0}\TemporaryFiles\HTML", System.Windows.Forms.Application.StartupPath);
        //private static StreamWriter writer = null;

        public static void CSS(double minHeight)
        {
            htmlResult = htmlResult + "<style>";
            object[] objArray1 = new object[] { htmlResult, "body{margin:0;padding:0;font-Family:'微软雅黑';font-size:16px;color:#333333;backgroud-color:#ffffff;line-height:28px;border: 1px solid #999999;padding:0 10px;margin:0 15px;overflow-x:hidden;min-height:", minHeight - 2.0, "px;}" };
            htmlResult = string.Concat(objArray1);
            htmlResult = htmlResult + "a{ text-decoration:none;color:#3399cc;font-size:16px;font-weight:700;cursor:pointer;}";
            htmlResult = htmlResult + "img{text-align:center;margin:0;padding:0;}";
            htmlResult = htmlResult + ".BZ{width:16px;height:16px;line-height:28px;}";
            htmlResult = htmlResult + ".detail{width:100%;height:auto; }";
            htmlResult = htmlResult + "div{text-indent:2em;}";
            htmlResult = htmlResult + "</style>";
        }

        public static void CSSForms(double minHeight)
        {
            htmlResult = htmlResult + "<style>";
            object[] objArray1 = new object[] { htmlResult, "body{margin:0;padding:0;border:1px solid #cccccc;padding:0 10px;margin:0 15px;overflow-x:hidden;min-height:", minHeight - 2.0, "px;}" };
            htmlResult = string.Concat(objArray1);
            htmlResult = htmlResult + "ul{margin:0;padding:0;}";
            htmlResult = htmlResult + "li{margin:0;padding:0;list-style-type:none; text-align:center;}";
            htmlResult = htmlResult + "</style>";
        }

        public static void JavaScript()
        {
            htmlResult = htmlResult + "<script type='text/javascript'>";
            htmlResult = htmlResult + "window.onload = function (){";
            htmlResult = htmlResult + "var detail = document.getElementById('detail');";
            htmlResult = htmlResult + "var height =parseFloat(detail.clientHeight);";
            htmlResult = htmlResult + "var min = parseFloat(document.getElementById('MinHeight').innerText);";
            htmlResult = htmlResult + "if (height < min)";
            htmlResult = htmlResult + "detail.style.marginTop = (min - height) / 2;";
            htmlResult = htmlResult + "if (detail.innerText.length < document.body.clientWidth / 16)";
            htmlResult = htmlResult + "detail.style.textAlign = 'center';";
            htmlResult = htmlResult + "}";
            htmlResult = htmlResult + "</script>";
        }

        public static string WriterHtml(string html, double minHeight)
        {
            try
            {
                html = ReplaceUtils.ReplaceHtml(HttpUtility.HtmlDecode(html));
                htmlResult = "<!DOCTYPE html>";
                htmlResult = htmlResult + "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>";
                htmlResult = htmlResult + "<head>";
                htmlResult = htmlResult + "<meta charset='utf-8'/>";
                htmlResult = htmlResult + "<title></title>";
                CSS(minHeight);
                JavaScript();
                htmlResult = htmlResult + "</head>";
                htmlResult = htmlResult + "<body>";
                htmlResult = htmlResult + "<div class='detail' id='detail'>";
                htmlResult = htmlResult + html;
                htmlResult = htmlResult + "</div>";
                object[] objArray1 = new object[] { htmlResult, "<span id='MinHeight' style='display:none;'>", minHeight, "</span>" };
                htmlResult = string.Concat(objArray1);
                htmlResult = htmlResult + "</body>";
                htmlResult = htmlResult + "</html>";
            }
            catch
            {
            }
            return htmlResult;
        }

        public static string WriterHtmlForms(string html, double minHeight)
        {
            try
            {
                htmlResult = "<!DOCTYPE html>";
                htmlResult = htmlResult + "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>";
                htmlResult = htmlResult + "<head>";
                htmlResult = htmlResult + "<meta charset='utf-8'/>";
                htmlResult = htmlResult + "<title></title>";
                CSSForms(minHeight);
                htmlResult = htmlResult + "</head>";
                htmlResult = htmlResult + "<body>";
                htmlResult = htmlResult + "<div>";
                htmlResult = htmlResult + "<ul>";
                htmlResult = htmlResult + html;
                htmlResult = htmlResult + "</ul>";
                htmlResult = htmlResult + "</div>";
                htmlResult = htmlResult + "</body>";
                htmlResult = htmlResult + "</html>";
            }
            catch
            {
            }
            return htmlResult;
        }
    }

}
