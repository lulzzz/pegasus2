using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using Microsoft.Maps.MapControl.WPF;
using NAE.FieldGateway.ViewModels;
using NAE.FieldGateway.Security;
using System.Security.Claims;
using System.Net.NetworkInformation;

namespace NAE.FieldGateway
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            map.ZoomLevel = Convert.ToDouble(ConfigurationManager.AppSettings["zoomLevel"]);
            map.Center = new Location(Convert.ToDouble(ConfigurationManager.AppSettings["mapCenterLat"]), Convert.ToDouble(ConfigurationManager.AppSettings["mapCenterLon"]));
            viewModel = new NaeViewModel();

            //viewModel.WebSocketHost = "ws://broker.pegasusmission.io/api/connect";
            //viewModel.WebSocketHost = ConfigurationManager.AppSettings["websocketHost"];
            //viewModel.WebSocketSubProtocol = ConfigurationManager.AppSettings["subprotocol"];
            //viewModel.WebSocketCredentials = GetSecurityToken();
            //viewModel.Source = ConfigurationManager.AppSettings["gatewaySource"];
            base.DataContext = viewModel;
        }

        private NaeViewModel viewModel;

        private static string GetSecurityToken()
        {
            string issuer = ConfigurationManager.AppSettings["issuer"];
            string audience = ConfigurationManager.AppSettings["audience"];
            string signingKey = ConfigurationManager.AppSettings["signingKey"];

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("http://pegasusmission.io/claims/name", Guid.NewGuid().ToString()));
            //claims.Add(new Claim("http://pegasusmission.io/claims/role", "gateway"));
            claims.Add(new Claim("http://pegasusmission.io/claims/role", "gateway"));

            return JwtSecurityTokenBuilder.Create(issuer, audience, claims, 60 * 24 * 365, signingKey);
            //return JwtSecurityTokenBuilder.Create(issuer, audience, claims, 2000, signingKey);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            viewModel.CloseConnections();
            base.OnClosing(e);
        }

        private void ResetSocket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.OpenWebSocket();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed Open Web Socket", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void SerialPort_Click(object sender, RoutedEventArgs e)
        {

            SerialPortWindow spw = new SerialPortWindow();
            spw.ShowDialog();
            if (spw.HasPortName)
            {
                try
                {
                    viewModel.OpenSerialConnection(spw.PortName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed Open Serial Port", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void StartPoint_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SetGpsStartPoint();
        }

        private void SetRunId_Click(object sender, RoutedEventArgs e)
        {
            RunIdWindow rw = new RunIdWindow();
            rw.ShowDialog();

            if(!string.IsNullOrEmpty(rw.RunIdText))
            {
                viewModel.RunId = rw.RunIdText;
            }           

        }

        private void NetworkInfo_Click(object sender, RoutedEventArgs e)
        {
            var addrs = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                         where nic.OperationalStatus == OperationalStatus.Up
                         where nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
                         where nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                         select nic);

            StringBuilder builder = new StringBuilder();



            foreach (NetworkInterface nic in addrs)
            {

                builder.Append("----------" + Environment.NewLine);
                builder.Append(String.Format("Name {0}", nic.Name) + Environment.NewLine);
                builder.Append(String.Format("Type {0}", nic.NetworkInterfaceType) + Environment.NewLine);
                builder.Append(String.Format("Status {0}", nic.OperationalStatus) + Environment.NewLine);
                builder.Append(String.Format("Speed {0}K", nic.Speed / 8000) + Environment.NewLine);
                builder.Append(String.Format("Address {0}", nic.GetPhysicalAddress().ToString()) + Environment.NewLine);

                foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        builder.Append(String.Format("IP {0}", ip.Address.ToString()) + Environment.NewLine);
                    }
                }


                //builder.Append(String.Format("IP {0}", nic.GetIPProperties().UnicastAddresses[0].Address.Address.ToString()) + Environment.NewLine);

                builder.Append("----------" + Environment.NewLine);

            }

            MessageBox.Show(builder.ToString());
        }

        private void StartUdp_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenUdpServer(Convert.ToInt32(ConfigurationManager.AppSettings["port"]));

        }

        private void SendReset_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SendUpdReset();
        }
    }
}
