using System;
using System.Linq;
using System.Web.Mvc;
using WorkWithCustomer.Models;

namespace WorkWithCustomer.Controllers
{
    public class DistributorController : Controller
    {
        private readonly WorkWithCustomerEntities _db = new WorkWithCustomerEntities();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currntPosition"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public ViewResult DistsPopup(int? currntPosition, int? position2)
        {
            var dists = _db.Distributors.OrderBy(dist => dist.code).ToList();
            var distributor = (from data in _db.Distributors.OrderBy(dist => dist.code)
                                        select data).Skip(currntPosition == null ? 0 : Convert.ToInt16(currntPosition)).Take(10).ToList();

            ViewBag.HasPrevious = (!((currntPosition == null || currntPosition == 0) || currntPosition <= (position2 ?? 0)));
            ViewBag.HasMore = ((currntPosition ?? 0) + 10 < dists.Count);
            ViewBag.CurrentPage = (currntPosition ?? 0) + 10;
            ViewBag.position2 = (position2 ?? 0);
            ViewBag.popupPage = "Distributor/DISTSPopup";
            return View(distributor.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="firstRow"></param>
        /// <param name="hasPrevious"></param>
        /// <returns></returns>
        [HttpPost]
        public ViewResult DistsPopup(FormCollection formCollection, string firstRow, bool hasPrevious)
        {
            var firstrow = firstRow.TrimEnd().ToUpper();
            var key = formCollection[0].TrimEnd().ToUpper();
            var dists = _db.Distributors.OrderBy(dist => dist.code).ToList();
            var distributor = (from data in _db.Distributors.OrderBy(dist => dist.code)
                                        where (String.Compare(data.code.TrimEnd(), key) >= 0)
                                        select data).Take(10).ToList();
            ViewBag.HasPrevious = false;
            if (hasPrevious && distributor.Count == 0)
                ViewBag.HasPrevious = true;
            if (distributor.Count == 0)
            {
                distributor = (from data in _db.Distributors.OrderBy(dist => dist.code)
                         where (String.Compare(data.code.TrimEnd(), firstrow) >= 0)
                         select data).Take(10).ToList();

            }
            var index = dists.TakeWhile(dist => distributor[0].code.TrimEnd() != dist.code.TrimEnd()).Count();
            ViewBag.CurrentPage = index + 10;
            ViewBag.position2 = index;
            ViewBag.HasMore = (index + 10 < dists.Count);
            ViewBag.popupPage = "Distributor/DISTSPopup";
            return View(distributor);
        }
    }
}
