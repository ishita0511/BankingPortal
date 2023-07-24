using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BankingPortal;
using BankingPortal.Controllers;

namespace BankingPortalTest
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public void Contact()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Contact() as ViewResult;
            Assert.AreEqual("If you are facing any trouble , Please send us a message.", result.ViewBag.TheMessage);   
        }
    }
}
