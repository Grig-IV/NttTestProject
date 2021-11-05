using NttClient.Models;
using NttClient.ViewModels;
using NttLibrary.Utils;
using System.Windows;

namespace NttClient.Views
{
    /// <summary>
    /// ChatView warper.
    /// </summary>
    public partial class ChatWindowView : Window
    {
        public ChatWindowView()
        {
            InitializeComponent();

            var client = SingletonProvider.GetSingelton<ClientModel>();
            var vm = new ChatWindowViewModel(client.ActiveChat, "Server");

            DataContext = vm;
            vm.CloseChatWindowAction = () => this.Close();
        }
    }
}
