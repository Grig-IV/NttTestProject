using NttClient.ViewModels;
using System.Net;
using System.Windows;

namespace NttClient
{
    public partial class ConnectionWindowView : Window
    {
        public ConnectionWindowView()
        {
            InitializeComponent();
            DataContext = new ConnectionWindowViewModel();
        }

        public ConnectionWindowView(IPAddress ipAddress, ushort port) : this()
        {
            var vm = DataContext as ConnectionWindowViewModel;
            vm.IpAddress = ipAddress;
            vm.Port = port;
        }
    }
}
