namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class ICD
    {
        public string CategoryIDs { get; set; }

        public string CategoryNames { get; set; }

        [StringLength(0xff)]
        public string DiseaseID { get; set; }

        [StringLength(0xff)]
        public string DisName { get; set; }

        public int Flag { get; set; }

        [StringLength(0xff)]
        public string ICD10 { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string LngIds { get; set; }

        [StringLength(0xff)]
        public string MnemonicCode { get; set; }

        public int Num { get; set; }

        public int TypeId { get; set; }
    }
}

