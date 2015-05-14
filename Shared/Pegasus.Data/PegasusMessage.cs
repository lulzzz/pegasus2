using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus2.Data
{
    public abstract class PegasusMessage
    {
        public abstract MessageType GetMessageType();
        public abstract byte[] ToCraftMessage();        
        public abstract PegasusMessage FromCraftMessage(string message);

        public abstract string ToJson();

        public static PegasusMessage Decode(string message)
        {
            PegasusMessage pmessage = null;
            int checkValue = 0;
            byte checkSum = 0;

            if(!message.Contains("*")) //not a valid message
            {
                return null;
            }

            string identifier = message.Substring(0, 2);
            string messageString = message.Substring(2, message.Length - 6);
            string checkValueString = message.Substring(message.Length - 2, 2);

            string[] parts = messageString.Split(new char[] { ',' });

            if (!int.TryParse(message.Substring(message.Length - 2, 2), NumberStyles.AllowHexSpecifier, null, out checkValue))
            {
                return null;
            }

            checkSum = (byte)Encoding.UTF8.GetBytes(message.Substring(0, message.Length-4)).Sum(x => (int)x);
            // checkSum = (byte)Encoding.ASCII.GetBytes(message.Substring(0, message.Length-4)).Sum(x => (int)x);
            
            //get the last byte of the checksum 
            if(checkSum != (byte)checkValue)
            {
                return null;
            }

            //check sum is good...let's get the message
            //"$:", "G:", "P:", "B:", "C:", "D:", "R:", "U:","N:"
            switch (identifier)
            {
                case "$:":
                    pmessage = new CraftTelemetry();
                    break;
                case "G:":
                    pmessage = new GroundTelemetry();
                    break;
                default:
                    return null;
            }

            return pmessage.FromCraftMessage(message);
        }

       
    }
}
