namespace BACAClient.Model
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Statistics
    {
        public int Collection { get; set; }

        public DateTime CreateTime { get; set; }

        public int FullText { get; set; }

        public int Knowledge { get; set; }

        public int Search { get; set; }
    }
}

