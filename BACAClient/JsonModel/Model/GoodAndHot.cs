namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class GoodAndHot
    {
        public string CategoryID { get; set; }

        public string CategoryName { get; set; }

        public int counts { get; set; }

        public int LngId { get; set; }

        public string Name { get; set; }

        public string Name_E { get; set; }

        public int SourceId { get; set; }

        public int TypeID { get; set; }

        [StringLength(100)]
        public string typeidlngid { get; set; }
    }
}

