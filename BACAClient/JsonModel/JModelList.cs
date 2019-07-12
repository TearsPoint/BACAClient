using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonModel
{
    /// <summary>
    /// 
    /// </summary>
    public static class JModelList
    {
        public static IDictionary<string, JMI> Models = new Dictionary<string, JMI>();

        static JModelList()
        {
            Models[JModelNo.TB0000] = new JMI(JModelNo.TB0000, "mdt.Test", "mdt.Test_SEQ", typeof(DtoTest));
        }


    }
}
