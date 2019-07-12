namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Announcement
    {
        public int CategoryID { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=2)]
        public string Content { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=4)]
        public DateTime CreateTime { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=0), System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=3), StringLength(50)]
        public string IP { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=5)]
        public DateTime ModifyTime { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=1)]
        public string Title { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }
    }
}

