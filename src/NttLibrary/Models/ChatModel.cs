using NttLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NttLibrary.Models
{
    /// <summary>
    /// Represen chat logic.
    /// 
    /// Connect to another ChatModel object and allows to send and recive messages
    /// </summary>
    public class ChatModel : Notifier, IDisposable
    {
        private readonly ObservableCollection<ChatMessage> _messages;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private bool _connected;
        private bool _disposed;

        private ChatModel()
        {
            _messages = new ObservableCollection<ChatMessage>();
            _disposed = false;
        }

        public ChatModel(TcpClient tcpClient) : this()
        {
            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            Connected = tcpClient.Connected;

            ActivateListeningAsync();
        }

        ~ChatModel()
        {
            Dispose(false);
        }

        public event NotifyCollectionChangedEventHandler MessageCollectionChanged
        {
            add { _messages.CollectionChanged += value; }
            remove { _messages.CollectionChanged -= value; }
        }

        public IReadOnlyCollection<ChatMessage> Messages => _messages;

        public IPEndPoint RemoteEndPoint => (IPEndPoint)_tcpClient.Client.RemoteEndPoint;

        public bool Connected
        {
            get { return _connected; }
            private set
            {
                _connected = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Send message to connected ChatModle.
        /// </summary>
        /// <param name="text">Text of message you want to send. Should't be null</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public async Task SendMessageAsync(string text)
        {
            if (text == string.Empty) throw new ArgumentException("Text should't be an empty string");
            if (!Connected) throw new InvalidOperationException("Chat should be connected");

            Exception capturedEx = null;
            try
            {
                var data = Encoding.UTF8.GetBytes(text);
                await _stream.WriteAsync(data.AsMemory(0, data.Length));
            }
            catch (Exception ex)
            {
                capturedEx = ex;
                Connected = false;
            }

            SendedMessage chatMessage;
            if (capturedEx != null)
            {
                chatMessage = new SendedMessage(text, capturedEx);
            }
            else
            {
                chatMessage = new SendedMessage(text);
            }
            _messages.Add(chatMessage);
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
                    _stream.Close();
                    _stream.Dispose();
                    _tcpClient.Close();
                    _tcpClient.Dispose();
                }
                _disposed = true;
            }
        }

        private async void ActivateListeningAsync()
        {
            try
            {
                while (Connected)
                {
                    var message = await ReciveMessageAsync();
                    MessageHandler(message);
                }
            }
            catch
            {
                Connected = false;
            }
        }

        private async ValueTask<string> ReciveMessageAsync()
        {
            var data = new byte[256];
            var message = new StringBuilder();

            do
            {
                var recivedBytes = await _stream.ReadAsync(data.AsMemory(0, data.Length));

                if (recivedBytes == 0)
                {
                    Connected = false;
                    return null;
                }

                message.Append(Encoding.UTF8.GetString(data, 0, recivedBytes));
            }
            while (_stream.DataAvailable);

            return message.ToString();
        }

        private void MessageHandler(string recivedMessage)
        {
            if (recivedMessage == null) return;

            var chatMessage = new RecivedMessage(recivedMessage);
            _messages.Add(chatMessage);
        }
    }
}
