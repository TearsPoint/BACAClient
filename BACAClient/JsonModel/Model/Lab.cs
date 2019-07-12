namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Lab
    {
        public string CategoryIDs { get; set; }

        public string CategoryNames { get; set; }

        public int Flag { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(0xff)]
        public string LabId { get; set; }

        [StringLength(0xff)]
        public string LabName { get; set; }

        [StringLength(0xff)]
        public string LabUnit { get; set; }

        public string LngIds { get; set; }

        public int Num { get; set; }

        public int TypeId { get; set; }
    }
}

