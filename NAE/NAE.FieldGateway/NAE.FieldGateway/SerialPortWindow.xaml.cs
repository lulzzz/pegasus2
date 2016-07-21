using NAE.FieldGateway.ViewModels;
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
using System.Windows.Shapes;

namespace NAE.FieldGateway
{
    /// <summary>
    /// Interaction logic for SerialPortWindow.xaml
    /// </summary>
    public partial class SerialPortWindow : Window
    {
        private SerialPortViewModel viewModel;

        public SerialPortWindow()
        {
            InitializeComponent();
            viewModel = new SerialPortViewModel();
        }

        public bool HasPortName
        {
            get { return !string.IsNullOrEmpty(viewModel.PortName); }
        }

        public string PortName
        {
            get { return viewModel.PortName; }
            set { viewModel.PortName = value; }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.PortName = null;
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string portName = this.Ports.SelectedValue as string;

            if (!string.IsNullOrEmpty(portName))
            {
                this.viewModel.PortName = portName;
            }

            this.Close();
        }
    }
}
