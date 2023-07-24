using BankingPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BankingPortal.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        BankPortalEntities db = new BankPortalEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserInfo s)
        {
            
                var credentials = db.UserInfoes.Where(Models => Models.UserName == s.UserName && Models.Password == s.Password).FirstOrDefault();
                if (credentials == null)
                {
                    TempData["msg"] = "<script>alert('Login Failed');</script>";
              
                    return View();
                }
                else
                {
                FormsAuthentication.SetAuthCookie(credentials.UserName, false);
                Session["username"] = s.UserName;
                    Session["AccountNumber"] = credentials.AccountNumber.ToString();
                    TempData["msg"] = "<script>alert('Login Successfully');</script>";
                    return RedirectToAction("Index", "Home");
                }
      
        }
        public ActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgot(UserInfo u)
        {
            generateOTP(u);
            TempData["account"] = u.AccountNumber.ToString();

            return RedirectToAction("otpID", "Login");
        }
        public ActionResult otpID()
        {
            return View();
        }
        [HttpPost]
        public ActionResult otpID(UserInfo u)
        {
            if ((u.OTP).ToString() == Session["otp"].ToString())
            {
                TempData["msg"] = "<script>alert('UserID has been sent to your registered email ID successfully.');</script>";
                using (MailMessage mail = new MailMessage())
                {
                    var credentials = db.UserInfoes.Where(Models => Models.AccountNumber == u.AccountNumber).FirstOrDefault();
                    var credentials1 = db.Registrations.Where(Models => Models.AccountNumber == u.AccountNumber).FirstOrDefault();
                    mail.From = new MailAddress("roughusemail2000@gmail.com");
                    mail.To.Add(credentials1.Email);
                    mail.Subject = "UserId ";
                    mail.Body += "<br /> Greetings from SSB Bank...";
                    mail.Body += "<br /> Your requested credentials are : <br />";
                    mail.Body += "<br /> UserId: " + credentials.UserName;
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
            else
            {
                TempData["msg"] = "<script>alert('Incorrect OTP, Please Try Again!!');</script>";
            }
            ModelState.Clear();
            return View();
        }
        public void generateOTP(UserInfo u)
        {
            var credentials = db.Registrations.Where(Models => Models.AccountNumber == u.AccountNumber).FirstOrDefault();
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
                Session["otp"] = strrandom;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("roughusemail2000@gmail.com");
                    mail.To.Add(credentials.Email);
                    mail.Subject = "OTP to Register for Internet Banking";
                    mail.Body += "<br /> Greetings from SSB Bank...";
                    mail.Body += "<br /> <br />Please enter the below OTP to verify your account.";
                    mail.Body += "<br /> OTP: " + Session["otp"];
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

        public ActionResult Forgotpassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgotpassword(UserInfo u)
        {
            generateOTP(u);
            TempData["account"] = u.AccountNumber.ToString();

            return RedirectToAction("otpPassword", "Login");
            
        }


        public ActionResult otpPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult otpPassword(UserInfo u)
        {
            if ((u.OTP).ToString() == Session["otp"].ToString())
            {
                TempData["msg"] = "<script>alert('Password is generated successfully successfully.');</script>";
                var credentials=db.UserInfoes.FirstOrDefault(x => x.AccountNumber == u.AccountNumber);
                credentials.Password=u.Password;
                credentials.Confirm_Password = u.Confirm_Password;
                db.Entry(credentials).State=System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Login");
            }
                return View();
        }





    }
}