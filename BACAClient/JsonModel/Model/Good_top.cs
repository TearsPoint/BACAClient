namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Good_top
    {
        [StringLength(50)]
        public string CategoryID { get; set; }

        [StringLength(50)]
        public string CategoryName { get; set; }

        public int counts { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=1), System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int LngId { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=2)]
        public string Name { get; set; }

        public string Name_E { get; set; }

        public int SourceId { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=0), System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int TypeID { get; set; }

        [StringLength(100)]
        public string typeidlngid { get; set; }
    }
}

