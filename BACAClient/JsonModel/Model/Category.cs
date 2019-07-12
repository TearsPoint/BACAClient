namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Category
    {
        public int CategoryID { get; set; }

        [Required, StringLength(50)]
        public string CategoryName { get; set; }

        public int DBId { get; set; }

        public int ShowFlag { get; set; }

        public int SortId { get; set; }

        public int TypeId { get; set; }
    }
}

