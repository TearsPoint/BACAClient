namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class HotSpot_top
    {
        [StringLength(50)]
        public string CategoryID { get; set; }

        public DateTime CreateTime { get; set; }

        public int FullText { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=0), System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        public int LngId { get; set; }

        public int TypeID { get; set; }

        [StringLength(100)]
        public string typeidlngid { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }
    }
}

