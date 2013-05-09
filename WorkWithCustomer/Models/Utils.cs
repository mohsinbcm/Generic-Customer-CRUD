using System;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.Web.Configuration;

namespace WorkWithCustomer.Models
{
    public static class Utils
    {
        public static NameValueCollection MessagesCol;

        public static string Username
        {
            get
            {
                var conf = WebConfigurationManager.OpenWebConfiguration("/");
                return
                    conf.ConnectionStrings.ConnectionStrings[1].ConnectionString.Substring(223,
                                                                                           (conf.ConnectionStrings.
                                                                                                ConnectionStrings[1].
                                                                                                ConnectionString.IndexOf
                                                                                                (";Password") - 223)).
                        ToUpper();
            }
        }

        /// <summary>
        /// Method to load custom messages from a message file
        /// </summary>
        public static void LoadMessages()
        {
            var sr = new StreamReader(HttpContext.Current.Server.MapPath("~\\Models\\messages_en.properties"));
            var fileContents = sr.ReadToEnd();
            var sepr = new[] { "\r\n" };
            var lines = fileContents.Split(sepr, StringSplitOptions.RemoveEmptyEntries);
            MessagesCol = new NameValueCollection();
            foreach (var line in lines)
            {
                int index = line.IndexOf('=');
                if (index != -1)
                {
                    MessagesCol.Add(line.Substring(0, index), line.Substring(index + 1));
                }
            }
            sr.Close();
        }
        //public static void PushNavigationInfo(string page)
        //{
        //    if(NavigationList==null)
        //    {
        //        NavigationList=new List<string>();
        //    }
        //    NavigationList.Add(page);
        //}
        //public static string PopNavigationInfo()
        //{
        //    return NavigationList.Count == 0 ? "/Login/Index" : NavigationList[NavigationList.Count - 2];
        //}
    }
}
