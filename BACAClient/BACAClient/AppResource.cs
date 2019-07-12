using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows.Resources;
using System.Resources;
using System.Collections;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Windows.Baml2006;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using Base;

namespace BACAClient
{

    public class AppResources
    {
        public static Assembly _assembly = typeof(AppResources).Assembly;

        static IDictionary<string, ResourceDictionary> _themes;
        public static IDictionary<string, ResourceDictionary> Themes
        {
            get
            {
                if (_themes == null)
                    _themes = new Dictionary<string, ResourceDictionary>();

                return _themes;
            }
        }

        static AppResources()
        {
            InitializeThemes();
        }

        /// <summary>
        /// 基础资源文件地址
        /// </summary>
        public const string _BASE_RESOURCES_FILE_PATH = "";

        /// <summary>
        /// 基础资源字典对象
        /// </summary>
        public static ResourceDictionary BaseResources
        {
            get
            {
                string path = _BASE_RESOURCES_FILE_PATH;
                if (!string.IsNullOrEmpty(path))
                {
                    Uri uri = new Uri(path, UriKind.Relative);
                    StreamResourceInfo sri = Application.GetResourceStream(uri);
                    //StreamReader sr = new StreamReader(sri.Stream);
                    ResourceDictionary resource = XamlReader.Load(sri.Stream) as ResourceDictionary;
                    return resource;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取ImageSource
        /// </summary>
        /// <param name="resourceSubFolderName"></param>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public static ImageSource GetImageSource(string resourceSubFolderName, string fileFullName)
        {
            BitmapImage bi = new BitmapImage(new Uri(BuildFullPath(resourceSubFolderName, fileFullName)));
            return bi;
        }

        static IDictionary<string, BitmapImage> _cachedImageSource = new Dictionary<string, BitmapImage>();
        /// <summary>
        /// 获取ImageSource
        /// 传入相对路径：如Resources文件下的Images文件夹下的***.jpg  : "Images\***.jpg"
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public static ImageSource GetImageSource(string relativePath)
        {
            BitmapImage bi;
            if (_cachedImageSource.ContainsKey(relativePath))
            {
                bi = _cachedImageSource[relativePath];
                return bi;
            }
            if (relativePath.StartsWith("http"))
                bi = new BitmapImage(new Uri(relativePath));
            else
                bi = new BitmapImage(new Uri(BuildFullPath(string.Empty, relativePath)));
            _cachedImageSource[relativePath] = bi;
            return bi;
        }

        /// <summary>
        /// 构建资源文件完整路径
        /// </summary>
        /// <param name="resourceSubFolderName"></param>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public static string BuildFullPath(string resourceSubFolderName, string fileFullName)
        {
            if (string.IsNullOrWhiteSpace(fileFullName) && resourceSubFolderName == "Images")
                fileFullName = "default_icon.png";
            return string.Format(@"{0}\{1}\{2}", Runtime.ResoucesFolderPath, string.IsNullOrEmpty(resourceSubFolderName) ? "" : resourceSubFolderName, fileFullName);
        }

        /// <summary>
        /// 初始化主题资源文件 
        /// </summary>
        private static void InitializeThemes()
        {
            var resources = _assembly.GetManifestResourceNames();

            var themes = resources.Where(a => a.EndsWith(".xaml") && a.Contains("Theming"));
            foreach (var item in themes)
            {
                Stream stream = _assembly.GetManifestResourceStream(item);
                ResourceDictionary rd = XamlReader.Load(stream) as ResourceDictionary;
                Themes.Add(item.Replace(_assembly.FullName, string.Empty), rd);
            }
        }

        public static string GetDirectory(string path)
        {
            string p = string.Empty;
            p = path.Substring(0, path.LastIndexOf(@"/") + 1);
            return p;
        }

        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object. 
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the  
            // current security settings. 
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings.  
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));

            // Set the new access settings. 
            dInfo.SetAccessControl(dSecurity);
        }

    }
}
