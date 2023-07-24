using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BankingPortal.Models;

namespace BankingPortal.Controllers
{
    public class Netbanking2Controller : Controller
    {
        BankPortalEntities db = new BankPortalEntities();
        // GET: Netbanking2
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(UserInfo u)
        {
            if (TempData["mydata"] != null)
            {

                if ((u.OTP).ToString() == TempData["mydata"].ToString())
                {
                    db.UserInfoes.Add(u);
                    db.SaveChanges();
                    UserRole us = new UserRole();
                    us.AccountNumber = u.AccountNumber;
                    us.Role = "Customer";
                    db.UserRoles.Add(us);
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('OTP confimed successfully');</script>";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    TempData["msg"] = "<script>alert('Incorrect OTP, Please Try Again!!');</script>";
                    return RedirectToAction("Index", "NetBanking");
                }
            }
            else
            {
                return RedirectToAction("Index", "NetBanking");
            }
           
        }
    }
}