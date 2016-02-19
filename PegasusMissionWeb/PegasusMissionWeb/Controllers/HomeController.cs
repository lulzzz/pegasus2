﻿using PegasusMissionWeb.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PegasusMissionWeb.Controllers
{
    public class HomeController : Controller
    {
        private static string error;
        private static readonly bool _liveVideo;
        private static readonly string _poster;
        private static readonly string _station;
        private static readonly string _payload;

        static HomeController()
        {
            _payload=System.Configuration.ConfigurationManager.AppSettings["payloadUrl"];
            _station=System.Configuration.ConfigurationManager.AppSettings["stationUrl"];
            _poster=System.Configuration.ConfigurationManager.AppSettings["posterUrl"];
            if (string.IsNullOrEmpty(_poster))
            {
                _poster = "/img/Pegasus2Announce.png";
            }
            _liveVideo = !string.IsNullOrEmpty(_payload) && !string.IsNullOrEmpty(_station);
        }


        public ActionResult Index()
        {
            ViewBag.LiveVideo = _liveVideo;
            ViewBag.Poster = _poster;
            ViewBag.PayloadUrl = _payload;
            ViewBag.StationUrl = _station;
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About The Pegasus Mission";

            return View();
        }

        public ActionResult Notifications()
        {
            ViewBag.Message = "Pegasus II Text Message Notifications";
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.PhoneError = error;
                error = null;
            }

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

        [HttpPost]
        public ActionResult AddPhone(string countryCode, string number)
        {
            error = null;
            //^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$
            //"^(\+[1-9][0-9]*(\([0-9]*\)|-[0-9]*-))?[0]?[1-9][0-9\- ]*$"
            Regex exp = new Regex(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$");

            StringBuilder builder = new StringBuilder();
            
            if(!string.IsNullOrEmpty(countryCode))
            {
                if(countryCode.Trim().ToCharArray()[0] != '+')
                {
                    builder.Append("+");
                }

                builder.Append(countryCode.Trim());
            }

            if(!string.IsNullOrEmpty(number))
            {
                builder.Append(number.Trim());
            }

            string phone = builder.ToString();

            if(!exp.IsMatch(phone))
            {
               error = "Phone number is not valid.";
                return RedirectToAction("Notifications");                
            }

            string jsonString = "{\"Number\":\"" + phone + "\"}";
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