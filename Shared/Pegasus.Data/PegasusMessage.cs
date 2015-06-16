namespace Pegasus2.Data
{
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public abstract class PegasusMessage
    {
        public abstract MessageType GetMessageType();
        public abstract byte[] ToCraftMessage();
        public abstract PegasusMessage FromCraftMessage(string message);

        public abstract string ToJson();

        public abstract PegasusMessage FromJson(string jsonString);

        public static PegasusMessage Decode(string message)
        {
            PegasusMessage pmessage = null;
            int checkValue = 0;
            byte checkSum = 0;

            if (!message.Contains("*")) //not a valid message; no check value
            {
                return null;
            }

            string prefix = message.Substring(0, 2); //identifies message type
            string messageString = message.Substring(2, message.Length - 6);  //the message without identifier and check value
            string checkValueString = message.Substring(message.Length - 2, 2); //the check value

            string[] parts = messageString.Split(new char[] { ',' }); //the message parts as string array

            //get the check value as an int
            if (!int.TryParse(message.Substring(message.Length - 2, 2), NumberStyles.AllowHexSpecifier, null, out checkValue))
            {
                return null;
            }

            //compute the check sum
            checkSum = (byte)Encoding.UTF8.GetBytes(message.Substring(0, message.Length - 4)).Sum(x => (int)x);

            //check value should equal check value; otherwise invalid message 
            if (checkSum != (byte)checkValue)
            {
                return null;
            }

            //let's return the deserialized message
            /*
                $: Craft Telemetry
                #: Ground Telemetry
                P: Parachute Command
                B: Delivery System Command
                V: Camera Command
                U: User Message
                R: Parachute Deployment
                R: Delivery System Release
                R: User Message
                N: Craft Note

             */
            switch (prefix)
            {
                case "$:": //craft telemetry
                    pmessage = new CraftTelemetry();
                    break;
                case "#:": //ground telemetry
                    pmessage = new GroundTelemetry();
                    break;
                case "P:": //parachute command
                    pmessage = new ParachuteCommand();
                    break;
                case "B:": //delivery system command
                    pmessage = new DeliverySystemCommand();
                    break;
                case "V:": //camera command
                    pmessage = new CameraCommand();
                    break;
                case "U:": //user message
                    pmessage = new UserMessage();
                    break;
                case "R:": //response (parachute deployment | delivery system release | user message)
                    break;
                case "N:": //craft note
                    pmessage = new CraftNote();
                    break;
                default:
                    return null;
            }

            return pmessage.FromCraftMessage(message);
        }


    }

}