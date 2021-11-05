using NttLibrary.Models;
using NttLibrary.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace NttLibrary.ViewModels
{
    public class ChatViewModel : Notifier
    {
        private readonly ChatModel _chatModel;
        private readonly string _chatName;
        private string _messageField;
        private RelayCommand _sendMessageCommand;

        private ChatViewModel()
        {
            _messageField = string.Empty;
        }

        public ChatViewModel(ChatModel chatModel, string chatName) : this()
        {
            _chatModel = chatModel;
            _chatName = chatName;

            _chatModel.PropertyChanged += ChatModel_PropertyChanged;
            _chatModel.MessageCollectionChanged += ChatModel_MessageCollectionChanged;
        }

        public string ChatName => _chatName;

        public bool Connected => _chatModel.Connected;

        public IPEndPoint RemoteEndPoint => _chatModel.RemoteEndPoint;

        public int UnreadedMessageCount
            => _chatModel.Messages.OfType<RecivedMessage>().Where(m => !m.IsReaded).Count();

        public IReadOnlyCollection<ChatMessage> Messages => _chatModel.Messages;

        public string MessageField
        {
            get { return _messageField; }
            set
            {
                _messageField = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand SendMessageCommand
        {
            get => _sendMessageCommand ??= new RelayCommand(
                execute: async _ =>
                {
                    await _chatModel.SendMessageAsync(MessageField);
                    MessageField = string.Empty;  // Whipe message field after sending
                },
                canExecute: _ => _chatModel.Connected && MessageField != string.Empty);
        }

        /// <summary>
        /// Subscribe for ChatModel.Connected property changing.
        /// </summary>
        private void ChatModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatModel.Connected))
            {
                NotifyPropertyChanged(nameof(Connected));
            }
        }

        /// <summary>
        /// UnreadedMessageCount property changing binding.
        /// </summary>
        private void ChatModel_MessageCollectionChanged(object sender, NotifyCollectionChangedEventArgs collectionChangedEventArgs)
        {
            foreach (var recivedMessage in collectionChangedEventArgs.NewItems.OfType<RecivedMessage>())
            {
                // UnreadedMessageCount changed because new message recived
                NotifyPropertyChanged(nameof(UnreadedMessageCount));

                // Subscribe for IsReaded property changing for each ReceivedMessage.
                recivedMessage.PropertyChanged += (_, propertyChangedEvenArgs) =>
                {
                    if (propertyChangedEvenArgs.PropertyName == nameof(RecivedMessage.IsReaded))
                    {
                        // UnreadedMessageCount changed because recived message was readed
                        NotifyPropertyChanged(nameof(UnreadedMessageCount));
                    }
                };
            }
        }
    }
}
