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
            claims.Add(new Claim("http://pegasusmission.io/claims/role", "gateway"));
            return JwtSecurityTokenBuilder.Create(issuer, audience, claims, 2000, signingKey);
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
    }
}
