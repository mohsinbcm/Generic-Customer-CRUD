using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WorkWithCustomer.Models;

namespace WorkWithCustomer.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            Configuration conf = WebConfigurationManager.OpenWebConfiguration("/");
            conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString =
                conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString.Substring(0, 214) +
                "\"";
            conf.Save();
            return PartialView();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            string userName = formCollection[2].Trim();
            string password = formCollection[3].Trim();
            var context = new WorkWithCustomerEntities();
            var adapter = (IObjectContextAdapter) context;
            ObjectContext objectContext = adapter.ObjectContext;
            var entityConnection = objectContext.Connection as EntityConnection;
            if (entityConnection != null)
            {
                string connectionString = entityConnection.StoreConnection.ConnectionString;
                connectionString = connectionString + ";USER ID=" + userName + ";Password=" + password + ";";
                entityConnection.StoreConnection.ConnectionString = connectionString;
            }
            try
            {
                objectContext.Connection.Open();
                Configuration conf = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
                conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString =
                    conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString.Split('\"')[0] +
                    "\"" +
                    conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString.Split('\"')[1] +
                    ";USER ID=" + userName + ";Password=" + password + "\"";
                conf.Save();
                ConfigurationManager.RefreshSection("connectionStrings");
                context.Database.Initialize(true);
                return RedirectToAction("WorkWithCustomerGrid", "WorkWithCustomer");
            }
            catch (Exception e)
            {
                if (e.InnerException.ToString().Contains("USERNAME"))
                {
                    TempData["error"] = "USERNAME AND/OR PASSWORD INVALID";
                }
                else if (e.InnerException.Message.Contains("PASSWORD MISSING") ||
                         e.InnerException.Message.Contains("USERID MISSING"))
                {
                    TempData["error"] = "USERNAME AND/OR PASSWORD MISSING";
                }
                else
                {
                    TempData["error"] = "An error occurred please try again later.";
                }
                Configuration conf = WebConfigurationManager.OpenWebConfiguration("/");
                conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString =
                    conf.ConnectionStrings.ConnectionStrings["WorkWithCustomerEntities"].ConnectionString.Substring(0,
                                                                                                                    214) +
                    "\"";
                conf.Save();
            }
            return PartialView();
        }
    }
}