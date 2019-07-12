namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Feedback
    {
        public string Category { get; set; }

        public DateTime CreateTime { get; set; }

        public string Description { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=0), System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Knowledge { get; set; }

        [StringLength(50)]
        public string LngId { get; set; }

        public string NodeName { get; set; }

        public int PageIndex { get; set; }

        [Required, StringLength(50)]
        public string PageName { get; set; }

        public string SearchInfo { get; set; }

        [StringLength(50)]
        public string SearchType { get; set; }

        public int Status { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(50)]
        public string TypeID { get; set; }
    }
}

