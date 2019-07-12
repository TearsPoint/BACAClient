using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Base;
using Base.Ex;
using Base.IO;
using DataService.ServiceModel;
using DomainModel;
using fastJSON;
using Newtonsoft.Json;

namespace Base.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebRequestMethod
    {
        /// <summary>
        /// 
        /// </summary>
        public static string POST = "POST";
        /// <summary>
        /// 
        /// </summary>
        public static string GET = "GET";
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataServiceGroup
    {
        public DataServiceGroup(string key)
        {
            this.Key = key;
        }
        public string Key { get; set; }


        /// <summary>
        /// Default
        /// </summary>
        public static DataServiceGroup Default = new DataServiceGroup("DataServiceBaseUrl");
         
    }

    /// <summary>
    /// RepositoryBase 的缩写
    /// </summary>
    /// <typeparam name="ISvc"></typeparam>
    public class RB<ISvc>
      where ISvc : class, ISvcBase
    {
        private ISvc _proxy;
        /// <summary>
        /// 
        /// </summary>
        public ISvc Proxy
        {
            get
            {
                if (_proxy == null)
                {
                    if (AppSetting.IsUseWCF)
                        _proxy = WcfClient.CreateProxySvc<ISvc>();
                    else
                        _proxy = typeof(ISvc).Intance<ISvc>();
                }
                return _proxy;
            }
        }
    }

    public interface IOnCreateNew
    {
        object OnCreateNew();
    }

    /// <summary>
    /// 
    /// </summary> 
    public abstract class RB
    {
        /// <summary>
        /// 服务名  XXXSvc
        /// </summary>
        public abstract string SvcName { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract DataServiceGroup DataServiceGroup { get; }

        public T CreateNew<T>(dynamic d = null)
            where T : class, IOnCreateNew, new()
        {
            return new T().OnCreateNew() as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string GetRequestUrl(string url)
        {
            if (!url.StartsWith("http"))
                url = string.Format("{0}/{1}/{2}", AppSetting.GetDataServiceBaseUrl(this.DataServiceGroup), SvcName, url);
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string Request(string url, IDictionary<string, string> postData)
        {
            return Request(url, JSON.ToJSON(postData));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public T Request<T>(string url, object postData)
        {
            return Request<T>(url, (postData is string) ? postData : postData.ToJson());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="urlParas"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public T Request<T>(string url, IDictionary<string, string> urlParas, object postData)
        {
            return Request<T>(url, urlParas, (postData is string) ? postData : postData.ToJson());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlParas"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public string Request(string url, IDictionary<string, string> urlParas, string jsonData)
        {
            if (urlParas != null)
                foreach (var p in urlParas)
                {
                    url = url.Replace(string.Format("{{{0}}}", p.Key), p.Value);
                }
            return Request(url, jsonData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public T Request<T>(string url, string jsonData)
        {
            string jR = Request(url, jsonData);
            return jR.ToObject<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPartUrl"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public string Request(string endPartUrl, string jsonData)
        {
            string res = string.Empty;
            try
            {
                byte[] bytes = new byte[] { };
                if (!jsonData.IsNullOrEmpty())
                    bytes = Encoding.UTF8.GetBytes(jsonData);
                //声明一个HttpWebRequest请求
                HttpWebRequest request = CreateRequest(endPartUrl, bytes);
                if (request.Method == "POST")
                {
                    Stream reqstream = request.GetRequestStream();
                    reqstream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.UTF8;

                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                res = streamReader.ReadToEnd();
                if (res.StartsWith("\""))
                    res = JsonConvert.DeserializeObject<string>(res);  // todo  JSON.ToJSON(res)

                streamReceive.Dispose();
                streamReader.Dispose();
            }
            catch (Exception ex)
            {
                return Loger.Log(Assembly.GetExecutingAssembly().FullName,
                     string.Format("{0}\r\n请求url: {1}\r\n请求post数据:\r\n{2}", MethodBase.GetCurrentMethod().FullName(), GetRequestUrl(endPartUrl).ToLower(), jsonData), ex);
            }
            return res;
        }

        private HttpWebRequest CreateRequest(string url, byte[] bytes)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetRequestUrl(url));
            string requestMethod = "GET";
            if (bytes.Length > 0) requestMethod = "POST";
            request.Method = requestMethod;
            //CookieContainer cookie = request.CookieContainer;   //Cookie 

            #region http 头部 
            //以下是发送的http头
            request.Referer = "";
            request.Headers.Set("Cache-Control", "no-cache");
            if (requestMethod == "POST")
            {
                request.Accept = "Accept:text/html,application/xhtml+xml,application/xml,application/json;q=0.9,*/*;q=0.8";
                request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
                request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
                request.Headers.Set("Pragma", "no-cache");
                request.ContentType = "text/json";
            }
            request.Timeout = 300000; //设置连接超时时间 , 默认超时时间 5分钟

            //request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
            //request.KeepAlive = true;

            //上面的http头看情况而定，但是下面俩必须加 
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;

            #endregion
            return request;
        }

    }

    /// <summary>
    /// 方便单元测试
    /// </summary>
    public class RestRT : RB
    {
        public RestRT(string svcName, DataServiceGroup dsGroup)
        {
            _svcName = svcName;
            _dataServiceGroup = dsGroup;
        }

        public static RestRT Instance(string svcName, DataServiceGroup dsGroup)
        {
            return new RestRT(svcName, dsGroup);
        }

        private string _svcName;
        public override string SvcName { get { return _svcName; } }

        DataServiceGroup _dataServiceGroup;
        /// <summary>
        /// 
        /// </summary>
        public override DataServiceGroup DataServiceGroup
        {
            get { return _dataServiceGroup; }
        }
    }
}
