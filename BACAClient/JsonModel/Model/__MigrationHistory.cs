namespace BACAClient.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class __MigrationHistory
    {
        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=1), StringLength(300)]
        public string ContextKey { get; set; }

        [Key, System.ComponentModel.DataAnnotations.Schema.Column(Order=0), StringLength(150)]
        public string MigrationId { get; set; }

        [Required]
        public byte[] Model { get; set; }

        [Required, StringLength(0x20)]
        public string ProductVersion { get; set; }
    }
}

