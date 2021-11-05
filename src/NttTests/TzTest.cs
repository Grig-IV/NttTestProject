using NttClient.Models;
using NttLibrary.Converters;
using NttLibrary.Models;
using NttLibrary.ValidationRules;
using NttLibrary.ViewModels;
using NttServer.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xunit;

namespace NttTests
{
    public class TzTest : IDisposable
    {
        private ServerModel _server;
        private ClientModel _client;

        /// <summary>
        /// On each test run create and connect instance of server and client 
        /// </summary>
        public TzTest()
        {
            _server = new ServerModel();
            _server.TryListening(ServerIp, Port);

            _client = new ClientModel();
            _ = _client.TryConnectAsync(ServerIp, Port).Result;
        }

        private IPAddress ServerIp => IPAddress.Parse("127.0.0.1");

        private ushort Port => 8101;

        public void Dispose()
        {
            _server.Dispose();
        }


        /// <summary>
        /// ������:
        /// ������������� ����� ������������ �������� IP �\��� ������� �����
        /// 
        /// ������:
        /// �������� ������������� ������� ��������� � ������������� ���������� - ����� 
        /// ��������� � ������������ ����������.��������� �� ������ �������� �����������.
        /// </summary>
        [Theory]
        [InlineData("qwerfas", "eqwdas", false, false)]
        [InlineData("255.255.255.256", "65536", false, false)]
        [InlineData("255.255.255.255", "65535", true, true)]
        [InlineData("127.1", "8888", true, true)]
        public void IpPortValidation_shouldNotValidate(string ipField, string portField,
            bool expectedIpValidationResult, bool expectedPortValidationResult)
        {
            // Arrange
            var ipConverter = new IpConverter();
            var portConverter = new NullableUshortConverter();
            var ipValidaor = new IpValidationRule() { ValidationStep = ValidationStep.ConvertedProposedValue };
            var portValidaor = new UshortValidationRule() { ValidationStep = ValidationStep.ConvertedProposedValue };

            // Act
            var convertedIp = ipConverter.ConvertBack(ipField, null, null, null);
            var convertedPort = portConverter.ConvertBack(portField, null, null, null);
            var actualIPValidationResult = ipValidaor.Validate(convertedIp, null).IsValid;
            var actualPortValidationResult = portValidaor.Validate(convertedPort, null).IsValid;

            // Assert
            Assert.Equal(expectedIpValidationResult, actualIPValidationResult);
            Assert.Equal(expectedPortValidationResult, actualPortValidationResult);
        }

        /// <summary>
        /// ������
        /// ������� ��������� ������� ����� � �������� ��� ����������� � �������
        /// 
        /// ������:
        /// ��������� ����������� ������� ��� ����������� �������
        /// </summary>
        [Fact]
        public void ChatModel_shouldHasConnected()
        {
            // Assert
            Assert.True(_client.ActiveChat.Connected);
            Assert.True(_server.Chats.First().Connected);
        }

        /// <summary>
        /// ������� ��������� ���������� ����� � �������� ��� ������ �����:
        /// ������ ������, ������ ����������� �� Ethernet � �.�.
        /// </summary>
        [Fact]
        public void ChatModel_shouldHasNotConnected()
        {
            // Act
            var firstIsConnected = _client.ActiveChat.Connected;

            _server.Dispose();
            Task.Delay(2000).Wait();

            var secondIsNotConnected = _client.ActiveChat.Connected;

            // Assert
            Assert.True(firstIsConnected);
            Assert.False(secondIsNotConnected);
        }

        /// <summary>
        /// ���������� �������� ��������� ��� ���������� ����������� � �������
        /// </summary>
        [Fact]
        public void ChatViewModel_SendMessageCommand_ShouldCantBeExecute()
        {
            // Arrange
            var chatVM = new ChatViewModel(_client.ActiveChat, null);

            // Act
            _server.Dispose();
            var canExecute = chatVM.SendMessageCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        /// <summary>
        /// ������:
        /// ����������� ������������� ��������� + ���� � �������� ���������
        /// 
        /// ������:
        /// ����������� �������� ��������� � ��������� ������������
        /// 
        /// ���� � �������� ���������
        /// </summary>
        [Fact]
        public void ChatModel_SendMessageCommand_ShouldAddMessageInMessages()
        {
            // Arrange
            var clientChat = _client.ActiveChat;
            var serverChat = _server.Chats.First();

            // Act
            clientChat.SendMessageAsync("Message").Wait();
            Task.Delay(1000).Wait();

            var clientMessage = clientChat.Messages.First();
            var serverMessage = serverChat.Messages.First();

            // Assert
            Assert.Equal(clientMessage.Text, serverMessage.Text);
        }

        /// <summary>
        /// ����������� ��������� ��������� � ��������� �����������
        /// </summary>
        [Fact]
        public void ChatModel_MessagesShouldHaveRecivedMessage()
        {
            // Arrange
            var serverChat = _server.Chats.First();
            var clientChat = _client.ActiveChat;

            var messageText = "Message by server";

            // Act
            serverChat.SendMessageAsync(messageText).Wait();
            Task.Delay(1000).Wait();

            var message = clientChat.Messages.First();

            // Assert
            Assert.Equal(messageText, message.Text);
            Assert.IsType<RecivedMessage>(message);
        }

        /// <summary>
        /// �������� ������������� ������� ���������� ����� � ����������� �����������.
        /// ��������� �� ������ �������� �����������.
        /// /// </summary>
        [Fact]
        public void ServerModel_TryStart_ShouldReturnSocketException()
        {
            // Arrage
            var newServer = new ServerModel();

            // Act
            var exception = newServer.TryListening(ServerIp, Port);

            // Assert
            Assert.NotNull(exception);
        }

        /// <summary>
        /// ��������� ���������� ������� ��� ������ ����� � ��������: ������ �������,
        /// ������ ����������� �� Ethernet � �.�.
        /// </summary>
        [Fact]
        public void ServerChatModel_shouldHasNotConnected()
        {
            // Arrange
            var serverChat = _server.Chats.First();

            // Act
            var firstIsConnected = serverChat.Connected;

            _client.Dispose();
            Task.Delay(2000).Wait();

            var secondIsNotConnected = serverChat.Connected;

            // Assert
            Assert.True(firstIsConnected);
            Assert.False(secondIsNotConnected);
        }
    }
}
