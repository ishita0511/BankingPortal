using BankingPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BankingPortal.Controllers
{
    public class NetBankingController : Controller
    {
        BankPortalEntities db = new BankPortalEntities();
        // GET: NetBanking
        public ActionResult Index()
        {
            return View();
        }
       [HttpPost]
        public ActionResult Index(Registration r)
             {
            //check whether user is present in userinfo table or not
               var credentials =db.UserInfoes.Where(m=> m.AccountNumber == r.AccountNumber).FirstOrDefault(); 
            if (credentials == null)
            {
                generateOTP(r);
                TempData["msg"] = "<script>alert('OTP is generated successfully');</script>";
                TempData["account"] = r.AccountNumber.ToString();

                return RedirectToAction("Index", "NetBanking2");
            }
            else
            {
                TempData["msg"]= "<script>alert('User already exist!, Login to continue');</script>";
                return RedirectToAction("Index", "Login");


            }

        }
     
        public void generateOTP(Registration r)
        {
            var credentials = db.Registrations.Where(Models => Models.AccountNumber == r.AccountNumber).FirstOrDefault();
            if (credentials != null)
            {
                string numbers = "0123456789";
                Random objrandom = new Random();
                string strrandom = string.Empty;
                for (int i = 0; i < 5; i++)
                {
                    int temp = objrandom.Next(0, numbers.Length);
                    strrandom += temp;
                }
                TempData["mydata"] = strrandom.ToString();
                TempData.Keep();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("roughusemail2000@gmail.com");
                    mail.To.Add(credentials.Email);
                    mail.Subject = "OTP to Register for Internet Banking";
                    mail.Body += "<br /> Greetings from abc Bank...";
                    mail.Body += "<br /> <br />Please enter the below OTP to verify your account.";
                    mail.Body += "<br /> OTP: " + TempData["mydata"];
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("roughusemail2000@gmail.com", "Garvit@12");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
        }
      

    }
}