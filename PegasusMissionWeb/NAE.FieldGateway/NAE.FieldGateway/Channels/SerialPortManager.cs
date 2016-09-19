
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAE.Data;

namespace NAE.FieldGateway.Channels
{
    public class SerialConnection 
    {

        public delegate void CraftTelemetryEventHandler(object sender, Telemetry message);
        public delegate void SerialErrorEventHandler(object sender, string message, SerialErrorType error);

        public SerialConnection()
        {

        }
        public SerialConnection(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
            this.parity = parity;
        }

        public event CraftTelemetryEventHandler OnTelemetry;
        public event SerialErrorEventHandler OnError;


        private string portName;
        private int baudRate;
        private int dataBits;
        private StopBits stopBits;
        private Parity parity;
        private SerialPort serialPort;
        private bool expectedShutdown;

        public bool IsConnected
        {
            get
            {
                if (this.serialPort != null)
                {
                    return this.serialPort.IsOpen;
                }
                else
                {
                    return false;
                }
            }
        }

        #region Serial Port Open/Close
        public void Open()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                serialPort = new SerialPort();
                string[] ports = SerialPort.GetPortNames();
                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = dataBits;
                serialPort.StopBits = stopBits;
                serialPort.Parity = parity;
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.Disposed += serialPort_Disposed;
                serialPort.ErrorReceived += serialPort_ErrorReceived;
                serialPort.Open();
                serialPort.DtrEnable = true;

            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Serial connection failed.");
                Trace.TraceError(ex.Message);

                if (OnError != null)
                {
                    OnError(this, "Serial connection could not open.", SerialErrorType.Open);
                }

                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                expectedShutdown = true;
                if (this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Serial connection faulted during close.");
                Trace.TraceError(ex.Message);

                if (OnError != null)
                {
                    OnError(this, "Serial connection could not close.", SerialErrorType.Close);
                }

                throw ex;
            }
        }

        #endregion
        

        #region Serial port events
        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Trace.TraceWarning("Serial port error received.");
            Trace.TraceError(e.EventType.ToString());

            if (OnError != null)
            {
                OnError(this, e.EventType.ToString(), SerialErrorType.Telemetry);
            }
        }

        void serialPort_Disposed(object sender, EventArgs e)
        {
            if (!expectedShutdown)
            {
                if (OnError != null)
                {
                    OnError(this, "Unexpected shutdown; serial port was disposed.", SerialErrorType.Unknown);
                }
            }


        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
                string line = null;
                try
                {

                    line = this.serialPort.ReadLine();
                    line = line.Replace("\r", "");
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Fault reading telemetry from serial port.");
                    Trace.TraceError(ex.Message);

                    if (OnError != null)
                    {
                        OnError(this, "Telemetry reading from serial port error.", SerialErrorType.Telemetry);
                    }
                }

            if (!string.IsNullOrEmpty(line))
            {
                try
                {
                    Telemetry telemetry = Telemetry.Load(line);

                    if (OnTelemetry != null)
                    {
                        OnTelemetry(this, telemetry);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error decoding message - {0}", ex.Message);
                }
            }
        }

        #endregion

    }
}
