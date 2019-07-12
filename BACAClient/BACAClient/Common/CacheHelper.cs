using BACAClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Windows;

namespace BACAClient.Common
{
    public class CacheHelper
    {
        public static string GetCache(string CacheKey)
        {
            try
            {
                string str = string.Empty;
                if (!string.IsNullOrEmpty(CacheKey) && (Application.Current.Properties[CacheKey] != null))
                {
                    str = Application.Current.Properties[CacheKey].ToString();
                }
                return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static InterfacesUsers GetInterfacesUsersModel()
        {
            try
            {
                CacheParameterName name = new CacheParameterName();
                return new InterfacesUsers { UserId = GetCache(name.LoginUserID), UserName = GetCache(name.UserName), DepName = GetCache(name.UserDepName) };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ReturnSearch GetReturnSearchModel()
        {
            try
            {
                CacheParameterName name = new CacheParameterName();
                ReturnSearch search = new ReturnSearch
                {
                    IsReturn = GetCache(name.IsReturn)
                };
                if (!string.IsNullOrEmpty(GetCache(name.TypeId)))
                {
                    search.TypeId = int.Parse(GetCache(name.TypeId));
                }
                if (!string.IsNullOrEmpty(GetCache(name.SourceId)))
                {
                    search.SourceId = int.Parse(GetCache(name.SourceId));
                }
                if (!string.IsNullOrEmpty(GetCache(name.PageIndex)))
                {
                    search.PageIndex = int.Parse(GetCache(name.PageIndex));
                }
                if (!string.IsNullOrEmpty(GetCache(name.Isdell)))
                {
                    search.Isdell = int.Parse(GetCache(name.Isdell));
                }
                search.Key = GetCache(name.Key);
                search.KeyWord = GetCache(name.keyword);
                search.Category = GetCache(name.Category);
                search.Pages = GetCache(name.Pages);
                search.UserId = GetCache(name.LoginUserID);
                return search;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void RemoveAllCache()
        {
            try
            {
                CacheParameterName name = new CacheParameterName();
                SetCache(name.IsReturn, 0.ToString());
                SetCache(name.TypeId, string.Empty);
                SetCache(name.SourceId, (-1).ToString());
                SetCache(name.PageIndex, "1");
                SetCache(name.Key, string.Empty);
                SetCache(name.keyword, string.Empty);
                SetCache(name.Category, string.Empty);
                SetCache(name.Pages, string.Empty);
                SetCache(name.Isdell, (-1).ToString());
            }
            catch
            {
            }
        }

        public static void RemoveAllCache(string CacheKey)
        {
            HttpRuntime.Cache.Remove(CacheKey);
        }

        public static void SetCache(string CacheKey, string Parameter)
        {
            try
            {
                Application.Current.Properties[CacheKey] = Parameter;
            }
            catch
            {
            }
        }

        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject, null, absoluteExpiration, Cache.NoSlidingExpiration);
        }

        public static void SetCache(string CacheKey, object objObject, TimeSpan Timeout)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject, null, DateTime.MaxValue, Timeout, CacheItemPriority.NotRemovable, null);
        }

        public static void SetDellInfo(InterfacesUsers model)
        {
            try
            {
                if (model != null)
                {
                    CacheParameterName name = new CacheParameterName();
                    SetCache(name.LoginUserID, model.UserId);
                    SetCache(name.UserName, model.UserName);
                    SetCache(name.UserDepName, model.DepName);
                }
            }
            catch
            {
            }
        }

        public static void SetReturnSearch(ReturnSearch model)
        {
            try
            {
                CacheParameterName name = new CacheParameterName();
                SetCache(name.IsReturn, model.IsReturn);
                SetCache(name.TypeId, model.TypeId.ToString());
                SetCache(name.SourceId, model.SourceId.ToString());
                SetCache(name.PageIndex, model.PageIndex.ToString());
                SetCache(name.Key, model.Key);
                SetCache(name.keyword, model.KeyWord);
                SetCache(name.Category, model.Category);
                SetCache(name.Pages, model.Pages);
                SetCache(name.Isdell, model.Isdell.ToString());
            }
            catch
            {
            }
        }
    }


}
