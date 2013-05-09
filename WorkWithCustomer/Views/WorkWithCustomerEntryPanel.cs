using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkWithCustomer.Models;

namespace WorkWithCustomer.Views
{
    public abstract class WorkWithCustomerEntryPanel : WebViewPage<Purchases>
    {
        readonly WorkWithCustomerEntities _db = new WorkWithCustomerEntities();
        public static Purchases purchases;
        public static CustomerGroups customerGrp;
        public static Salespersons salesPersons;
        public static Distributors distributor;
        private string _idkey;

        public string onPreRenderer()
        {
            FormCollection forColl = WorkWithCustomer.Controllers.WorkWithCustomerController.frmColl;
            for (int index = 1; index < (forColl.Count - 1); index++)
            {
                if (forColl[index].Split(',').ToArray()[0].Trim() == "true")
                {
                    _idkey = forColl.AllKeys[index];
                }
            }
            purchases = _db.Purchases.Find(_idkey);
            customerGrp = GetCusgrp(purchases.cusgrp);
            salesPersons = GetRep(purchases.rep);
            distributor = GetDistributor(purchases.distributor);
            return string.Empty;
        }

        private CustomerGroups GetCusgrp(String cusgrpCode)
        {
            var cus = (_db.CustomerGroups.Where(data => data.cusgrp == cusgrpCode)).ToList();
            return cus.First();
        }

        private Salespersons GetRep(String repCode)
        {
            var rep = (_db.Salesreps.Where(data => data.person == repCode)).ToList();
            return rep.First();
        }

        private Distributors GetDistributor(String disCode)
        {
            var dis = (_db.Distributors.Where(data => data.code == disCode)).ToList();
            return dis.First();
        }

    }
}