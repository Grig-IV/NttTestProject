using NttServer.Models;
using System.Net;
using Xunit;

namespace NttTests
{
    public class ServerModelTest
    {
        private IPAddress ServerIp => IPAddress.Parse("127.0.0.1");
        private ushort Port => 8102;

        [Fact]
        public void ServerModel_TryListening_ShouldWorkAfterMultipleCall()
        {
            // Arrange
            var server = new ServerModel();

            // Act
            var socketEx_1 = server.TryListening(ServerIp, Port);
            var socketEx_2 = server.TryListening(ServerIp, Port);

            // Assert
            Assert.Null(socketEx_1);
            Assert.Null(socketEx_2);
        }
    }
}
