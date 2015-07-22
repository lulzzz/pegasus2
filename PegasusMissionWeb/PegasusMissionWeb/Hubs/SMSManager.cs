using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace PegasusMissionWeb.Hubs
{
   public class SMSManager
    {
        
            //private static string issuer = "urn:pegasusmission.io";
            //private static string audience = "https://tools.pegasusmission.io/api4/smsmanger";
            //private static string url = "http://localhost:49374/api4/smsmanager";
            private static string url = "https://tools.pegasusmission.io/api4/smsmanager";
            //private static string signingKey = "cW0iA3P/mhFi0/O4EAja7UuJ16q6Aeg4cOzL7SIvLL8=";
            
                private static string securityTokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiJjZGIzZDE4ZS1jZDczLTRiNGItOTM5ZS0xZjJjODIzMTViZGQiLCJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL3JvbGUiOiJ3ZWIiLCJpc3MiOiJ1cm46cGVnYXN1c21pc3Npb24uaW8iLCJhdWQiOiJodHRwczovL3Rvb2xzLnBlZ2FzdXNtaXNzaW9uLmlvL2FwaTQvc21zbWFuZ2VyIiwiZXhwIjoxNDUyMDkzMjk0LCJuYmYiOjE0MzY1NDEyOTR9.Y6gVu-WMIcJYquJy94yM9AqAlBGzuANOE6YU2J7Dd2c";
  

         public static void AddPhone(string jsonString)
            {
                //Phone phone = new Phone() { Number = number };
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("Authorization", String.Format("Bearer {0}", securityTokenString));

                    //string jsonString = JsonConvert.SerializeObject(phone);
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                    request.ContentLength = bytes.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response.StatusCode != HttpStatusCode.Accepted)
                        {
                            Trace.TraceWarning(String.Format("Web API returned invalid status code of {0}", response.StatusCode.ToString()));
                        }
                        else
                        {
                            Trace.TraceWarning("Message sent to Web API");
                        }
                    }
                    //}
                }
                catch (WebException we)
                {
                    Trace.TraceWarning("Failed Web exception call sending to Web API.");
                    Trace.TraceError(we.Message);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Failed call to Web API.");
                    Trace.TraceError(ex.Message);
                }
            }



            //public static string GetTokenString()
            //{
            //    string id = Guid.NewGuid().ToString();
            //    List<Claim> claims = new List<Claim>();
            //    claims.Add(new Claim("http://pegasusmission.io/claims/name", id));
            //    claims.Add(new Claim("http://pegasusmission.io/claims/role", "web"));
            //    return JwtSecurityTokenBuilder.Create(issuer, audience, claims, 60 * 24 * 180, signingKey);
            //}
        }
    
}
