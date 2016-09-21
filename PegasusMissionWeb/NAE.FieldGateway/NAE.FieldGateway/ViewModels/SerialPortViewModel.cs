using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.FieldGateway.ViewModels
{
    public class SerialPortViewModel : INotifyPropertyChanged
    {
        public SerialPortViewModel()
        {
            this.portNames = new ObservableCollection<string>(SerialPort.GetPortNames());
        }

        private ObservableCollection<string> portNames;

        public string PortName { get; set; }

        public ObservableCollection<string> PortNames
        {
            get { return this.portNames; }
            set
            {
                portNames = value;
                RaisePropertyChanged("PortNames");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
