using NttLibrary.Models;
using NttServer.Models;
using System;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace NttTests
{
    public class ChatModelTest : IDisposable
    {
        private ServerModel _server;

        /// <summary>
        /// On each test run create and connect instance of server and client 
        /// </summary>
        public ChatModelTest()
        {
            _server = new ServerModel();
            _server.TryListening(ServerIp, Port);
        }

        private IPAddress ServerIp => IPAddress.Parse("127.0.0.1");

        private ushort Port => 8101;

        public void Dispose()
        {
            _server.Dispose();
        }

        [Fact]
        public void ChatModel_ShouldThrowObjectDisposedException()
        {
            // Arrange
            var tcpClient = new TcpClient();
            var expectedException = typeof(ObjectDisposedException);

            // Act
            tcpClient.Connect(ServerIp, Port);
            tcpClient.Close();
            Action testCode = () => new ChatModel(tcpClient);

            // Arrange
            Assert.Throws(expectedException, testCode);
        }

        [Fact]
        public void ChatModel_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var tcpClient = new TcpClient();
            var expectedException = typeof(InvalidOperationException);

            // Act
            Action testCode = () => new ChatModel(tcpClient);

            // Arrange
            Assert.Throws(expectedException, testCode);
        }
    }
}
