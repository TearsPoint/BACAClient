using Base.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACAClient.Repository
{

    /// <summary>
    /// 仓储中心
    /// </summary>
    public static class RestRC
    { 
        /// <summary>
        /// 
        /// </summary>
        public static BACARepository BACARepository
        {
            get
            {
                return RestRC<BACARepository>.Repository;
            }
        }
    }

    public class BACARepository : RB
    {
        public override string SvcName { get { return string.Empty; } }

        public override DataServiceGroup DataServiceGroup { get { return DataServiceGroup.Default; } }


    }
}
