using NttServer.ViewModels;
using System.Windows;

namespace NttServer
{
    public partial class ServerSettingsView : Window
    {
        public ServerSettingsView()
        {
            InitializeComponent();
            DataContext = new ServerSettingsViewModel();
        }

        public ServerSettingsView(ushort port) : this()
        {
            (DataContext as ServerSettingsViewModel).Port = port;
        }
    }
}
