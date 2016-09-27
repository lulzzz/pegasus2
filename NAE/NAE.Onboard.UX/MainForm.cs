using NAE.Data;
using NAE.Onboard.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAE.Onboard.UX
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //file the TDMS file
            tdmsFileDialog.Filter = "*.tdms";
            string filename = tdmsFileDialog.FileName;

            TelemetryFileReader reader = TelemetryFileReader.Load(filename, 500);

            List<double> dcoffsets = reader.GetDCOffsets(20);

            List<EagleRawTelemetry> list = reader.Read();
            List<EagleTelemetry> tlist = new List<EagleTelemetry>();

            foreach(EagleRawTelemetry item in list)
            {
                EagleTelemetry telem = TelemetryConverter.Convert(item, dcoffsets);
                tlist.Add(telem);
            }

            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;
            double maxSpeed = 0;

            foreach(EagleTelemetry item in tlist)
            {
                maxX = item.AccelXG > maxX ? item.AccelXG : maxX;
                maxY = item.AccelXG > maxY ? item.AccelYG : maxY;
                maxZ = item.AccelXG > maxZ ? item.AccelZG : maxZ;
                maxSpeed = item.AccelXG > maxX ? item.AirSpeedKph : maxSpeed;
            }


            Config config = new Config();
            config.Aggregates = new AggregateValues();
            config.Aggregates.MaxAccelX = maxX;
            config.Aggregates.MaxAccelY = maxY;
            config.Aggregates.MaxAccelZ = maxZ;
            config.Aggregates.MaxSpeed = maxSpeed;

            //dump the eagle telemetry and aggregates to a file

            







        }
    }
}
