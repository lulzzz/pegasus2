using Newtonsoft.Json;
using Piraeus.Protocols.Coap;
using Piraeus.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FieldGateway.Telemetry.Generator
{
    public enum ChannelType
    {
        SerialPort,
        WebSocket
    }
    public class TelemetryManager
    {
        public TelemetryManager(SerialConnection conn)
        {
            this.serial = conn;
        }

        public TelemetryManager(string host, string subprotocol, string token)
        {
            this.ws = new WebSocketClient();
            Task task = Task.Factory.StartNew(async () =>
            {
                await this.ws.ConnectAsync(host, subprotocol, token);
            });

            Task.WhenAll(task);
        }

        private SerialConnection serial;
        private WebSocketClient ws;
        private string runId;
        private const string telemetryUriString = "http://pegasusnae.org/telemetry";
        public async Task RunAsync(string runId)
        {
            this.runId = runId;

            if(serial != null)
            {
                await RunViaSerialPort();
            }
            else
            {
                await RunViaWebSocket();
            }
        }

        private async Task RunViaSerialPort()
        {
            this.serial.Open();

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.RT_Telemetry_Test)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        Thread.Sleep(500);
                        await this.serial.SendAsync(line + "\r\n");
                    }
                }
            }
        }

        private async Task RunViaWebSocket()
        {
            if(!this.ws.IsConnected)
            {
                Thread.Sleep(5000);
            }

            int index = 0;
            ushort id = 0;
            double gpsLatStart = 0;
            double gpsLonStart = 0;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.RT_Telemetry_Test2)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        id++;
                        string line = await reader.ReadLineAsync();
                        NAE.Data.Telemetry t = NAE.Data.Telemetry.Load(line);

                        if(gpsLatStart == 0)
                        {
                            gpsLatStart = t.GpsLatitude;
                            gpsLonStart = t.GpsLongitude;
                        }

                        t.GpsLatitudeStart = gpsLatStart;
                        t.GpsLongitudeStart = gpsLonStart;
                        t.RunId = this.runId;

                        string jsonString = JsonConvert.SerializeObject(t);
                        string coapUriString = String.Format("coaps://pegasusmission.io/publish?topic={0}", telemetryUriString);
                        CoapRequest request = new CoapRequest(id, RequestMessageType.NonConfirmable, MethodType.POST, 
                                            new Uri(coapUriString), MediaType.Json, Encoding.UTF8.GetBytes(jsonString));

                        byte[] message = request.Encode();
                        await ws.SendAsync(message);
                        index++;
                        Console.WriteLine("Sending message {0}  sleeping 500ms AirSpeed {1}", index,t.GpsSpeedMph);
                        Thread.Sleep(500);
                        
                    }
                }
            }
        }
    }
}
