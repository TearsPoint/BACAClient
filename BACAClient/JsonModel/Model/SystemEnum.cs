namespace BACAClient.Model
{
    using System;

    public class SystemEnum
    {
        public enum Acquiescence
        {
            AutoCloseTimer = 1,
            Counts = 100,
            Extension = 0,
            HistorySaveDays = 30,
            PageSize = 10,
            ReutrnXMLMore = 0
        }

        public enum DLLTYPE
        {
            NOTDLL = -1,
            OPENPROGECT = 0,
            RETURNXML = 1
        }

        public enum DownloadType
        {
            CANCEL = -1,
            ERROR = 0,
            SUCCESS = 1
        }

        public enum FeedbackStatus
        {
            Processing,
            Processed
        }

        public enum FeedbackType
        {
            ImproveSuggest,
            ProgramError,
            DataError
        }

        public enum HotSpotType
        {
            Knowledge,
            FullText
        }

        public enum ISDLL
        {
            GENERAL,
            OPENPROGECT,
            RETURNXML
        }

        public enum ISRETURN
        {
            NO,
            YES
        }

        public enum LogType
        {
            Error,
            SQLError,
            PDFError,
            DownLoadError,
            OfficeError,
            POSTError,
            Local
        }

        public enum NODETYPE
        {
            COLUMN,
            XMLNODE,
            CORRESPOND
        }

        public enum OperType
        {
            Add,
            Edit,
            Delete
        }

        public enum RECOMMENDTYPE
        {
            ADD,
            UPDATE,
            RELEASE
        }

        public enum RELEASESTATUS
        {
            ALL = -1,
            NORELEASE = 0,
            RELEASE = 1,
            RELEASEENDUPDATE = 2
        }

        public enum ReutrnXMLMore
        {
            NO,
            YES
        }

        public enum SolrDoType
        {
            EXIST = 6,
            LOGIN = 5,
            MAX = 7,
            READ = 2,
            REMOVE = 3,
            UPDATE = 4,
            WRITE = 1
        }

        public enum SolrReturnInfoType
        {
            Existed,
            Success,
            Error
        }

        public enum SolrType
        {
            DoSearch,
            GetData,
            DoGood,
            DoCollection,
            DoSearchHistory,
            DoFeedback,
            DoUsers,
            DoAnnouncement,
            DoCount,
            DoKnowledge,
            DoSelfResource,
            DoSolrKnowledge
        }

        public enum Sort
        {
            ASC = 2,
            DESC = 1
        }

        public enum SOURCETYPE
        {
            ALL = -1,
            OTHER = 1,
            SELF = 9,
            SYSTEM = 0
        }

        public enum TYPE
        {
            All,
            Disease,
            Laboratory,
            Evidence,
            ClinicalPathways,
            DisResearch,
            ClinicalCase,
            Article,
            DiseaseID,
            gotokdp,
            LabId
        }

        public enum UserType
        {
            Super,
            SystemManage,
            DepExperts,
            GeneralUser
        }
    }
}

