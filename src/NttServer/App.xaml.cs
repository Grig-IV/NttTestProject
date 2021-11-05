using System.Diagnostics;
using System.Windows;

namespace NttServer
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServerSettingsView serverSettings;

            if (Debugger.IsAttached)
            {
                // Auto fill input field for fast debugging
                serverSettings = new ServerSettingsView(8888);
            }
            else
            {
                serverSettings = new ServerSettingsView();
            }

            serverSettings.Show();
        }
    }
}
