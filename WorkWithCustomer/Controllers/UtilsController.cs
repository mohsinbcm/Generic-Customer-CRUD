using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WorkWithCustomer.Controllers
{
    public class UtilsController : Controller
    {
        /// <summary>
        ///Contains Code for the Date and Time Displayed on the Layout Header
        ///Default Action:
        ///GET /WorkWithCustomer/GetServerTime
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServerTime()
        {
            return Content(DateTime.Today.DayOfWeek + " " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
        }

        
    }
}