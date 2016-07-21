using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldGateway.Telemetry.Generator
{
    public class SerialConnection
    {

        //public delegate void CraftTelemetryEventHandler(object sender, CraftTelemetry message);
        //public delegate void GroundTelemetryEventHandler(object sender, GroundTelemetry message);
        //public delegate void CraftNoteEventHandler(object sender, CraftNote message);
        //public delegate void SerialErrorEventHandler(object sender, string message, SerialErrorType error);

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

        //public event CraftTelemetryEventHandler OnCraftTelemetry;
        //public event GroundTelemetryEventHandler OnGroundTelemetry;
        //public event CraftNoteEventHandler OnCraftNote;
        //public event SerialErrorEventHandler OnError;


        private string portName;
        private int baudRate;
        private int dataBits;
        private StopBits stopBits;
        private Parity parity;
        private bool expectedShutdown;
        private SerialPort serialPort;

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
                serialPort.Disposed += serialPort_Disposed;
                serialPort.Open();
                serialPort.DtrEnable = true;

            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Serial connection failed.");
                Trace.TraceError(ex.Message);

                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                if (this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Serial connection faulted during close.");
                Trace.TraceError(ex.Message);


                throw ex;
            }
        }

        #endregion




        #region Serial port events
        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Trace.TraceWarning("Serial port error received.");
            Trace.TraceError(e.EventType.ToString());

        }

        void serialPort_Disposed(object sender, EventArgs e)
        {
        }

        public async Task SendAsync(string message)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                this.serialPort.Write(message);
            });


            await Task.WhenAll(task);

        }
        #endregion

    }
}
