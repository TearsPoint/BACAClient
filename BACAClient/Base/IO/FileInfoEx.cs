using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Base.IO
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfoEx
    {
        private FileAttributes _attrubutes;
        public FileAttributes Attributes
        {
            get { return _attrubutes; }
            set { _attrubutes = value; }
        }

        private long _length;
        public long Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        private DateTime _lastAccessTime;
        public DateTime LastAccessTime
        {
            get { return _lastAccessTime; }
            set { _lastAccessTime = value; }
        }

        public bool IsDirectory { get; set; }

        public IList<FileInfoEx> ChildrenFileInfo { get; set; }

        /// <summary>
        /// 自动根据 Length属性 解析大小（单位为b\Kb\Mb）
        /// </summary>
        public string Size
        {
            get
            {
                string size = "";
                if (Length / 1024.0 == 0)
                    size = Length.ToString() + " B";
                else if (Length / 1024.0 / 1024.0 == 0)
                    size = Convert.ToString(Length / 1024.0) + " KB";
                else
                    size = Convert.ToString((Length / 1024.0 / 1024.0)) + " MB";
                return size;
            }
        }
    }
}
