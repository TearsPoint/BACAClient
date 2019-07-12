using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DomainModel
{ 
    [DataContract]
    public class ColInfo
    {
        [DataMember]
        public string ColName { get; set; }

        [DataMember]
        public int ColIndex { get; set; }
    } 
}
