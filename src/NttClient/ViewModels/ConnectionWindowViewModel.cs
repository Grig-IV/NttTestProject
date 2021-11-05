using NttClient.Models;
using NttClient.Views;
using NttLibrary.Utils;
using System.Net;
using System.Windows;

namespace NttClient.ViewModels
{
    public class ConnectionWindowViewModel : Notifier
    {
        private bool _isConnecting;
        private IPAddress _ipAddress;
        private ushort? _port;
        private RelayCommand _createConnectionCommand;

        public ConnectionWindowViewModel()
        {
            _isConnecting = false;
            _ipAddress = null;
            _port = null;
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            private set
            {
                _isConnecting = value;
                NotifyPropertyChanged();
            }
        }

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public ushort? Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public RelayCommand CreateConnectionCommand
        {
            get
            {
                return _createConnectionCommand ??= new RelayCommand(
                    execute: async connectionWindow =>
                    {
                        IsConnecting = true;

                        var client = SingletonProvider.GetSingelton<ClientModel>();
                        var socketEx = await client.TryConnectAsync(IpAddress, (ushort)Port);

                        // Show warning message in case exception.
                        if (socketEx != null)
                        {
                            IsConnecting = false;
                            MessageBox.Show(
                                $"Failed to connect to Server:\n{socketEx.Message}",
                                "Connection error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }

                        var newChatWindow = new ChatWindowView();
                        newChatWindow.Show();
                        (connectionWindow as Window).Close();
                    },
                    canExecute: _ => IpAddress != null && Port != null);
            }
        }
    }
}
