namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class HIS_Relational
    {
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string IMGPath { get; set; }

        public int IMGs { get; set; }

        [StringLength(50)]
        public string LngId { get; set; }

        [StringLength(0xff)]
        public string PDFName { get; set; }

        [StringLength(0xff)]
        public string PDFPath { get; set; }

        public int RecordIndex { get; set; }

        [StringLength(50)]
        public string ReLngId { get; set; }

        [StringLength(50)]
        public string ReName { get; set; }

        public string ReTitle { get; set; }

        public int ReTypeId { get; set; }

        public int? TypeId { get; set; }
    }
}

