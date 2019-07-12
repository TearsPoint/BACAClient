using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Base.Ex;

namespace Base.IO
{
    /// <summary>
    /// 文件搜索器
    /// </summary>
    public class FileSearcher
    {
        /// <summary>
        /// 创建一搜索器实例
        /// </summary>
        public static FileSearcher Instance() { return new FileSearcher(); }

        private IList<FileInfoEx> _searchResult;
        /// <summary>
        /// 搜索结果集
        /// </summary>
        public IList<FileInfoEx> SearchResult
        {
            get
            {
                if (_searchResult == null)
                    _searchResult = new List<FileInfoEx>();
                return _searchResult;
            }
            set { _searchResult = value; }
        }

        /// <summary>
        /// 搜索文件（支持模糊搜索）
        /// </summary>
        /// <param name="name">需匹配内容</param>
        /// <param name="path">目录路径</param>
        public IList<FileInfoEx> FindByName(string name, string path)
        {
            DirectoryInfo dire = new DirectoryInfo(path);
            SearchFromDirectory(name, dire, false);
            return SearchResult;
        }

        /// <summary>
        /// 根据后缀查询文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public IList<FileInfoEx> FindBySuffix(string name, string path)
        {
            DirectoryInfo dire = new DirectoryInfo(path);
            SearchFromDirectory(name, dire, true);
            return SearchResult;
        }

        public IList<FileInfoEx> GetFileInfoWithLevel(string path, bool isContainHide = false)
        {
            FileInfoEx root = new FileInfoEx() { FileName = "Root", IsDirectory = true };
            DirectoryInfo dire = new DirectoryInfo(path);
            root.ChildrenFileInfo = GetFileInfoWithLevel(dire, isContainHide);
            return root.ChildrenFileInfo;
        }

        private static IList<FileInfoEx> GetFileInfoWithLevel(DirectoryInfo dire, bool isContainHide = false)
        {
            IList<FileInfoEx> results = new List<FileInfoEx>();
            FileInfo[] files;
            DirectoryInfo[] dires;

            files = dire.GetFiles();
            dires = dire.GetDirectories();

            foreach (FileInfo n in files)
            { 
                FileInfoEx file = new FileInfoEx
                {
                    Attributes = n.Attributes,
                    Length = n.Length,
                    FullName = n.FullName,
                    FileName = n.Name,
                    LastAccessTime = n.LastAccessTime
                };
                results.Add(file);
            }

            foreach (var n in dires)
            {
                FileInfoEx file = new FileInfoEx
                {
                    Attributes = n.Attributes,
                    Length = 0,
                    FullName = n.FullName,
                    FileName = n.Name,
                    LastAccessTime = n.LastAccessTime,
                    IsDirectory = true
                };
                file.ChildrenFileInfo = GetFileInfoWithLevel(n);
                results.Add(file);
            }
            return results;
        }

        /// <summary>
        /// 从指定目录搜索匹配指定名称的文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dire"></param>
        private void SearchFromDirectory(string name, DirectoryInfo dire, bool isSuffix)
        {
            FileInfo[] files;
            DirectoryInfo[] dires;

            try
            {
                if (isSuffix)
                    files = dire.GetFiles(string.Format("*.{0}", name));
                else files = dire.GetFiles(string.Format("*{0}*.*", name)); //搜索文件格式
                dires = dire.GetDirectories();
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
                return;
            }

            foreach (FileInfo n in files)
            {
                FileInfoEx file = new FileInfoEx
                {
                    Attributes = n.Attributes,
                    Length = n.Length,
                    FullName = n.FullName,
                    FileName = n.Name,
                    LastAccessTime = n.LastAccessTime
                };
                SearchResult.Add(file);
            }

            if (dires.Count() == 0) return;

            foreach (var item in dires)
            {
                SearchFromDirectory(name, item, isSuffix);
            }
        }
    }
}
