using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using Base;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;
using System.Xml;
using DomainModel;
using System.ServiceModel.Web;
using Base.Ex;

namespace DataService.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface ISvcBase
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dtodata"></param>
        /// <returns></returns>
        [OperationContract(Name = "Save")]
        [WebInvoke(Method = "POST", UriTemplate = RestfulApiList.Save,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string Save();

        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [OperationContract(Name = "Remove")]
        [WebInvoke(Method = "POST", UriTemplate = RestfulApiList.Remove,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string Remove();

        /// <summary>
        /// 只是更新删除标志
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [OperationContract(Name = "Delete")]
        [WebInvoke(Method = "POST", UriTemplate = RestfulApiList.Delete,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string Delete();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetByKey")]
        [WebInvoke(Method = "POST", UriTemplate = RestfulApiList.GetByKey,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string GetByKey();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [OperationContract(Name = "Query")]
        [WebInvoke(Method = "POST", UriTemplate = RestfulApiList.Query,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string Query();
    }


    /// <summary>
    /// 标注操作契约方法自行选择数据契约的序列化器。不可继承此类
    /// <para>对于类型使用特性[NetDataContractFormat]标注的参数，使用NetDataContractSerializer</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AdaptiveDataContractSerializerAttribute : Attribute, IOperationBehavior
    {
        /// <summary>
        /// 获取/设置是否使用大数据块的DataContractSerializer。
        /// </summary>
        public bool UseBlobDataContractSerializer { get; set; }

        private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description, bool useBlobDataContractSerializer)
        {
            DataContractSerializerOperationBehavior dcsOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if (dcsOperationBehavior != null)
            {
                description.Behaviors.Remove(dcsOperationBehavior);
                description.Behaviors.Add(new AdaptiveDataContractSerializerOperationBehavior(description) { UseBlobDataContractSerializer = useBlobDataContractSerializer });
            }
        }

        #region IOperationBehavior 成员

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription, this.UseBlobDataContractSerializer);
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription, this.UseBlobDataContractSerializer);
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }

        #endregion

        /// <summary>
        /// Represents the run-time behavior of the <see cref="DataContractSerializer"/>.
        /// </summary>
        private class AdaptiveDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
        {
            public AdaptiveDataContractSerializerOperationBehavior(OperationDescription operationDescription) :
                base(operationDescription)
            { }

            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                if (type.GetAttribute<NetDataContractFormatAttribute>() != null)
                    return new NetDataContractSerializer(name, ns);

                if (UseBlobDataContractSerializer)
                    return new DataContractSerializer(type, name, ns, knownTypes,
                    int.MaxValue /*maxItemsInObjectGraph*/,
                    false/*ignoreExtensionDataObject*/,
                    false/*preserveObjectReferences*/,
                    null/*dataContractSurrogate*/);


                return base.CreateSerializer(type, name, ns, knownTypes);
            }

            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                if (type.GetAttribute<NetDataContractFormatAttribute>() != null)
                    return new NetDataContractSerializer(name, ns);

                if (UseBlobDataContractSerializer)
                    return new DataContractSerializer(type, name, ns, knownTypes,
                    int.MaxValue /*maxItemsInObjectGraph*/,
                    false/*ignoreExtensionDataObject*/,
                    false/*preserveObjectReferences*/,
                    null/*dataContractSurrogate*/);

                return base.CreateSerializer(type, name, ns, knownTypes);
            }

            public bool UseBlobDataContractSerializer { get; set; }
        }

        /// <summary>
        /// 指示类型采用"NetDataContractSerializer"序列化器。不可继承此类
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public sealed class NetDataContractFormatAttribute : Attribute
        {
        }
    }
}
