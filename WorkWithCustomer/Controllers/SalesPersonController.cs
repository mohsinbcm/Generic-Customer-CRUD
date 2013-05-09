using System;
using System.Linq;
using System.Web.Mvc;
using WorkWithCustomer.Models;

namespace WorkWithCustomer.Controllers
{
    public class SalesPersonController : Controller
    {
        private readonly WorkWithCustomerEntities _db = new WorkWithCustomerEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currntPosition"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public ViewResult SlmenPopup(int? currntPosition, int? position2)
        {
            var slmen = _db.Salesreps.OrderBy(slmn => slmn.person).ToList();
            var salesPersons = (from data in _db.Salesreps.OrderBy(slmn => slmn.person)
                                               select data).Skip(currntPosition == null
                                                                     ? 0
                                                                     : Convert.ToInt16(currntPosition)).Take(10).ToList();

            ViewBag.HasPrevious = (!((currntPosition == null || currntPosition == 0) ||
                                     currntPosition <= (position2 ?? 0)));
            ViewBag.HasMore = ((currntPosition ?? 0) + 10 < slmen.Count);
            ViewBag.CurrentPage = (currntPosition ?? 0) + 10;
            ViewBag.position2 = (position2 ?? 0);
            ViewBag.popupPage = "SalesPerson/SLMENPopup";
            return View(salesPersons.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="firstRow"></param>
        /// <param name="hasPrevious"></param>
        /// <returns></returns>
        [HttpPost]
        public ViewResult SlmenPopup(FormCollection formCollection, string firstRow, bool hasPrevious)
        {
            var firstrow = firstRow.TrimEnd().ToUpper();
            var key = formCollection[0].TrimEnd().ToUpper();
            var slmen = _db.Salesreps.OrderBy(slmn => slmn.person).ToList();
            var salesPersons = (from data in _db.Salesreps.OrderBy(slmn => slmn.person)
                                               where (String.Compare(data.person.TrimEnd(), key) >= 0)
                                               select data).Take(10).ToList();
            if (salesPersons.Count == 0)
            {
                salesPersons = (from data in _db.Salesreps.OrderBy(slmn => slmn.person)
                                where (String.Compare(data.person.TrimEnd(), firstrow) >= 0)
                                select data).Take(10).ToList();
            }
            var index = slmen.TakeWhile(slmn => salesPersons[0].person.TrimEnd() != slmn.person.TrimEnd()).Count();
            ViewBag.HasPrevious = false;
            if (hasPrevious)
                ViewBag.HasPrevious = true;
            ViewBag.CurrentPage = index + 10;
            ViewBag.position2 = index;
            ViewBag.HasMore = (index + 10 < slmen.Count);
            ViewBag.popupPage = "SalesPerson/SLMENPopup";
            return View(salesPersons);
        }
    }
}