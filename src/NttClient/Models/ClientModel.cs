using NttLibrary.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NttClient.Models
{
    /// <summary>
    /// Represent client logic.
    /// 
    /// Allows connecting to a server and create one active chat.
    /// </summary>
    public class ClientModel : IDisposable
    {
        private ChatModel _activeChat;
        private bool _disposed;

        public ClientModel()
        {
            _activeChat = null;
            _disposed = false;
        }

        ~ClientModel()
        {
            Dispose(false);
        }

        public ChatModel ActiveChat => _activeChat;

        /// <summary>
        /// Try to connects the client to a remote TCP host using the specified IP address and port
        /// number as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// If client connected successfully return null, otherwise captured SocketException.
        /// </returns>
        public async ValueTask<SocketException> TryConnectAsync(IPAddress serverAddress, ushort serverPort)
        {
            var tcpClient = new TcpClient();

            try
            {
                await tcpClient.ConnectAsync(serverAddress, serverPort);
            }
            catch (SocketException socketEx)
            {
                return socketEx;
            }

            _activeChat?.Dispose();
            _activeChat = new ChatModel(tcpClient);

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ActiveChat?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
