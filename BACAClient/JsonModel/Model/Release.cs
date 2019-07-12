namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Release
    {
        public string AdminId { get; set; }

        public DateTime CreateTime { get; set; }

        public int KnowledgeId { get; set; }

        public int LngId { get; set; }

        public DateTime ModifyTime { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=0), System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int ReleaseId { get; set; }

        public int Status { get; set; }

        public int TypeId { get; set; }

        public string UserId { get; set; }
    }
}

