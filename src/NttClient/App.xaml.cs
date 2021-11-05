using System.Diagnostics;
using System.Net;
using System.Windows;

namespace NttClient
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConnectionWindowView connectionWindow;

            if (Debugger.IsAttached)
            {
                // Auto fill input fields for fast debugging
                connectionWindow = new ConnectionWindowView(IPAddress.Parse("127.0.0.1"), 8888);
            }
            else
            {
                connectionWindow = new ConnectionWindowView();
            }

            connectionWindow.Show();
        }
    }
}
