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
        /// Клиент:
        /// Невозможность ввода некорректных значений IP и\или номером порта
        /// 
        /// Сервер:
        /// Проверка невозможности запуска программы с некорректными настройкам - вывод 
        /// сообщения о некорректных настройках.программа не должна аварийно завершаться.
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
        /// Клиент
        /// Наличие индикации наличия связи с сервером при подключению к серверу
        /// 
        /// Сервер:
        /// Индикация подключения клиента при подключении клиента
        /// </summary>
        [Fact]
        public void ChatModel_shouldHasConnected()
        {
            // Assert
            Assert.True(_client.ActiveChat.Connected);
            Assert.True(_server.Chats.First().Connected);
        }

        /// <summary>
        /// Наличие индикации отсутствия связи с сервером при обрыве связи:
        /// «убили» сервер, разрыв подключения по Ethernet и т.п.
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
        /// Блокировка отправки сообщений при отсутствии подключения к серверу
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
        /// Клиент:
        /// Отображение отправленного сообщения + ввод и отправка сообщений
        /// 
        /// Сервер:
        /// Отображение принятых сообщений с указанием отправителей
        /// 
        /// Ввод и отправка сообщений
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
        /// Отображение принятого сообщение с указанием отправителя
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
        /// Проверка невозможности запуска нескольких копий с одинаковыми настройками.
        /// программы не должны аварийно завершаться.
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
        /// индикация отключения клиента при обрыве связи с клиентом: «убили» клиента,
        /// разрыв подключения по Ethernet и т.п.
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
