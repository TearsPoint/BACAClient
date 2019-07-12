using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACAClient.Common
{
    public class PageUtils
    {
        public static int GetPageCount(int counts)
        {
            int pageSize = GetPageSize();
            return (int)Math.Ceiling((decimal)(counts / GetPageSize()));
        }

        public static int GetPageSize()
        {
            string configer = ConfigerHelper.GetConfiger(new ConfigerParameterName().PageSize);
            if (string.IsNullOrEmpty(configer))
            {
                configer = SystemEnum.Acquiescence.PageSize.ToString();
            }
            return int.Parse(configer);
        }
    }

}
