using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Base.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Base;

namespace JetSun.Core.DBAccess
{
    /// <summary>
    /// 表示数据库服务器类型。
    /// </summary>
    [Serializable]
    public class DbsKind
    {
        private int _id;
        private bool _allowMultiple;
        private string _name;

        static IList<DbsKind> _items = new List<DbsKind>();
        /// <summary>
        /// 返回所有<see cref="DbsKind"/>的枚举数。
        /// </summary>
        public static IEnumerable<DbsKind> Items
        {
            get { return _items; }
        }
        /// <summary>
        /// 返回指定ID对应的<see cref="DbsKind"/>。不存在时返回 Unspecified。
        /// </summary>
        /// <param name="dbsKindId"></param>
        /// <returns></returns>
        public static DbsKind Get(int dbsKindId)
        {
            return _items.FirstOrDefault(a => a.Id == dbsKindId) ?? DbsKind.Unspecified;
        }

        private DbsKind(int id, string name)
            : this(id, name, false)
        {
        }

        private DbsKind(int id, string name, bool allowMultiple)
        {
            _name = name;
            _id = id;
            _allowMultiple = allowMultiple;
            _items.Add(this);
        }

        /// <summary>
        /// 返回ID
        /// </summary>
        public int Id
        {
            get { return _id; }
        }
        /// <summary>
        /// 返回是否允许多实例。
        /// </summary>
        public bool AllowMultiple
        {
            get { return _allowMultiple; }
        }
        /// <summary>
        /// 返回名称。
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 返回是否是未指明的。
        /// </summary>
        public bool IsUnspecified
        {
            get { return _id == 0; }
        }
        /// <summary>
        /// 未指明的。
        /// </summary>
        public static DbsKind Unspecified = new DbsKind(0, "");
        /// <summary>
        /// HIS库
        /// </summary>
        public static DbsKind His = new DbsKind(1, "His");
        /// <summary>
        /// 主索引库
        /// </summary>
        public static DbsKind Mpi = new DbsKind(2, "Mpi");
        /// <summary>
        /// 健康档案库
        /// </summary>
        public static DbsKind HealthCase = new DbsKind(3, "HealthCase");
        /// <summary>
        /// 临床管理
        /// </summary>
        public static DbsKind Cis = new DbsKind(4, "Cis", true);
        /// <summary>
        /// 体检
        /// </summary>
        public static DbsKind Pis = new DbsKind(5, "Pis");
        /// <summary>
        /// 文档共享库
        /// </summary>
        public static DbsKind EHR = new DbsKind(6, "EHR");
        /// <summary>
        /// 文档仓储库
        /// </summary>
        public static DbsKind XdsRepository = new DbsKind(7, "XdsRepository");
        /// <summary>
        /// 区域平台数据库
        /// </summary>
        public static DbsKind Rhin = new DbsKind(8, "Rhin");
        /// <summary>
        /// 交互数据库
        /// </summary>
        public static DbsKind IXS = new DbsKind(9, "IXS");
        /// <summary>
        /// 库存
        /// </summary>
        public static DbsKind Wis = new DbsKind(10, "Wis", true);
        /// <summary>
        /// 科室管理
        /// </summary>
        public static DbsKind Mis = new DbsKind(11, "MIS");
        /// <summary>
        /// 临床数据
        /// </summary>
        public static DbsKind Cdr = new DbsKind(12, "Cdr");
        /// <summary>
        /// 电子病历
        /// </summary>
        public static DbsKind EMR = new DbsKind(13, "Emr");
        /// <summary>
        /// 护理
        /// </summary>
        public static DbsKind Nursing = new DbsKind(14, "Nursing");
        /// <summary>
        /// 药房
        /// </summary>
        public static DbsKind Pharmacy = new DbsKind(15, "Pharmacy");
        /// <summary>
        /// HIS20库
        /// </summary>
        public static DbsKind HIS20 = new DbsKind(100, "HIS20");
        /// <summary>
        /// 门诊20
        /// </summary>
        public static DbsKind OP20 = new DbsKind(101, "OP20");
        /// <summary>
        /// 住院20
        /// </summary>
        public static DbsKind IP20 = new DbsKind(102, "IP20");
        /// <summary>
        /// 检验20
        /// </summary>
        public static DbsKind LIS20 = new DbsKind(103, "LIS20");
        /// <summary>
        /// 库存20
        /// </summary>
        public static DbsKind WIS20 = new DbsKind(104, "WIS20");
        /// <summary>
        /// 技诊20
        /// </summary>
        public static DbsKind HTD20 = new DbsKind(105, "HTD20");
        /// <summary>
        /// Ris接口库
        /// </summary>
        public static DbsKind HIS_Ris = new DbsKind(106, "HIS_Ris");
        /// <summary>
        /// CRM
        /// </summary>
        public static DbsKind CRM = new DbsKind(107, "CRM");
        /// <summary>
        /// 
        /// </summary>
        public static DbsKind CHIS = new DbsKind(200, "CHIS");
        /// <summary>
        /// 合理用药 Rational Administration of drug
        /// </summary>
        public static DbsKind RAD = new DbsKind(112, "RAD");

        /// <summary>
        /// 单机版系统 上传的服务器的HIS库
        /// </summary>
        public static DbsKind HISOfUpLoadService = new DbsKind(300, "HISOfUpLoadService");
        /// <summary>
        /// 单机版系统 上传的服务器的PIS库
        /// </summary>
        public static DbsKind PISOfUpLoadService = new DbsKind(301, "PISOfUpLoadService");

        /// <summary>   
        /// 单机版系统 上传的服务器的HIS20库
        /// </summary>
        public static DbsKind His20OfUpLoadService = new DbsKind(302, "HIS20OfUpLoadService");

        /// <summary>
        /// 单机版系统 上传的服务器的LIS20库
        /// </summary>
        public static DbsKind Lis20OfUpLoadService = new DbsKind(303, "Lis20OfUpLoadService");

        /// <summary>
        /// 单机版系统 上传的服务器的OP20库
        /// </summary>
        public static DbsKind OP20OfUpLoadService = new DbsKind(304, "OP20OfUpLoadService");

        /// <summary>
        /// 分包机接口服务器
        /// </summary>
        public static DbsKind FBJOfInterface = new DbsKind(400, "FBJOfInterface");

    }
    /// <summary>
    /// 数据库服务器。不可继承此类。
    /// </summary>
    [DataContract]
    public class Dbs
    {
        static Regex _rxQualifiedName = new Regex("^[dD][bB][sS](?<id>[-]?[0-9]+)$");
        internal const string QualifiedNamePrefix = "Dbs";

        [DataMember]
        private int _id;
        [DataMember]
        private DbsKind _kind;

        private Dbs(int id, DbsKind kind)
            : this(id, kind, false)
        {
        }

        private Dbs(int id, DbsKind kind, bool isClone)
        {
            _kind = kind;
            _id = id;
            if (!isClone) _dbses[id] = this;
        }

        /// <summary>
        /// 返回一个克隆的实例。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        internal Dbs(Dbs source, ConnectionSettings settings)
            : this(source.Id, source.Kind, true)
        {
            _settings = settings;
        }

        private static int GetIdFromQualifiedName(string qualifiedName)
        {
            Match match = _rxQualifiedName.Match(qualifiedName);
            if (!match.Success) throw new ArgumentException("识别名称错误", "qualifiedName");

            return int.Parse(match.Groups["id"].Value);
        }

        /// <summary>
        /// 返回指定名称是否Dbs的限定名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsQualifiedName(string name)
        {
            return _rxQualifiedName.IsMatch(name);
        }

        static Dictionary<int, Dbs> _dbses = new Dictionary<int, Dbs>();
        /// <summary>
        /// 创建 Dbs 实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Dbs Create(int id)
        {
            Dbs dbs;
            lock (_dbses)
            {
                if (!_dbses.TryGetValue(id, out dbs))
                {
                    dbs = new Dbs(id, DbsKind.Unspecified);
                }
            }
            return dbs;
        }

        /// <summary>
        /// 创建 Dbs 实例
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static Dbs Create(string qualifiedName)
        {
            return Create(GetIdFromQualifiedName(qualifiedName));
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<Dbs> Items
        {
            get { return _dbses.Values; }
        }

        /// <summary>
        /// 获取连接串Id
        /// </summary>
        public int Id { get { return _id; } }
        /// <summary>
        /// 获取所属数据库类型。
        /// </summary>
        public DbsKind Kind { get { return _kind; } }

        [IgnoreDataMember]
        private ConnectionSettings? _settings;
        /// <summary>
        /// 返回当前设置
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConnectionSettings Settings
        {
            get { return _settings ?? ConnectionSettings.Empty; }
        }

        /// <summary>
        /// 获取连接串的限定名称
        /// </summary>
        public string QualifiedName
        {
            get
            {
                return string.Format("{0}{1}", QualifiedNamePrefix, Id);
            }
        }

        /// <summary>
        /// 返回Dbs的限定名称格式的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return QualifiedName.ToString();
        }

        /// <summary>
        /// 获取Dbs的哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// 判断当前实例是否和指定对象相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.GetHashCode() == (obj.GetHashCode());
        }

        /// <summary>
        /// 表示未指明的数据库。其连接串将优先从Web.config中加载。
        /// </summary>
        public static readonly Dbs Unspecified = new Dbs(0, DbsKind.Unspecified);

        /// <summary>
        /// HIS库（Id=1）
        /// </summary>
        public static readonly Dbs His = new Dbs(1, DbsKind.His);
        /// <summary>
        /// 主索引库（Id=2）
        /// </summary>
        public static readonly Dbs Mpi = new Dbs(2, DbsKind.Mpi);
        /// <summary>
        /// 健康档案库（Id=3）
        /// </summary>
        public static readonly Dbs HealthCase = new Dbs(3, DbsKind.HealthCase);
        /// <summary>
        /// 住院库（Id=4）
        /// </summary>
        public static readonly Dbs IP = new Dbs(4, DbsKind.Cis);
        /// <summary>
        /// 门诊库（Id=5）
        /// </summary>
        public static readonly Dbs OP = new Dbs(5, DbsKind.Cis);
        /// <summary>
        /// 患者共享文档库（Id=6）
        /// </summary>
        public static readonly Dbs EHR = new Dbs(6, DbsKind.EHR);
        /// <summary>
        /// 文档仓储库（Id=7）
        /// </summary>
        public static Dbs XdsRepository = new Dbs(7, DbsKind.XdsRepository);
        /// <summary>
        /// 区域平台数据库（Id=8）
        /// </summary>
        public static Dbs Rhin = new Dbs(8, DbsKind.Rhin);
        /// <summary>
        /// 交互数据库（Id=9）
        /// </summary>
        public static Dbs IXS = new Dbs(9, DbsKind.IXS);
        /// <summary>
        /// 交互数据库（Id=10）
        /// </summary>
        public static Dbs DXS = new Dbs(10, DbsKind.IXS);

        /// <summary>
        /// 管理信息库（Id=11）
        /// </summary>
        public static Dbs Mis = new Dbs(11, DbsKind.Mis);

        /// <summary>
        /// 临床数据库（Id=12）
        /// </summary>
        public static Dbs Cdr = new Dbs(12, DbsKind.Cdr);

        /// <summary>
        /// 电子病历(Id=13)
        /// </summary>
        public static Dbs Emr = new Dbs(13, DbsKind.EMR);
        /// <summary>
        /// 护理（Id=14）
        /// </summary>
        public static Dbs Nursing = new Dbs(14, DbsKind.Nursing);
        /// <summary>
        /// 药房（Id=15）
        /// </summary>
        public static Dbs Pharmacy = new Dbs(15, DbsKind.Pharmacy);

        /// <summary>
        /// 体检库（Id=20）
        /// </summary>
        public static readonly Dbs Pis = new Dbs(20, DbsKind.Pis);

        /// <summary>
        /// HIS20库（Id=100）
        /// </summary>
        public static readonly Dbs His20 = new Dbs(100, DbsKind.HIS20);
        /// <summary>
        /// 门诊2.0（Id=101）
        /// </summary>
        public static readonly Dbs OP20 = new Dbs(101, DbsKind.OP20);
        /// <summary>
        /// 住院2.0（Id=102）
        /// </summary>
        public static readonly Dbs IP20 = new Dbs(102, DbsKind.IP20);
        /// <summary>
        /// 检验2.0（Id=103）
        /// </summary>
        public static readonly Dbs LIS20 = new Dbs(103, DbsKind.LIS20);
        /// <summary>
        /// 库存2.0（Id=104）
        /// </summary>
        public static readonly Dbs WIS20 = new Dbs(104, DbsKind.WIS20);
        /// <summary>
        /// 技诊2.0（Id=105）
        /// </summary>
        public static readonly Dbs HTD20 = new Dbs(105, DbsKind.HTD20);
        /// <summary>
        /// Ris接口库（Id=106）
        /// </summary>
        public static readonly Dbs HIS_Ris = new Dbs(106, DbsKind.HIS_Ris);
        /// <summary>
        /// CRM（Id=107）
        /// </summary>
        public static readonly Dbs CRM = new Dbs(107, DbsKind.CRM);
        /// <summary>
        /// RAD，合理用药（Id=112）
        /// </summary>
        public static readonly Dbs RAD = new Dbs(112, DbsKind.RAD);

        /// <summary>
        /// 社区（Id=200）
        /// </summary>
        public static readonly Dbs Chis = new Dbs(200, DbsKind.CHIS);



        /// <summary>
        /// HIS30(体检上传服务器)
        /// </summary>
        public static readonly Dbs HISOfUpLoadService = new Dbs(300, DbsKind.HISOfUpLoadService);

        /// <summary>
        /// PIS30(体检上传服务器)
        /// </summary>
        public static readonly Dbs PISOfUpLoadService = new Dbs(301, DbsKind.PISOfUpLoadService);

        /// <summary>
        /// HIS20(体检上传服务器)
        /// </summary>
        public static readonly Dbs His20OfUpLoadService = new Dbs(302, DbsKind.His20OfUpLoadService);

        /// <summary>
        /// LIS20(体检上传服务器)
        /// </summary>
        public static readonly Dbs Lis20OfUpLoadService = new Dbs(303, DbsKind.Lis20OfUpLoadService);

        /// <summary>
        /// OP20(体检上传服务器)
        /// </summary>
        public static readonly Dbs OP20OfUpLoadService = new Dbs(304, DbsKind.OP20OfUpLoadService);
        /// <summary>
        /// 分包机接口服务器
        /// </summary>
        public static readonly Dbs FBJOfInterface = new Dbs(400, DbsKind.FBJOfInterface);
    }

    /// 连接串管理类
    /// </summary>
    public class DbsManager
    {
        const string defaultConfig = @"dbs.config";
        const string RootKey = "configuration";
        const string ConnectionKey = "connectionstrings";
        const string MetadataAssemblyKey = "metadataAssembly";
        const string PrivoderKey = "privoder";
        const string CurrentKey = "current";
        const string HistoryKey = "history";
        const string SettingKey = "settings";
        const string IsCryptographKey = "IsCryptograph"; //是否为密文
        const string EncryptKey = "09BC1C3A-F043-49F3-B15F-EB9EE1F22C7E";//加密密钥
        private static SymmetricAlgorithm _symmetricAlgorithm;

        Dictionary<Dbs, DbsSetting> _dbses = new Dictionary<Dbs, DbsSetting>();
        public Dictionary<Dbs, DbsSetting> DbsList
        {
            get
            {
                return _dbses;
            }
        }

        Dictionary<string, string> _settings = new Dictionary<string, string>();

        static SymmetricAlgorithm SymmetricAlgorithm
        {
            get
            {
                if (_symmetricAlgorithm == null)
                {
                    _symmetricAlgorithm = CryptographyEx.CreateSymmAlgoTripleDes();//3DES算法
                    _symmetricAlgorithm.IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
                }
                return _symmetricAlgorithm;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DbsManager Instance = new DbsManager(true);
        /// <summary>
        /// 
        /// </summary>
        public DbsManager()
            : this(false)
        {
        }

        private DbsManager(bool autoLoad)
        {
            if (autoLoad) Load(Runtime.DbsFilePath);
        }

        const string _RegKey_serverConfigSize = "ServerConfigSize";
        const string _RegKey_serverConfigfn = "ServerConfigfName";

        private void LoadDbs(XElement doc)
        {
            byte code;
            DbsProvider provider;
            XElement currentElement;
            XElement historyElement;
            string currentConnectionString;
            string historyConnectionString;
            bool currentConnectionStringIsCryptograph;
            bool historyConnectionStringIsCryptograph;
            foreach (XElement e in doc.GetChildElements(ConnectionKey))
            {
                if (Dbs.IsQualifiedName(e.Name.ToString()))
                {
                    Dbs dbs = Dbs.Create(e.Name.ToString());
                    if (byte.TryParse(GetElementValue(e, PrivoderKey), out code))
                        DbsProvider.TryGetProvider(code, out provider);
                    else
                        provider = DbsProvider.MsSql;

                    currentConnectionString = GetElementValue(e, CurrentKey);
                    currentElement = e.GetElement(CurrentKey);
                    if (currentElement != null)
                    {
                        currentConnectionStringIsCryptograph = currentElement.GetAttributeValue<bool>(IsCryptographKey, false);
                        if (currentConnectionStringIsCryptograph)
                        {
                            currentConnectionString = Decrypt(currentConnectionString);
                        }
                    }
                    historyConnectionString = GetElementValue(e, HistoryKey);
                    historyElement = e.GetElement(HistoryKey);
                    if (historyElement != null)
                    {
                        historyConnectionStringIsCryptograph = historyElement.GetAttributeValue<bool>(IsCryptographKey, false);
                        if (historyConnectionStringIsCryptograph)
                        {
                            historyConnectionString = Decrypt(historyConnectionString);
                        }
                    }

                    DbsSetting setting = new DbsSetting(dbs, DbsProvider.MsSql, currentConnectionString)
                    {
                        Provider = provider,
                        HistoryConnectionString = historyConnectionString,
                        MetadataAssemblyFullName = GetElementValue(e, MetadataAssemblyKey)
                    };
                    Add(setting);
                }
            }
        }

        private string GetElementValue(XElement element, string key)
        {
            XElement child = element.Elements(key).FirstOrDefault();
            if (child == null) return string.Empty;

            return child.Value;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private string Encrypt(string plainText)
        {
            return CryptographyEx.Encrypt(SymmetricAlgorithm,
                plainText, EncryptKey, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="base64Text"></param>
        /// <returns></returns>
        private string Decrypt(string base64Text)
        {
            return CryptographyEx.Decrypt(SymmetricAlgorithm,
                base64Text, EncryptKey, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        /// <summary>
        /// 从配置文件中加载
        /// </summary>
        /// <param name="configFile"></param>
        public void Load(string configFile)
        {
            if (!File.Exists(configFile)) return;

            XElement doc;
            using (TextReader sr = TextReader.Synchronized(new StreamReader(configFile)))
            {
                doc = XElement.Load(sr);
            }

            if (doc == null) return;

            Load(doc);
        }

        private void Load(XElement doc)
        {
            LoadDbs(doc);
            LoadSettings(doc);
            LoadFromCofiguration(Dbs.Unspecified);
        }

        private void LoadFromCofiguration(Dbs dbs)
        {
            ConnectionStringSettings css = WebConfigurationManager.ConnectionStrings[dbs.QualifiedName]
                                        ?? ConfigurationManager.ConnectionStrings[dbs.QualifiedName];

            if (css != null)
            {
                DbsProvider provider = DbsProvider.Providers.FirstOrDefault(a => string.Equals(a.Provider, css.ProviderName, StringComparison.CurrentCultureIgnoreCase) && a.Validate(css.ConnectionString));

                DbsSetting setting = new DbsSetting(dbs, provider, css.ConnectionString)
                {
                    Provider = provider
                };
                Add(setting);
            }
        }

        private void LoadSettings(XElement doc)
        {
            foreach (XElement e in doc.GetChildElements(SettingKey))
            {
                _settings[e.Name.LocalName] = e.Value;
            }
        }

        /// <summary>
        /// 增加连接串设置
        /// </summary>
        /// <param name="setting"></param>
        public void Add(DbsSetting setting)
        {
            _dbses[setting.Dbs] = setting;
        }


    }
}