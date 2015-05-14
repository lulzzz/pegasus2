

namespace Piraeus.ServiceModel.Protocols.Coap
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    //using System.Web;

    internal static class UriExtensions
    {
        public static IEnumerable<CoapOption> DecomposeCoapUri(this Uri resource)
        {            
            List<CoapOption> options = new List<CoapOption>();

            if(Uri.CheckSchemeName(resource.Scheme))
            {

            }

            
            
            //GenericUriParser gup = new GenericUriParser(GenericUriParserOptions.NoFragment);

            
            //if (!UriParser.IsKnownScheme("coap"))
            //{
            //    UriParser.Register(gup, "coap", -1);
            //}

            //if (!resource.IsWellFormedOriginalString())
            //{
            //    //throw new UriFormatException("Not well formed URI.");
            //}

            //if (!resource.IsAbsoluteUri)
            //{
            //    //throw new UriFormatException("Not absolute URI.");
            //}

            //if (resource.Scheme != "coap" && resource.Scheme != "coaps")
            //{
            //    //throw new UriFormatException(String.Format("Invalid scheme '{0}'", resource.Scheme));
            //}

            options.Add(new CoapOption(OptionType.UriHost, resource.Host));

            if(resource.Port > 0)
            {
                options.Add(new CoapOption(OptionType.UriPort, (uint)resource.Port));
            }

            if (resource.AbsolutePath != "/")
            {
                string[] parts = resource.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string part in parts)
                {
                    options.Add(new CoapOption(OptionType.UriPath, part));
                }
            }

            if (!string.IsNullOrEmpty(resource.Query))
            {
                //NameValueCollection nvc = HttpUtility.ParseQueryString(resource.Query);
               
                //WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(resource.Query);

                string[] querySegments = resource.Query.Split('&');
                foreach (string segment in querySegments)
                {
                    string[] parts = segment.Split('=');
                    if (parts.Length > 0)
                    {
                        string key = parts[0].Trim(new char[] { '?', ' ' });
                        string val = parts[1].Trim();

                        options.Add(new CoapOption(OptionType.UriQuery, key + "=" + val));
                    }
                }

                //int index = 0;
                //while(index < nvc.Count)
                //{
                //    options.Add(new CoapOption(OptionType.UriQuery, nvc.GetKey(index) + "=" + nvc[index]));
                //    index++;
                //}
            }

            return options.ToArray();


        }
    }
}
