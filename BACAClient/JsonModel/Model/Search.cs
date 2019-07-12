namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Search
    {
        [StringLength(0xff)]
        public string CategoryID { get; set; }

        public DateTime CreateTime { get; set; }

        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string IP { get; set; }

        public string SearchInfo { get; set; }

        [StringLength(50)]
        public string Source { get; set; }

        public int? TypeID { get; set; }

        [StringLength(50)]
        public string TypeName { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }
    }
}

