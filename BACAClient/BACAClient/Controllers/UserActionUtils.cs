namespace BACAClient.Controllers
{
    using BACAClient.Common;
    using BACAClient.Model;
    using BACAClient.UserControls.Controllers;
    using System;

    public class UserActionUtils
    {
        public static BACAClient.Model.MESSAGE MESSAGE = new BACAClient.Model.MESSAGE();

        public static bool DeleteCollection(string userId, Knowledge model)
        {
            Collection collection = new Collection
            {
                UserID = userId,
                TypeID = model.TypeId,
                LngId = model.LngId
            };
            bool flag = false;
            if (flag)
            {
                BACAClient.UserControls.Controllers.OpenWindows.Prompt(MESSAGE.RemoveSuccess);
                return flag;
            }
            BACAClient.UserControls.Controllers.OpenWindows.Prompt(MESSAGE.RemoveError);
            return flag;
        }

        public static void DownLoad(string pdfpath, string pdfname, string typename, string title)
        {
            string configer = ConfigerHelper.GetConfiger(new ConfigerParameterName().Resource);
            pdfpath = string.Format(@"{0}\{1}", configer, pdfpath);
            switch (DownLoadUtils.DownloadPDF(pdfpath, pdfname, typename, title))
            {
                case SystemEnum.DownloadType.ERROR:
                    BACAClient.UserControls.Controllers.OpenWindows.Prompt(MESSAGE.DownLoadError);
                    break;

                case SystemEnum.DownloadType.SUCCESS:
                    BACAClient.UserControls.Controllers.OpenWindows.Prompt(MESSAGE.DownLoadSuccess);
                    break;
            }
        }

        public static void WriteCollection(string userId, Knowledge model)
        {
            Collection collection = new Collection
            {
                UserID = userId,
                TypeID = model.TypeId,
                LngId = model.LngId
            }; 
        }

        public static bool WriteFeedback(BACAClient.Model.Feedback model)
        {
            bool flag = true; 
            BACAClient.UserControls.Controllers.OpenWindows.Prompt(MESSAGE.Error);
            return flag;
        }

        public static int WriteGood(string countnow, string userid, Knowledge knowledge)
        {
            Good model = new Good
            {
                UserID = userid,
                TypeID = knowledge.TypeId,
                LngId = knowledge.LngId,
                CategoryID = knowledge.CategoryName,
                FullText = 0
            };
            int num = 0; 
            BACAClient.UserControls.Controllers.OpenWindows.Prompt(MESSAGE.GoodError);
            return num;
        }

        public static void WriteHotSpot(string UserID, int FullText, Knowledge knowledge)
        {
            HotSpot model = new HotSpot
            {
                UserID = UserID,
                TypeID = knowledge.TypeId,
                LngId = knowledge.LngId,
                CategoryID = knowledge.CategoryName,
                FullText = FullText
            };
            //todo
        }

        public static void WriteSearchHistory(int TypeId, string TypeName, string keyword, string category, string source, string userId)
        {
            Search model = new Search
            {
                TypeID = new int?(TypeId),
                TypeName = TypeName,
                SearchInfo = keyword,
                CategoryID = category,
                Source = source,
                UserID = userId
            };
        }
    }
}

