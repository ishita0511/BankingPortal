using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

using System.Web.Mvc;
using BankingPortal.Models;

namespace BankingPortal.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        
        public ActionResult Index()
        {
            return View();
        }

        //Create Account
        [HttpPost]
        public ActionResult Index(Registration r)
        {
            if (ModelState.IsValid == true)
            {
                BankPortalEntities db = new BankPortalEntities();
                r.Status = 0;
                db.Registrations.Add(r);
                db.SaveChanges();

                TempData["msg"] = "<script>alert('Registered Successfully, Account Number will be Sent to your Registered EmailId after Approval ');</script>";
                ModelState.Clear();
                return View();
            }
            else
            {
                TempData["msg"] = "<script>alert('Registration Failed, Try again with valid credentials');</script>";
                return View();
            }
           

        }

       
  


    }
}