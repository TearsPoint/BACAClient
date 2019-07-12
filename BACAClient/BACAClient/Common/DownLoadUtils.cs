using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BACAClient.Common
{

    public class DownLoadUtils
    {
        public static bool DownloadExcel(string pdfpath, string downpath, string pdfname)
        {
            bool flag = false;
            try
            {
                pdfpath = pdfpath + @"\" + pdfname;
                if (!Directory.Exists(downpath))
                {
                    Directory.CreateDirectory(downpath);
                }
                new WebClient().DownloadFile(pdfpath, downpath + @"\" + pdfname);
                flag = true;
            }
            catch (Exception exception)
            {
                object[] objArray1 = new object[] { pdfname, "/", pdfpath, "/", downpath, "/", exception };
                string erorInfo = string.Concat(objArray1);
            }
            return flag;
        }

        public static SystemEnum.DownloadType DownloadPDF(string pdfpath, string pdfname, string typename, string title)
        {
            string selectedPath = string.Empty;
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = dialog.SelectedPath;
            }
            else
            {
                return SystemEnum.DownloadType.CANCEL;
            }
            if (string.IsNullOrEmpty(selectedPath))
            {
                return SystemEnum.DownloadType.ERROR;
            }
            try
            {
                if (typename != "引文文献")
                {
                    pdfpath = pdfpath + string.Format(@"\{0}", pdfname);
                }
                if (!Directory.Exists(selectedPath))
                {
                    Directory.CreateDirectory(selectedPath);
                }
                WebClient client = new WebClient();
                selectedPath = selectedPath + string.Format(@"\【{0}】{1}.pdf", typename, title);
                client.DownloadFile(pdfpath, selectedPath);
                return SystemEnum.DownloadType.SUCCESS;
            }
            catch (Exception exception)
            {
                object[] objArray1 = new object[] { pdfname, "/", pdfpath, "/", selectedPath, "/", exception };
                string erorInfo = string.Concat(objArray1);
                return SystemEnum.DownloadType.ERROR;
            }
        }
    }

}
