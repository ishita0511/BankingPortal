using BankingPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankingPortal.Controllers
{
    public class BalanceController : Controller
    {
        // GET: Balance
        public ActionResult Index()
        {
            return View();
        }

        // GET: Balance/Details/5
        public ActionResult Details()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            BankPortalEntities db = new BankPortalEntities();
            long l = long.Parse((string)Session["AccountNumber"]);
            var bal = db.balances.Where(x => x.AccountNumber == l).FirstOrDefault();
            return View(bal);
        }


    }
}