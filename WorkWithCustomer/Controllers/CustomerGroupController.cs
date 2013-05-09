using System;
using System.Linq;
using System.Web.Mvc;
using WorkWithCustomer.Models;

namespace WorkWithCustomer.Controllers
{
    public class CustomerGroupController : Controller
    {
        private readonly WorkWithCustomerEntities _db = new WorkWithCustomerEntities();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currntPosition"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public ViewResult CusgrpPopup(int? currntPosition, int? position2)
        {
            var cusgrp = _db.CustomerGroups.OrderBy(cusgr => cusgr.cusgrp).ToList();
            var customerGroups = (from data in _db.CustomerGroups.OrderBy(cust => cust.cusgrp)
                                          select data).Skip(currntPosition == null ? 0 : Convert.ToInt16(currntPosition)).Take(10).ToList();

            ViewBag.HasPrevious = (!((currntPosition == null || currntPosition == 0) || currntPosition <= (position2 ?? 0)));
            ViewBag.HasMore = ((currntPosition ?? 0) + 10 < cusgrp.Count);
            ViewBag.CurrentPage = (currntPosition ?? 0) + 10;
            ViewBag.position2 = (position2 ?? 0);
            ViewBag.popupPage = "CustomerGroup/CusGrpPopup";
            return View(customerGroups.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="firstRow"></param>
        /// <param name="hasPrevious"></param>
        /// <returns></returns>
        [HttpPost]
        public ViewResult CusgrpPopup(FormCollection formCollection, string firstRow, bool hasPrevious)
        {
            var firstrow = firstRow.TrimEnd().ToUpper();
            var key = formCollection[0].TrimEnd().ToUpper();
            var cusgrp = _db.CustomerGroups.OrderBy(cusgr => cusgr.cusgrp).ToList();
            var customerGroups = (from data in _db.CustomerGroups.OrderBy(cust => cust.cusgrp)
                                          where (String.Compare(data.cusgrp.TrimEnd(), key) >= 0)
                                          select data).Take(10).ToList();
            if (customerGroups.Count == 0)
            {
                customerGroups = (from data in _db.CustomerGroups.OrderBy(slmn => slmn.cusgrp)
                         where (String.Compare(data.cusgrp.TrimEnd(), firstrow) >= 0)
                         select data).Take(10).ToList();
            }
            var index = cusgrp.TakeWhile(csgrp => customerGroups[0].cusgrp.TrimEnd() != csgrp.cusgrp.TrimEnd()).Count();
            ViewBag.HasPrevious = false;
            if (hasPrevious)
                ViewBag.HasPrevious = true;
            ViewBag.CurrentPage = index + 10;
            ViewBag.position2 = index;
            ViewBag.HasMore = (index + 10 < cusgrp.Count);
            ViewBag.popupPage = "CustomerGroup/CusGrpPopup";
            return View(customerGroups);
        }
    }
}
