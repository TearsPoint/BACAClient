namespace BACAClient.Model
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Node
    {
        public string Content { get; set; }

        public string Name { get; set; }

        public string Name_c { get; set; }

        public SystemEnum.NODETYPE Type { get; set; }
    }
}

