using NAE.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PegasusMissionWeb.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
        private static string tokenWebApiUriString = "https://authz.pegasusmission.io/api/phone";
        private static string secret = "851o2LqnMUod9lp7DvVxSrH+KQAkydBF9MDREicDus4=";
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

            PegasusHub hub = new PegasusHub();
            //hub.Subscribe();


            return View();
        }

        public ActionResult OnboardTelemetry(string runId)
        {
         
           var runIds = parseConfJson();
            //if (runId != null) { parseConfJson(runId); }
            string v = "";
            foreach (var n in runIds)
            {
                v += "<option onChange='all()'>"+n+"</option>";
            }
            ViewBag.runIds = v;
                return View();           
        }

        private List<string> parseConfJson()
        {
            var configUrl = System.Configuration.ConfigurationManager.AppSettings["configUrl"];

            using (var client = new WebClient())
            {
                List<string> arrOfRunIds = new List<string>();
                var arr = JsonConvert.DeserializeObject<List<Config>>(client.DownloadString(new Uri(configUrl)));
                foreach(var n in arr)
                {
                    arrOfRunIds.Add(n.RunId);
                }
                return arrOfRunIds;
            }
        }

        

        public ActionResult SendMessage()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult SendMessageToBackend(string country, string message)
        {
            UserMessage um = new UserMessage() { Id = Guid.NewGuid().ToString(), Message = message+ "`" + country };
            SendMessage2(um);
            TempData["SendMessageResult"] = "Thank you, your message was sent successfully!";
            return RedirectToAction("SendMessage");
        }

        public ActionResult About()
        {
            ViewBag.Message = "About The Pegasus Mission";

            return View();
        }



        public static void SendMessage2(UserMessage message)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://broker.pegasusmission.io/api/connect?topic=http://pegasus2.org/usermessage");
                //{                
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", String.Format("Bearer {0}", GetSecurityToken()));
                string jsonString = JsonConvert.SerializeObject(message);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.Accepted)
                    {
                        Debug.WriteLine(String.Format("Piraeus returned invalid status code of {0}", response.StatusCode.ToString()));
                    }
                    else
                    {
                        Debug.WriteLine("Message sent to Piraeus");
                    }
                }
                //}
            }
            catch (WebException we)
            {
                Trace.TraceWarning("Failed Web exception call sending to Piraeus.");
                Trace.TraceError(we.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed call to Piraeus.");
                Trace.TraceError(ex.Message);
            } }

        private static string GetSecurityToken()
        {
            //remember this is going to be a JSON string for the security token
            //so be sure to decode to a proper string; 

            string urlEncodedSecret = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(secret));

            string requestUriString = String.Format("{0}?key={1}", tokenWebApiUriString, urlEncodedSecret);

            using (WebClient webClient = new WebClient())
            {
                string jsonString = webClient.DownloadString(requestUriString);
                //deserialize the json string to a get the actual security token string
                return JsonConvert.DeserializeObject<string>(jsonString);
            }
        }

        #region Pegasus NAE testing code
        //private void parseConfJson(string runId)
        //{
        //    var configUrl = System.Configuration.ConfigurationManager.AppSettings["configUrl"];

        //    using (var client = new WebClient())
        //    {
        //        var configFile = JsonConvert.DeserializeObject<List<Config>>(client.DownloadString(new Uri(configUrl)));
        //        var runToShow = configFile.Where(n => n.RunId.Equals(runId)).First().OnboardTelemetryUrl;

        //        var telemetryFile = JsonConvert.DeserializeObject<List<EagleTelemetry>>(client.DownloadString(new Uri(runToShow)));
        //        PegasusHub hub = new PegasusHub();
        //        //foreach (EagleTelemetry t in telemetryFile)
        //        //    {
        //        //        hub.Send(JsonConvert.SerializeObject(t));
        //        //    }                
        //    }
        //}
        #endregion

        #region Pegasus2 legacy code


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

        //public ActionResult ActivateSMS()
        //{
        //    string number = "9689370998";

        //    string jsonString = "{\"Number\":\"" + number + "\"}";
        //    //"{\"Number\":\"5302137893\"}"

        //    //string jsonString = String.Format("{\"Number\":\"{0}\"}", number);
        //    SMSManager.AddPhone(jsonString);
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult AddPhone(string countryCode, string number)
        //{
        //    error = null;
        //    //^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$
        //    //"^(\+[1-9][0-9]*(\([0-9]*\)|-[0-9]*-))?[0]?[1-9][0-9\- ]*$"
        //    Regex exp = new Regex(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$");

        //    StringBuilder builder = new StringBuilder();

        //    if (!string.IsNullOrEmpty(countryCode))
        //    {
        //        if (countryCode.Trim().ToCharArray()[0] != '+')
        //        {
        //            builder.Append("+");
        //        }

        //        builder.Append(countryCode.Trim());
        //    }

        //    if (!string.IsNullOrEmpty(number))
        //    {
        //        builder.Append(number.Trim());
        //    }

        //    string phone = builder.ToString();

        //    if (!exp.IsMatch(phone))
        //    {
        //        error = "Phone number is not valid.";
        //        return RedirectToAction("Notifications");
        //    }

        //    string jsonString = "{\"Number\":\"" + phone + "\"}";
        //    SMSManager.AddPhone(jsonString);
        //    return RedirectToAction("Index");
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //public ActionResult Chat()
        //{
        //    ViewBag.Message = "Your chat page.";

        //    return View();
        //}
        #endregion
    }
}