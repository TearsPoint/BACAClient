namespace BACAClient.Model
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class History
    {
        public DateTime CreateTime { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }
    }
}

