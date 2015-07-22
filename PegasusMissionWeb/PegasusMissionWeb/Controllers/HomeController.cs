using PegasusMissionWeb.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PegasusMissionWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ActivateSMS()
        {
            string number = "9689370998";

            string jsonString = "{\"Number\":\"" + number + "\"}";
            //"{\"Number\":\"5302137893\"}"

            //string jsonString = String.Format("{\"Number\":\"{0}\"}", number);
            SMSManager.AddPhone(jsonString);
            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            ViewBag.Message = "Your chat page.";

            return View();
        }
    }
}