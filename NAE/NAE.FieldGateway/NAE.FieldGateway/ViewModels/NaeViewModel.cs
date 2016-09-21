using Microsoft.Maps.MapControl.WPF;
using NAE.Data;
using NAE.FieldGateway.Channels;
using NAE.FieldGateway.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NAE.FieldGateway.ViewModels
{
    public class NaeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private SynchronizationContext uiContext;


        public NaeViewModel()
        {
            uiContext = SynchronizationContext.Current; 

            this.userMessages = new ObservableCollection<string>();
            
            this.host = ConfigurationManager.AppSettings["websocketHost"];
            this.subprotocol = ConfigurationManager.AppSettings["subprotocol"];
        }

        public void TelemetryUpdate(Telemetry telemetry)
        {
            if(telemetry == null)
            {
                throw new ArgumentNullException("telemetry");
            }

            this.GpsLatitudeStart = telemetry.GpsLatitudeStart;
            this.GpsLongitudeStart = telemetry.GpsLongitudeStart;
            this.GpsLatitude = telemetry.GpsLatitude;
            this.GpsLongitude = telemetry.GpsLongitude;
            this.GpsAltitude = telemetry.GpsAltitude;
            this.GpsDirection = telemetry.GpsDirection;
            this.GpsSpeedKph = telemetry.GpsSpeedKph;
            this.GpsSpeedMph = telemetry.GpsSpeedMph;
            this.Humidity = telemetry.Humidity;
            this.LinearAccelX = telemetry.LinearAccelX;
            this.LinearAccelY = telemetry.LinearAccelY;
            this.LinearAccelZ = telemetry.LinearAccelZ;
            this.Pitch = telemetry.Pitch;
            this.Roll = telemetry.Roll;
            this.Yaw = telemetry.Yaw;
            this.Pressure = telemetry.Pressure;
            this.Timestamp = telemetry.Timestamp;
            this.Altitude = telemetry.Altitude;
            this.Current = telemetry.Current;
            this.Fix = telemetry.SatelliteFix;
            this.Satellites = telemetry.Satellites;
            this.Sound = telemetry.Sound;
            this.Temperature = telemetry.Temperature;
            this.Voltage = telemetry.Voltage;
        }


        private string host;
        private string subprotocol;
       

        private WebSocketManager wsManager;
        private SerialConnection spManager;
        private string status;


        private double gpsLatitude;
        private double gpsLongitude;
        private double gpsLatitudeStart;
        private double gpsLongitudeStart;
        private double gpsAltitude;
        private double gpsDirection;
        private double gpsSpeedKph;
        private double gpsSpeedMph;
        private double humidity;
        private double linearAccelX;
        private double linearAccelY;
        private double linearAccelZ;
        private double pitch;
        private double roll;
        private double yaw;
        private double pressure;
        private double altitude;
        private int current;
        private bool fix;
        private int satellites;
        private double sound;
        private double temperature;
        private double voltage;
        private DateTime timestamp;
        private bool startPoint;
        private string runId;
        private Location vehicleLocation;
        private Location startPointLocation;

        private UdpServer udp;

        private ObservableCollection<string> userMessages;
        
        #region Set GPS Start Point

        public void SetGpsStartPoint()
        {
            startPoint = true;
        }

        #endregion

        #region NAE Locations

        public Location VehicleLocation
        {
            get { return vehicleLocation; }
            set
            {
                if (vehicleLocation.Latitude != value.Latitude || vehicleLocation.Longitude != value.Longitude)
                {
                    vehicleLocation = value;
                    RaisePropertyChanged("VehicleLocation");
                }
            }
        }

        public Location StartPointLocation
        {
            get { return startPointLocation; }
            set
            {
                if (startPointLocation.Latitude != value.Latitude || startPointLocation.Longitude != value.Longitude)
                {
                    startPointLocation = value;
                    RaisePropertyChanged("StartPointLocation");
                }
            }
        }

        #endregion

        #region UDP
        public void OpenUdpServer(int port)
        {
            if (udp == null)
            {
                udp = new UdpServer(port);
                udp.OnReceive += Udp_OnReceive;
                Task task = udp.RunAsync();
                Task.WhenAll(task);
            }
        }


        public void SendUpdReset()
        {            
            string message = Reset.GetMessage();
            udp.Send(Encoding.ASCII.GetBytes(message));
        }

        private void Udp_OnReceive(object sender, string message)
        {
            NAE.Data.Telemetry telemetry = null;
            try
            {
                //read the UPD message as a CSV String
                telemetry = NAE.Data.Telemetry.Load(message);
                TelemetryUpdate(telemetry);
            }
            catch (Exception ex)
            {

            }

            if (telemetry != null)
            {
                try
                {
                    //forward to Web socket
                    this.wsManager.SendMessage(telemetry);
                }
                catch (Exception ex)
                {

                }
            }

        }

        public void SendReset()
        {
            udp.Send(Encoding.ASCII.GetBytes(Reset.GetMessage()));
        }



        #endregion

        public ObservableCollection<string> UserMessages
        {
            get { return this.userMessages; }
            set
            {
                userMessages = value;
                RaisePropertyChanged("UserMessages");
            }
        }

        

        public string RunId
        {
            get { return runId; }
            set
            {
                if (runId != value)
                {
                    runId = value;
                    RaisePropertyChanged("RunId");
                }
            }
        }

        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                if (timestamp != value)
                {
                    timestamp = value;
                    RaisePropertyChanged("Timestamp");
                }
            }
        }
        
        #region health
        public int Current
        {
            get { return current; }
            set
            {
                if (current != value)
                {
                    current = value;
                    RaisePropertyChanged("Current");
                }

            }
        }

        public double Voltage
        {
            get { return voltage; }
            set
            {
                if (voltage != value)
                {
                    voltage = value;
                    RaisePropertyChanged("Voltage");
                }

            }
        }

        public bool Fix
        {
            get { return fix; }
            set
            {
                if (fix != value)
                {
                    fix = value;
                    RaisePropertyChanged("Fix");
                }

            }
        }

        public int Satellites
        {
            get { return satellites; }
            set
            {
                if (satellites != value)
                {
                    satellites = value;
                    RaisePropertyChanged("Satellites");
                }
            }
        }

        #endregion
        
        #region cockpit

        public double Sound
        {
            get { return sound; }
            set
            {
                if (sound != value)
                {
                    sound = value;
                    RaisePropertyChanged("Sound");
                }
            }
        }

        public double Temperature
        {
            get { return temperature; }
            set
            {
                if (temperature != value)
                {
                    temperature = value;
                    RaisePropertyChanged("Temperature");
                }
            }
        }

        public double Altitude
        {
            get { return altitude; }
            set
            {
                if (altitude != value)
                {
                    altitude = value;
                    RaisePropertyChanged("Altitude");
                }
            }
        }

        public double Humidity
        {
            get { return humidity; }
            set
            {
                if (humidity != value)
                {
                    humidity = value;
                    RaisePropertyChanged("Humidity");
                }
            }
        }

        public double Pressure
        {
            get { return pressure; }
            set
            {
                if (pressure != value)
                {
                    pressure = value;
                    RaisePropertyChanged("Pressure");
                }
            }
        }

        #endregion

        #region location

        public double GpsLatitudeStart
        {
            get { return gpsLatitudeStart; }
            set
            {
                if (gpsLatitudeStart != value)
                {
                    gpsLatitudeStart = value;
                    RaisePropertyChanged("GpsLatitudeStart");
                }
            }
        }
        public double GpsLongitudeStart
        {
            get { return gpsLongitudeStart; }
            set
            {
                if (gpsLongitudeStart != value)
                {
                    gpsLongitudeStart = value;
                    RaisePropertyChanged("GpsLongitudeStart");
                }
            }
        }
        public double GpsLatitude
        {
            get { return gpsLatitude; }
            set
            {
                if (gpsLatitude != value)
                {
                    gpsLatitude = value;
                    RaisePropertyChanged("GpsLatitude");
                }
            }
        }

        public double GpsLongitude
        {
            get { return gpsLongitude; }
            set
            {
                if(gpsLongitude != value)
                {
                    gpsLongitude = value;
                    RaisePropertyChanged("GpsLongitude");
                }
            }
        }

        public double GpsAltitude
        {
            get { return gpsAltitude; }
            set
            {
                if (gpsAltitude != value)
                {
                    gpsAltitude = value;
                    RaisePropertyChanged("GpsAltitude");
                }
            }
        }

        public double GpsDirection
        {
            get { return gpsDirection; }
            set
            {
                if (gpsDirection != value)
                {
                    gpsDirection = value;
                    RaisePropertyChanged("GpsDirection");
                }
            }
        }


        public double GpsSpeedKph
        {
            get { return gpsSpeedKph; }
            set
            {
                if (gpsSpeedKph != value)
                {
                    gpsSpeedKph = value;
                    RaisePropertyChanged("GpsSpeedKph");
                }
            }
        }

        public double GpsSpeedMph
        {
            get { return gpsSpeedMph; }
            set
            {
                if (gpsSpeedMph != value)
                {
                    gpsSpeedMph = value;
                    RaisePropertyChanged("GpsSpeedMph");
                }
            }
        }

        #endregion

        #region vehicle

        public double LinearAccelX
        {
            get { return linearAccelX; }
            set
            {
                if (linearAccelX != value)
                {
                    linearAccelX = value;
                    RaisePropertyChanged("LinearAccelX");
                }
            }
        }

        public double LinearAccelY
        {
            get { return linearAccelY; }
            set
            {
                if (linearAccelY != value)
                {
                    linearAccelY = value;
                    RaisePropertyChanged("LinearAccelY");
                }
            }
        }

        public double LinearAccelZ
        {
            get { return linearAccelZ; }
            set
            {
                if (linearAccelZ != value)
                {
                    linearAccelZ = value;
                    RaisePropertyChanged("LinearAccelZ");
                }
            }
        }


        public double Pitch
        {
            get { return pitch; }
            set
            {
                if (pitch != value)
                {
                    pitch = value;
                    RaisePropertyChanged("Pitch");
                }
            }
        }

        public double Roll
        {
            get { return roll; }
            set
            {
                if (roll != value)
                {
                    roll = value;
                    RaisePropertyChanged("Roll");
                }
            }
        }

        public double Yaw
        {
            get { return yaw; }
            set
            {
                if (yaw != value)
                {
                    yaw = value;
                    RaisePropertyChanged("Yaw");
                }
            }
        }

        #endregion

        #region Serial Port
        public bool IsSerialConnected
        {
            get { return (spManager != null); }
        }

        public void OpenSerialConnection(string portName)
        {
            int baudRate = Convert.ToInt32(ConfigurationManager.AppSettings["baudRate"]);
            int dataBits = Convert.ToInt32(ConfigurationManager.AppSettings["dataBits"]);
            StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings["stopBits"], true);
            Parity parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings["parity"], true);

            OpenSerialConnection(portName, baudRate, dataBits, stopBits, parity);
        }

        public void OpenSerialConnection(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
        {
            spManager = new SerialConnection(portName, baudRate, dataBits, stopBits, parity);
            spManager.OnTelemetry += SerialPort_OnTelemetry;
            spManager.OnError += SerialPort_OnError;
            spManager.Open();
        }

        private void SerialPort_OnTelemetry(object sender, Telemetry message)
        {
            if(startPoint)
            {
                this.GpsLatitudeStart = message.GpsLatitude;
                this.GpsLongitudeStart = message.GpsLongitude;
                startPoint = false;
            }

            message.GpsLatitudeStart = this.GpsLatitudeStart;
            message.GpsLongitudeStart = this.GpsLongitudeStart;

            TelemetryUpdate(message);

            try
            {
                wsManager.SendMessage(message);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Web Socket failed to send.");
                Trace.TraceError(ex.Message);
            }
        }

        void SerialPort_OnError(object sender, string message, SerialErrorType error)
        {
            Trace.TraceWarning("Serial Port error.");
            Trace.TraceError(String.Format("{0} - {1}", error.ToString(), message));
        }

        #endregion


        public void ShutdownDevice()
        {
            if (udp != null)
            {
                udp.Send(Encoding.UTF8.GetBytes("{X:!,*2E"));
            }
            else
            {
                throw new InvalidOperationException("UDP is not operational.");
            }
        }

        #region Web Socket


        private string GetSecurityToken()
        {
            string issuer = ConfigurationManager.AppSettings["issuer"];
            string audience = ConfigurationManager.AppSettings["audience"];
            string signingKey = ConfigurationManager.AppSettings["signingKey"];

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("http://pegasusmission.io/claims/name", Guid.NewGuid().ToString()));
            claims.Add(new Claim("http://pegasusmission.io/claims/role", "gateway"));
            return JwtSecurityTokenBuilder.Create(issuer, audience, claims, 2000, signingKey);
        }
        public string WebSocketStatus
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    RaisePropertyChanged("WebSocketStatus");
                }
            }
        }

        public void OpenWebSocket()
        {
            if (wsManager != null)
            {
                wsManager.Close();
                wsManager = null;
            }


            string token = GetSecurityToken();
            string[] subscriptions = new string[1];
            subscriptions[0] = "http://pegasus2.org/usernote";
            
            wsManager = new WebSocketManager(this.host, this.subprotocol, token, true, subscriptions);

            wsManager.OnStatusChange += wsManager_OnStatusChange;
            wsManager.OnUserNote += WsManager_OnUserNote;          
            wsManager.Connect();           
        }

        private void WsManager_OnUserNote(object sender, UserMessage message)
        {
            byte[] byteArray = message.ToCraftMessage();

            if (udp != null)
            {
                udp.Send(byteArray);
            }

            uiContext.Send(x => this.userMessages.Add(message.Message), null);
            //this.userMessages.Add(message.Message);

            this.UserMessages = this.userMessages;
        }

        void wsManager_OnStatusChange(object sender, string message)
        {
            WebSocketStatus = message;
        }

        #endregion


        public void CloseConnections()
        {
            if (wsManager != null && wsManager.IsConnected)
            {
                wsManager.Close();
            }

            if (spManager != null && spManager.IsConnected)
            {
                spManager.Close();
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    
    }
}
