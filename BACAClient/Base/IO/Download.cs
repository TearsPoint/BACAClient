using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using Microsoft.VisualBasic.Devices;
using Base.Ex;

namespace Base.IO
{
    /// <summary>
    /// 上传、下载
    /// </summary>
    public class UpOrDownload
    {
        /// <summary>
        /// 下载网络文件
        /// </summary>
        /// <param name="fileAddress">网络文件访问地址</param> 
        /// <param name="savePath">保存路径</param>
        public bool DownloadNetworkFile(string fileAddress, string savePath)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(fileAddress, savePath);
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 上传网络文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="address">指定的主机地址</param>
        /// <param name="userName">登录用户名</param>
        /// <param name="password">密码</param>
        public bool UploadNetworkFile(string sourcePath, string address, string userName, string password)
        {
            try
            {
                Network netWork = new Network();
                netWork.UploadFile(sourcePath, address, userName, password);
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                return false; }
            return true;
        }

        /// <summary>
        /// 上传网络文件（适用于不需要用户验证的）
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="address">指定的主机地址</param>
        public bool UploadNetworkFile(string sourcePath, string address)
        {
            try
            {
                Network netWork = new Network();
                netWork.UploadFile(sourcePath, address);
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                return false;
            }
            return true;
        }
    }
}
