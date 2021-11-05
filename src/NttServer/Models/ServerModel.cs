using NttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NttServer.Models
{
    /// <summary>
    /// Represent server logic.
    /// 
    /// Allows listening for pending connection and create chat for each connected client.
    /// </summary>
    public class ServerModel : IDisposable
    {
        private readonly ObservableCollection<ChatModel> _chats;
        private TcpListener _listener;
        private Task _acceptingClientTask;
        private bool _disposed;

        public ServerModel()
        {
            _listener = null;
            _chats = new ObservableCollection<ChatModel>();
            _acceptingClientTask = null;
            _disposed = false;
        }

        ~ServerModel()
        {
            Dispose(false);
        }

        public event NotifyCollectionChangedEventHandler ChatCollectionChanged
        {
            add { _chats.CollectionChanged += value; }
            remove { _chats.CollectionChanged -= value; }
        }

        public IEnumerable<ChatModel> Chats => _chats;

        public IPEndPoint LocalEndpoint => (IPEndPoint)_listener?.LocalEndpoint;

        /// <summary>
        /// Try to starts/resume listening for incoming connection requests.
        /// </summary>
        /// <returns>
        /// If listening started successfully return null, otherwise captured SocketException.
        /// </returns>
        public SocketException TryListening(IPAddress localAddress, ushort port)
        {
            // In case new end point same as previous one no need to create new TcpListner.
            var isEndPointNew = LocalEndpoint == null ||
                !LocalEndpoint.Address.Equals(localAddress) ||
                LocalEndpoint.Port != port;
            if (isEndPointNew)
            {
                _listener = new TcpListener(localAddress, port);
            }

            try
            {
                _listener.Start();
            }
            catch (SocketException socketEx)
            {
                return socketEx;
            }

            // Multiple call protection.
            if (_acceptingClientTask?.Status != TaskStatus.Running)
            {
                _acceptingClientTask = StartAcceptingClientsAsync();
            }

            return null;
        }

        public void StopListening()
        {
            _listener.Stop();
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
                    foreach (ChatModel chat in Chats)
                    {
                        chat.Dispose();
                    }
                    _listener?.Stop();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Accepts a pending connection request while listener is bound or exception is thrown.
        /// </summary>
        /// <returns></returns>
        private async Task StartAcceptingClientsAsync()
        {
            while (_listener.Server.IsBound)
            {
                var newClient = await _listener.AcceptTcpClientAsync();
                var newChat = new ChatModel(newClient);
                _chats.Add(newChat);
            }
        }
    }
}
