namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Users
    {
        [StringLength(50)]
        public string AdminID { get; set; }

        public DateTime CreateTime { get; set; }

        [Required, StringLength(50)]
        public string Department { get; set; }

        [StringLength(50)]
        public string Id { get; set; }

        [Required, StringLength(50)]
        public string IP { get; set; }

        public DateTime ModifyTime { get; set; }

        [Required, StringLength(50)]
        public string PassWord { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required, StringLength(50)]
        public string UserType { get; set; }
    }
}

