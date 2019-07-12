namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Department
    {
        [StringLength(0xff)]
        public string Address { get; set; }

        [StringLength(50)]
        public string CategoryName { get; set; }

        public double DepartmentId { get; set; }

        [StringLength(0xff)]
        public string DepartmentName { get; set; }

        public double HospitalId { get; set; }

        [StringLength(0xff)]
        public string HospitalName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int Id { get; set; }
    }
}

