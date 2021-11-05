using NttLibrary.Models;
using NttLibrary.Utils;
using NttLibrary.ViewModels;
using NttServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows.Data;

namespace NttServer.ViewModels
{
    public class ServerViewModel : Notifier
    {
        private readonly ServerModel _server;
        private readonly ObservableCollection<ChatViewModel> _chatViewModeles;
        private readonly ICollectionView _chatsView;
        private RelayCommand _goToSettingWindow;

        public ServerViewModel()
        {
            _server = SingletonProvider.GetSingelton<ServerModel>();

            _chatViewModeles = CreateChatViewModels(_server.Chats);
            _chatsView = CreateChatCollectionView(_chatViewModeles);

            // Bind _chatViewModeles to server chat collection
            _server.ChatCollectionChanged += (_, e) =>
            {
                foreach (ChatModel newChat in e.NewItems)
                {
                    _chatViewModeles.Add(new ChatViewModel(newChat, "Client"));
                }
            };
        }

        public IPEndPoint LocalEndpoint => _server.LocalEndpoint;

        public ICollectionView ChatsView => _chatsView;

        /// <summary>
        /// Set Close action from View
        /// </summary>
        public Action CloseServerWindowAction { get; set; }

        public RelayCommand GoToSettingWindow
        {
            get => _goToSettingWindow ??= new RelayCommand(
                execute: _ =>
                {
                    var settingsWindow = new ServerSettingsView((ushort)LocalEndpoint.Port);
                    settingsWindow.Show();
                    CloseServerWindowAction();
                });
        }

        private ObservableCollection<ChatViewModel> CreateChatViewModels(IEnumerable<ChatModel> chatModels)
        {
            return new ObservableCollection<ChatViewModel>(chatModels.Select(c => new ChatViewModel(c, "Client")));
        }

        private ICollectionView CreateChatCollectionView(Collection<ChatViewModel> chatsCollection)
        {
            var collectionViewSource = new CollectionViewSource
            {
                Source = chatsCollection,
                IsLiveSortingRequested = true
            };

            var collectionView = collectionViewSource.View;

            var unreadedMessageSD = new SortDescription
            {
                PropertyName = nameof(ChatViewModel.UnreadedMessageCount),
                Direction = ListSortDirection.Descending,
            };
            var connectedSD = new SortDescription
            {
                PropertyName = nameof(ChatViewModel.Connected),
                Direction = ListSortDirection.Descending,
            };

            collectionView.SortDescriptions.Add(unreadedMessageSD);
            collectionView.SortDescriptions.Add(connectedSD);

            return collectionView;
        }
    }
}
