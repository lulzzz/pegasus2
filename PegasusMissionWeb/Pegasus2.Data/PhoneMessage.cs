using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus2.Data
{
    [Serializable]
    [JsonObject]
    public class PhoneMessage
    {
        public PhoneMessage()
        {
        }
        
        public string Message { get; set; }
    }
}
