using System;
using System.Net;
using System.ServiceModel;
using System.Reflection;
using System.ServiceModel.Channels;
using Base.Ex;

namespace Base
{
    public class WcfClient
    {
        /// <summary>
        /// 创建一个Wcf服务客户端实例
        /// </summary>
        /// <typeparam name="TSvcClient"></typeparam>
        /// <typeparam name="ITSvc"></typeparam>
        /// <returns></returns>
        public static TSvcClient CreateSvcClient<TSvcClient, ITSvc>()
            where TSvcClient : ClientBase<ITSvc>
            where ITSvc : class
        {
            try
            {
                TSvcClient svcClient = Activator.CreateInstance<TSvcClient>();
                svcClient.Endpoint.Address = GetSvcEndpointAddress<ITSvc>();
                return svcClient;
            }
            catch (Exception ex)
            {
# if  !SILVERLIGHT 
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
#endif
            }
            return null;
        }

# if  !SILVERLIGHT
        /// <summary>
        /// 创建Wcf代理服务
        /// </summary>
        /// <typeparam name="ITProxySvc"></typeparam>
        /// <returns></returns>
        public static ITProxySvc CreateProxySvc<ITProxySvc>()
            where ITProxySvc : class
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding("BlobBindingConfig");
                EndpointAddress ep = GetSvcEndpointAddress<ITProxySvc>();
                ITProxySvc proxySvc = ChannelFactory<ITProxySvc>.CreateChannel(binding, ep);
                return proxySvc;
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return null;
        }
#endif

        /// <summary>
        /// 取得Wcf服务Uri地址
        /// </summary>
        /// <typeparam name="TSvcClient"></typeparam>
        /// <typeparam name="ITSvc"></typeparam>
        /// <returns></returns>
        public static string GetSvcUri<ITSvc>()
        {
            string url = string.Empty;
            string iSvcName = typeof(ITSvc).Name;

            if (iSvcName.StartsWith("I", StringComparison.CurrentCultureIgnoreCase))
                iSvcName = iSvcName.Substring(1);
            url = string.Format("{0}/{1}{2}", AppSetting.DataServiceBaseUrl, iSvcName, AppSetting.IsDebugHost ? string.Empty : ".svc");
            return url;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSvcClient"></typeparam>
        /// <typeparam name="ITSvc"></typeparam>
        /// <returns></returns>
        public static EndpointAddress GetSvcEndpointAddress<ITSvc>()
        {
            return new EndpointAddress(GetSvcUri<ITSvc>());
        }


    }
}
