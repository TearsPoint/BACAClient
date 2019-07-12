namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class XMLNodeName
    {
        public string Abbreviation { get; set; }

        public int CategoryID { get; set; }

        [StringLength(50)]
        public string CategoryName { get; set; }

        [StringLength(50)]
        public string CategoryOtherName { get; set; }

        public string Fulltext { get; set; }

        public string ICD { get; set; }

        public string IMGPath { get; set; }

        public int IMGs { get; set; }

        public int LngId { get; set; }

        public string Name { get; set; }

        public string Name_A { get; set; }

        public string Name_E { get; set; }

        [StringLength(0xff)]
        public string PDFName { get; set; }

        [StringLength(0xff)]
        public string PDFPath { get; set; }
    }
}

