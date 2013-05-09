using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using WorkWithCustomer.Models;

namespace WorkWithCustomer.Controllers
{
    /// <summary>
    /// The Class has been made to define actions that the controller will handle
    /// Here, the code for varios actions have been written as various methods of the controller class
    /// this class is conroller specific rather than being a generic class applicable to all the controllers
    /// </summary>
    public class WorkWithCustomerController : Controller
    {
        //Constant value to define the number of records on the Grid Screen
        private const int PageSize = 10;
        //String value to store value of customer for indexing 
        //default entity object used throughout the controller
        private readonly WorkWithCustomerEntities _db = new WorkWithCustomerEntities();
        public static FormCollection frmColl;
        public String ErrorField;
        public String ErrorMessage;
        private string _idkey;

        /// <summary>
        /// Contains code for the default action (index page call)
        /// Action Handled::
        /// GET: /WorkWithCustomer
        /// </summary>
        /// <param name="page"></param>
        /// <param name="filteredIndex"></param>
        /// <returns></returns>
        public ActionResult WorkWithCustomerGrid(int? page, int? filteredIndex)
        {
            PagedItem<Purchases> customers;
            ConfigurationManager.RefreshSection("connectionStrings");
            ViewBag.userName = Utils.Username;
            ViewBag.ValNotFound = false;
            if (page > 0)
                customers = GetPagedCustomers((int)(page), PageSize);
            else
                customers = page == null
                                ? GetPagedCustomers(0, PageSize)
                                : GetPagedCustomers(0, (PageSize + Convert.ToInt32(page)));
            if (filteredIndex == null)
                filteredIndex = 0;
            ViewBag.HasPrevious = page - 10 >= filteredIndex && customers.HasPrevious;
            var table = customers.Entities;
            ViewBag.HasMore = customers.HasNext;
            ViewBag.CurrentPage = (page ?? 0);
            ViewBag.FilteredIndex = filteredIndex;
            if (TempData["postBackError"] != null) TempData["postBackError"] = TempData["postBackError"].ToString();
            return PartialView(table);
        }

        /// <summary>
        /// Contains Code for the filtering action from the Grid Page
        /// Default Action:
        /// POST /WorkWithCustomer
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="rowStart"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WorkWithCustomerGrid(FormCollection formCollection, string rowStart)
        {
            string rowstart = rowStart.TrimEnd().ToUpper();
            TempData["layout"] = "";
            TempData["Result"] = " ";
            List<Purchases> custs = _db.Purchases.OrderBy(cust => cust.customer).ToList();
            String key = formCollection[0].ToUpper();
            List<Purchases> purchases = (from data in _db.Purchases.OrderBy(cust => cust.customer)
                                         where (String.Compare(data.customer.TrimEnd(), key) >= 0)
                                         select data).Take(10).ToList();
            if (purchases.Count == 0)
            {
                purchases = (from data in _db.Purchases.OrderBy(cust => cust.customer)
                             where (String.Compare(data.customer.TrimEnd(), rowstart) >= 0)
                             select data).Take(10).ToList();
            }
            int index = custs.TakeWhile(cust => purchases[0].customer.TrimEnd() != cust.customer.TrimEnd()).Count();
            ViewBag.HasPrevious = false;
            ViewBag.HasMore = (index + 9 < custs.Count);
            ViewBag.CurrentPage = ViewBag.FilteredIndex = index;
            return PartialView(purchases);
        }

        /// <summary>
        /// Contains Code for the Add action on the Grid Screen
        /// Default Action:
        /// GET /WorkWithCustomer/WorkWithCustomerEntryPanel/Add
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkWithCustomerEntryPanel()
        {
            TempData["readonly"] = "";
            if (TempData["redir"].ToString() == "Add") TempData["readonly"] = "false";
            if (TempData["Result"] == null) TempData["Result"] = " ";
            var purchases = (Purchases)TempData["dis"];
            if (purchases == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Create";
            ViewBag.rep = "";
            ViewBag.full_name = "";
            ViewBag.distributor = "";
            ViewBag.DNAME = "";
            ViewBag.cusgrp = "";
            ViewBag.description = "";
            if ((TempData["dis"]) != new Purchases())
            {
                LoadDropDownData(purchases);
            }
            return PartialView(purchases);
        }

        /// <summary>
        /// Contains Code for various action that can be taken on the Grid Screen
        /// Default Action:
        /// POST /WorkWithCustomer/WorkWithCustomerEntryPanel
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WorkWithCustomerEntryPanel(FormCollection formCollection)
        {
            frmColl = formCollection;
            String mode = formCollection[0].Split(',').ToArray()[1].TrimEnd();
            if (mode == "DisplayCustomer")
            {
                TempData["readonly"] = "readonly";
                TempData["redir"] = "Display";
            }
            else if (mode == "DeleteCustomer")
            {
                TempData["readonly"] = "readonly";
                TempData["redir"] = "Delete";
            }
            else if (mode == "AddCustomer")
            {
                TempData["dis"] = new Purchases();
                TempData["redir"] = "Add";
                return RedirectToAction("WorkWithCustomerEntryPanel");
            }
            else
            {
                TempData["readonly"] = "";
                TempData["redir"] = "Edit";
                ViewBag.Title = "WorkWithCustomerEntryPanel";
            }
            for (int index = 1; index < (formCollection.Count - 1); index++)
            {
                if (formCollection[index].Split(',').ToArray()[0].Trim() == "true")
                {
                    _idkey = formCollection.AllKeys[index];
                }
            }
            Purchases purchases = _db.Purchases.Find(_idkey);
            if (_idkey == null)
            {
                TempData["postBackError"] = "Please Select a customer !!";
                return RedirectToAction("WorkWithCustomerGrid", "WorkWithCustomer");
            }
            LoadDropDownData(purchases);
            HttpContext.Application["WorkWithCustomer"] = "WorkWithCustomerEntryPanel";
            //return PartialView(new Purchases());
            return PartialView();
        }

        /// <summary>
        /// Contains code to load the data and description for cusgrp,slmen and dists.
        /// </summary>
        /// <param name="purchases">model object</param>
        public void LoadDropDownData(Purchases purchases)
        {
            ViewBag.rep = purchases.rep.Trim() != "" ? _db.Salesreps.Find(purchases.rep.ToUpper()).person : " ";
            ViewBag.full_name = purchases.rep.Trim() != "" ? _db.Salesreps.Find(purchases.rep.ToUpper()).full_name : " ";

            //length has been checked as it may contain white space as code field value.
            ViewBag.distributor = purchases.distributor.Length != 0
                                      ? _db.Distributors.Find(purchases.distributor.ToUpper()).code
                                      : " ";
            //length has been checked as it may contain white space as code field value.  
            ViewBag.DNAME = purchases.distributor.Length != 0
                                ? _db.Distributors.Find(purchases.distributor.ToUpper()).description
                                : " ";

            ViewBag.cusgrp = purchases.cusgrp.Trim() != ""
                                 ? _db.CustomerGroups.Find(purchases.cusgrp.ToUpper()).cusgrp
                                 : " ";
            ViewBag.description = purchases.cusgrp.Trim() != ""
                                      ? _db.CustomerGroups.Find(purchases.cusgrp.ToUpper()).description
                                      : " ";
        }

        /// <summary>
        /// Contains Code for the Panel Screen
        /// Default Action:
        /// POST /WorkWithCustomer/WorkWithCustomerPanel
        /// </summary>
        /// <param name="purchases">model object</param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WorkWithCustomerPanel(Purchases purchases, FormCollection formCollection)
        {
            TempData["Readonly"] = "";
            TempData["redir"] = formCollection.AllKeys[formCollection.Count - 2];
            Purchases custs = purchases;
            return PartialView(custs);
        }

        /// <summary>
        /// Contains Code for the Cancel Action on all EntryPanel and Panel Screens
        /// Default Action:
        /// GET /WorkWithCustomer/Cancel
        /// </summary>
        /// <returns></returns>
        public ActionResult Cancel()
        {
            TempData["Result"] = " ";

            return RedirectToAction("WorkWithCustomerGrid");
        }

        /// <summary>
        /// Contains Code for the Save action on the Panel Screen
        /// Default Action:
        /// POST /WorkWithCustomer/WorkWithCustomerPanel/Save
        /// </summary>
        /// <param name="purchases"></param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Purchases purchases, FormCollection formCollection)
        {
            string redirectAction;
            if (formCollection.Keys[formCollection.Count - 2] == "Delete")
            {
                Purchases cust = _db.Purchases.Find(purchases.customer.TrimEnd());
                _db.Purchases.Remove(cust);
                _db.SaveChanges();
                TempData["Result"] = "";
                redirectAction = "WorkWithCustomerGrid";
            }
            else
            {
                ValidateModel(purchases, formCollection.Keys[formCollection.Count - 2]);
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (formCollection.Keys[formCollection.Count - 2] == "Add")
                        {
                            purchases.customer = purchases.customer.Trim().ToUpper();
                            _db.Entry(purchases).State = EntityState.Added;
                            _db.SaveChanges();
                            TempData["Result"] = "";
                        }
                        else
                        {
                            Purchases context = _db.Purchases.Find(purchases.customer);
                            if (purchases != context)
                            {
                                _db.Entry(purchases).State = EntityState.Modified;
                                _db.SaveChanges();
                                TempData["Result"] = "";
                            }
                            else
                            {
                                TempData["Result"] = "Nothing to Update!!!";
                            }
                        }
                        redirectAction = "WorkWithCustomerGrid";
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                            {
                                TempData["Result"] = TempData["Result"] + "<br />" + validationError.ErrorMessage;
                            }
                        }
                        TempData["dis"] = purchases;
                        redirectAction = "WorkWithCustomerEntryPanel";
                    }
                }
                else
                {
                    redirectAction = "WorkWithCustomerEntryPanel";
                    if (!ModelState.IsValid)
                    {
                        TempData["redir"] = formCollection.Keys[formCollection.Count - 2];
                        TempData["dis"] = purchases;
                        IEnumerable<string> allElements = from item in ModelState
                                                          where item.Value.Errors.Any()
                                                          select item.Key;
                        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                        foreach (ModelError item in allErrors)
                        {
                            if (ErrorMessage != null)
                            {
                                ErrorMessage = ErrorMessage + Environment.NewLine + item.ErrorMessage;
                            }
                            else
                            {
                                ErrorMessage = item.ErrorMessage;
                            }
                        }
                        foreach (string item in allElements)
                        {
                            if (ErrorField != null)
                            {
                                ErrorField = ErrorField + " " + item;
                            }
                            else
                            {
                                ErrorField = item;
                            }
                        }
                        TempData["errorElement"] = ErrorField;
                        TempData["message"] = ErrorMessage;
                    }
                    else
                    {
                        return PartialView(purchases);
                    }
                }
            }
            return RedirectToAction(redirectAction);
        }

        /// <summary>
        /// Contains Code for the CUSGRP Drop Down List Selection on EntryPanel Screen
        /// Default Action:
        /// GET /WorkWithCustomer/GetCustName
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public ActionResult GetPopupData(string keyValue = " ", string operationType = " ")
        {
            string key = keyValue.ToUpper();
            string name = "";
            switch (operationType)
            {
                case "cusgrp":
                    List<CustomerGroups> getName = (from data in _db.CustomerGroups
                                                    where (String.Compare(data.cusgrp, key) >= 0)
                                                    select data).ToList();
                    if (getName.Count != 0) name = getName.First().cusgrp + "," + getName.First().description;
                    else
                    {
                        getName = (from data in _db.CustomerGroups.OrderBy(csgp => csgp.cusgrp)
                                   select data).Take(1).ToList();
                        name = getName.First().cusgrp + "," + getName.First().description;
                    }
                    break;
                case "rep":
                    List<Salespersons> getSlmen = (from data in _db.Salesreps
                                                   where (String.Compare(data.person.TrimEnd(), key) >= 0)
                                                   select data).ToList();
                    if (getSlmen.Count != 0) name = getSlmen.First().person + "," + getSlmen.First().full_name;
                    else
                    {
                        getSlmen = (from data in _db.Salesreps.OrderBy(slmn => slmn.person)
                                    select data).Take(1).ToList();
                        name = getSlmen.First().person + "," + getSlmen.First().full_name;
                    }
                    break;
                case "distributor":
                    List<Distributors> getDistributor = (from data in _db.Distributors
                                                         where (String.Compare(data.code.TrimEnd(), key) >= 0)
                                                         select data).ToList();
                    if (getDistributor.Count != 0)
                        name = getDistributor.First().code + "," + getDistributor.First().description;
                    else
                    {
                        getDistributor = (from data in _db.Distributors.OrderBy(dist => dist.code)
                                          select data).Take(1).ToList();
                        name = getDistributor.First().code + "," + getDistributor.First().description;
                    }
                    break;
            }
            return Content(name);
        }

        /// <summary>
        /// Method to validate the model(Purchases)
        /// </summary>
        /// <param name="purchases"></param>
        /// <param name="mode"></param>
        [AcceptVerbs(HttpVerbs.Post)]
        public void ValidateModel(Purchases purchases, String mode)
        {
            if (Utils.MessagesCol == null)
                Utils.LoadMessages();
            var errMsg = new string[] { };
            if (purchases.credit_limit < purchases.balance)
            {
                if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OES0373");
                if (errMsg != null)
                {
                    string st = errMsg[0] + ". The Balance amount is:" + purchases.credit_limit +
                                " & the Credit Limit is:" + purchases.balance;
                    ModelState.AddModelError("balance", st);
                }
            }
            if (purchases.name.Trim().Length <= 0)
            {
                if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OEM0012");
                if (errMsg != null) ModelState.AddModelError("name", errMsg[0]);
            }
            if (purchases.prospect_no == null)
            {
                if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OEM0024");
                if (errMsg != null) ModelState.AddModelError("prospect_no", errMsg[0]);
            }
            if (purchases.cusgrp.Trim() == "")
            {
                if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OEM0020");
                if (errMsg != null) ModelState.AddModelError("cusgrp", errMsg[0]);
            }
            if (purchases.rep.Trim() == "")
            {
                if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OEM0021");
                if (errMsg != null) ModelState.AddModelError("rep", errMsg[0]);
            }
            //length has been checked as it may contain white space as code field value.
            if (purchases.distributor.Length == 0)
            {
                if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OEM0022");
                if (errMsg != null) ModelState.AddModelError("distributor", errMsg[0]);
            }
            if (mode == "Add")
            {
                if (purchases.customer.Trim().Length <= 0)
                {
                    if (Utils.MessagesCol != null) errMsg = Utils.MessagesCol.GetValues("OEM0003");
                    if (errMsg != null) ModelState.AddModelError("customer", errMsg[0]);
                }
                else
                {
                    Purchases cust = _db.Purchases.Find(purchases.customer.TrimEnd());
                    if (cust != null)
                    {
                        if (Utils.MessagesCol != null)
                            errMsg = Utils.MessagesCol.GetValues("Y2U0003");
                        if (errMsg != null) ModelState.AddModelError("customer", errMsg[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Method to query specific number of records from the database.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public PagedItem<Purchases> GetPagedCustomers(int skip, int take)
        {
            IOrderedQueryable<Purchases> purchases = _db.Purchases.OrderBy(c => c.customer);
            int customerCount = purchases.Count();
            List<Purchases> customers = purchases.Skip(skip).Take(take).ToList();
            return new PagedItem<Purchases>
                       {
                           Entities = customers,
                           HasNext = (skip + 10 < customerCount),
                           HasPrevious = (skip > 0)
                       };
        }
    }
}