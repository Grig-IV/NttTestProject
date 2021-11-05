using NttLibrary.Utils;
using NttServer.Models;
using NttServer.Views;
using System.Net;
using System.Windows;

namespace NttServer.ViewModels
{
    public class ServerSettingsViewModel
    {
        private IPAddress _localAddr;
        private ushort? _port;
        private RelayCommand _runServerCommand;

        public ServerSettingsViewModel()
        {
            _localAddr = IPAddress.Parse("127.0.0.1");
            _port = null;
        }

        public ushort? Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public RelayCommand RunServerCommand
        {
            get => _runServerCommand ??= new RelayCommand(
                execute: settingsWindow =>
                {
                    var server = SingletonProvider.GetSingelton<ServerModel>();
                    var socketEx = server.TryListening(_localAddr, (ushort)Port);

                    // Show error message in case exception
                    if (socketEx != null)
                    {
                        MessageBox.Show(
                            $"Failed to create Server:\n{socketEx.Message}",
                            "Server creating error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    var serverWindow = new ServerView();
                    serverWindow.Show();
                    (settingsWindow as Window).Close();
                },
                canExecute: settingsWindow => Port != null);
        }
    }
}
