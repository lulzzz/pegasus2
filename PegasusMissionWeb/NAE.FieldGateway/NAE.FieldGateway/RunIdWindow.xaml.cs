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
    /// Interaction logic for RunIdWindow.xaml
    /// </summary>
    public partial class RunIdWindow : Window
    {
        public RunIdWindow()
        {
            InitializeComponent();
        }

        public string RunIdText;

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.RunIdText = RunId.Text;
            this.Close();
        }
    }
}
