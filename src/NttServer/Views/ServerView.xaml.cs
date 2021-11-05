using NttServer.ViewModels;
using System.Windows;

namespace NttServer.Views
{
    public partial class ServerView : Window
    {
        public ServerView()
        {
            InitializeComponent();
            var vm = new ServerViewModel();
            DataContext = vm;
            vm.CloseServerWindowAction = () => this.Close();
        }
    }
}
