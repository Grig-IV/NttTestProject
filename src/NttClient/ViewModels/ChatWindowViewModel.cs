using NttClient.Models;
using NttLibrary.Models;
using NttLibrary.Utils;
using NttLibrary.ViewModels;
using System;

namespace NttClient.ViewModels
{
    /// <summary>
    /// Heir of ChatViewModel with implemented ChangeServerCommand.
    /// </summary>
    public class ChatWindowViewModel : ChatViewModel
    {
        private RelayCommand _changeServerCommand;

        public ChatWindowViewModel(ChatModel chatModel, string chatName) : base(chatModel, chatName) { }

        /// <summary>
        /// Set Close action from View.
        /// </summary>
        public Action CloseChatWindowAction { get; set; }

        public RelayCommand ChangeServerCommand
        {
            get => _changeServerCommand ??= new RelayCommand(
                execute: chatWindow =>
                {
                    // Suggest previos end point.
                    var lastIp = RemoteEndPoint.Address;
                    var lastPort = (ushort)RemoteEndPoint.Port;

                    var client = SingletonProvider.GetSingelton<ClientModel>();
                    client.ActiveChat.Dispose();

                    var connctionWindow = new ConnectionWindowView(lastIp, lastPort);
                    connctionWindow.Show();
                    CloseChatWindowAction();
                });
        }
    }
}
