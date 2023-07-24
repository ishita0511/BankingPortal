using BankingPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BankingPortal.Controllers
{
    
    public class HomeController : Controller
    {


        // GET: Home
        BankPortalEntities db = new BankPortalEntities();

       
        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            return View();
        }

        public ActionResult AboutUs()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            return View();
        }
        public ActionResult Statement()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            return View();
        }


        [HttpPost]

        public ActionResult Statement(DateTime? start, DateTime? end)
        {
           
            if (start.HasValue && end.HasValue)
            {



                if (start >= Convert.ToDateTime("1/1/0002 00:00:00 AM") && end <= DateTime.Now && start <= end)
                {

                    long l = long.Parse((string)Session["AccountNumber"]);

                    DateTime dt = Convert.ToDateTime(start.Value.ToShortDateString());
                    TimeSpan ts = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    dt = dt.Add(ts);


                    DateTime dt1 = Convert.ToDateTime(end.Value.ToShortDateString());
                    TimeSpan ts1 = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    dt1 = dt1.Add(ts1);

                    var info = db.Funcdate(dt, dt1).Where(x => x.SenderAccount == l).ToList();

                    return View(info);

                }
                else
                {
                    ViewData["wrongdates"] = "Please Enter valid Dates !";
                    return View();
                }
            }
            else
            {
                ViewData["wrongdates"] = "Please mention dates !";
                return View();
            }


        }
        
        public ActionResult Transfer()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            _ = TempData["Extract"];
            return View();
        }
        

        [HttpPost]
        public ActionResult Transfer(long SenderAccountNumber, long ReceiverAccountNumber, float Amount, string Remark, string TransferMode)
        {
            var sender = db.Beneficiaries.Where(x => x.BeneficiaryAcc == ReceiverAccountNumber && x.AccountNumber == SenderAccountNumber).FirstOrDefault();
            var bal = db.balances.Where(x => x.AccountNumber == SenderAccountNumber).FirstOrDefault();
            if (sender == null)
            {
                TempData["Message"] = "<script>alert('Beneficiary not present, add beneficiary');</script>";
                return View();
            }

            else if(Amount == 0 || Amount > bal.AccountBalance)
            {

                TempData["Message"] = "<script>alert('Please Enter Valid Balance');</script>";
                return View();
            }
            else
            {
                TempData["Message"] = "<script>alert('Transfer Successfull');</script>";
                Random rnd = new Random();
                long RId = rnd.Next(100000, 999999);

                db.Functransfer(SenderAccountNumber, ReceiverAccountNumber, Amount, Remark, TransferMode, RId);
                return RedirectToAction("Show","Home");
              
            }

        }



           

            public ActionResult Show()
            {
                BankPortalEntities db = new BankPortalEntities();
                long l = long.Parse((string)Session["AccountNumber"]);
                var v = db.TransactionInfoes.Where(x => x.SenderAccount == l).OrderByDescending(t => t.Id).First();
                return View(v);
            }
        

        public ActionResult Bene()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Bene(Beneficiary b)
        {



            if (ModelState.IsValid)
            {
                db.Beneficiaries.Add(b);
                db.SaveChanges();
                TempData["Mesage"] = "<script>alert('Beneficiary Added Successfully');</script>";

                ModelState.Clear();
                return View();
            }
            else
            {
                TempData["Mesage"] = "<script>alert('Please Enter valid details');</script>";
                return View();
            }
           

        }

        public ActionResult showbeneficiary()
        {
            long l = long.Parse((string)Session["AccountNumber"]);
            var credentials = db.Beneficiaries.Where(x => x.AccountNumber == l).ToList();
            return View(credentials);
        }


        public ActionResult ExtractBeneficiary(int id)
        {
            var credentials1 = db.Beneficiaries.Where(x => x.Id == id).FirstOrDefault();
            TempData["Extract"] = credentials1.BeneficiaryAcc.ToString();
            return RedirectToAction("Transfer","Home");

        }
        public ActionResult Profiles()
        {


            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            long l = long.Parse((string)Session["AccountNumber"]);
            
            var info = db.Registrations.Where(x => x.AccountNumber == l).FirstOrDefault();
            return View(info);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Customers()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
           
            var info = db.Registrations.ToList();
            return View(info);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Approval()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
           
            var info = db.Registrations.Where(x => x.Status == 0).ToList();
            return View(info);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Approve(long id)
        {
          
            var cred = db.Registrations.Where(x => x.AccountNumber == id).FirstOrDefault();
            cred.Status = 1;

            db.Entry(cred).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            SendEmailAccount(cred);

            balance b = new balance();
            b.AccountNumber = cred.AccountNumber;
            if (cred.AccountType == "Savings Account"){b.AccountBalance = 10000;}
            else{  b.AccountBalance = 25000;}
      
            db.balances.Add(b);
            db.SaveChanges();
            return RedirectToAction("Approval", "Home");
        }

   


        public ActionResult Contact()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            ViewBag.TheMessage = "If you are facing any trouble , Please send us a message.";
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string message)
        {
            ViewBag.TheMessage = "Thanks we got your message , we will get back to you";
            return View();
        }


        

        [Authorize(Roles = "admin")]
        public ActionResult Edit(long id)
        {

            var row = db.Registrations.Where(model => model.AccountNumber == id).FirstOrDefault();
            return View(row);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Registration r)
        {
            if (ModelState.IsValid == true)
            {
                db.Entry(r).State = EntityState.Modified;
                int a = db.SaveChanges();
                if (a > 0)
                {
                    TempData["UpdateMessage"] = "<script>alert('Updated !!')</script>";
                    return RedirectToAction("Customers");
                }
                else
                {
                    TempData["UpdateMessage"] = "<script>alert('Not Updated !!')</script>";
                }
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Decline(long id)
        {
            var r = db.Registrations.Where(model => model.AccountNumber == id).FirstOrDefault();
            TempData["AccNum"] = id.ToString();
            
            return View(r);

        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Decline(Registration r)
        {

            long id = long.Parse((string)TempData["AccNum"]);

            var DeletedRow = db.Registrations.Where(model => model.AccountNumber == id).FirstOrDefault();
            SendEmailDecline(DeletedRow, r.Remark);
            db.Registrations.Remove(DeletedRow);
            db.SaveChanges();
            
            var list = db.Registrations.ToList();
            return View("Approval", list);
        }


        [Authorize(Roles = "admin")]
        public ActionResult Delete(long id)
        {
            var DeletedRow = db.Registrations.Where(model => model.AccountNumber == id).FirstOrDefault();
            db.Registrations.Remove(DeletedRow);
            db.SaveChanges();

            var list = db.Registrations.ToList();
            return View("Customers", list);
        }


        [Authorize(Roles = "admin")]
        public ActionResult Details(long id)
        {
            var DetailRow = db.Registrations.Where(model => model.AccountNumber == id).FirstOrDefault();
            return View(DetailRow);
        }

        public ActionResult LogOff()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            TempData["msg"] = "<script>alert('LogOut Successfully !!!');</script>";
            return RedirectToAction("Index", "Login");
        }
        [Authorize(Roles = "admin")]
        public void SendEmailAccount(Registration r)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("roughusemail2000@gmail.com");
                mail.To.Add(r.Email);
                mail.Subject = "Account created successfully";
                mail.Body += "<br /> Greetings from SSB Bank...";
                mail.Body += "<br /> <br />Congratulations your Account Number is generated successfully ";
                mail.Body += "<br /> Name: " + r.FirstName + " " + r.LastName;
                mail.Body += "<br /> Father's Name: " + r.FatherName;
                mail.Body += "<br /> Account Number: " + r.AccountNumber;
                mail.Body += "<br /> Account Type: " + r.AccountType;
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

        [Authorize(Roles = "admin")]
        public void SendEmailDecline(Registration r, string remark)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("roughusemail2000@gmail.com");
                mail.To.Add(r.Email);
                mail.Subject = "Account Request Declined!!!";
                mail.Body += "<br /> Greetings from SSB Bank...";
                mail.Body += "<br /> <br /> Sorry to inform you that your account request has been rejected <br/>";

                mail.Body += "<br /> Reason : <br/>";
                mail.Body += remark;

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


