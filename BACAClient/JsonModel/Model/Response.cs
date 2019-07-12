namespace BACAClient.Model
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Response
    {
        public int CollectionCount { get; set; }

        public int CurPage { get; set; }

        public int DataErrorCount { get; set; }

        public int DepExpertsCount { get; set; }

        public string Express { get; set; }

        public int FullTextCount { get; set; }

        public int GeneralUserCount { get; set; }

        public int ImproveSuggestCount { get; set; }

        public int KnowledgeCount { get; set; }

        public int NoRelease { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int ProgramErrorCount { get; set; }

        public int RecordCount { get; set; }

        public int Release { get; set; }

        public int SearchCount { get; set; }

        public int SystemManageCount { get; set; }

        public string TableName { get; set; }
    }
}

