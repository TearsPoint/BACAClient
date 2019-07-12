namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Knowledge
    {
        public int CategoryID { get; set; }

        [StringLength(50)]
        public string CategoryName { get; set; }

        [StringLength(50)]
        public string CategoryOtherName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName="ntext")]
        public string Fulltext { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName="xml")]
        public string FulltextXML { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string IMGPath { get; set; }

        public int IMGs { get; set; }

        public string Intro { get; set; }

        public string Key1 { get; set; }

        public string Key2 { get; set; }

        public string Key3 { get; set; }

        public string Key4 { get; set; }

        public string Key5 { get; set; }

        public string Key6 { get; set; }

        public string Key7 { get; set; }

        public string Key8 { get; set; }

        public int LngId { get; set; }

        [StringLength(0xff)]
        public string PDFName { get; set; }

        [StringLength(0xff)]
        public string PDFPath { get; set; }

        public int SourceID { get; set; }

        public int TypeId { get; set; }

        [StringLength(50)]
        public string TypeName { get; set; }
    }
}

