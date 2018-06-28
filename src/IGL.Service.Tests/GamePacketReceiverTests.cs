using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IGL.Serialization;
using IGL.Service.Tests.Helpers;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IGL.Service.Tests
{
    [TestClass]
    public class GamePacketReceiverTests
    {
        private readonly BackboneConfiguration _backboneConfiguration = new BackboneConfiguration();
        private IConfiguration _configuration;
        private string _correlation;
        private int _errors;
        private int _success;

        [TestInitialize]
        public void TestInitialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<BackboneConfiguration>();

            _configuration = builder.Build();

            _configuration.GetSection("IGL:Service:BackboneConfiguration")
                .Bind(_backboneConfiguration);

            if (string.IsNullOrEmpty(_backboneConfiguration?.SubscriptionId))
                Assert.Fail(
                    "The connection to Azure should be specified in the secrets.json file or the appsettings file.");
        }


        [TestMethod]
        public void GamePacketReceiver_NoSession()
        {
            _correlation = Guid.NewGuid().ToString();

            var factory = new ServiceBusMessagingFactory(_backboneConfiguration);

            var receiver = new TestGamePacketReceiver(_configuration, factory);
            receiver.OnGamePacketCompleted += Receiver_OnGamePacketCompleted;
            receiver.OnListenerError += Receiver_OnListenerError;

            Task.WaitAll(receiver.StartListening("GameEvents"));

            var client = factory.GetQueueClientByName("GameEvents").Result;

            for (var msg = 0; msg < 6; msg++)
            {
                var message = new Message
                {
                    CorrelationId = _correlation
                };
                client.SendAsync(message);
            }

            for (var msg = 0; msg < 50; msg++)
            {
                var packet = new GamePacket {GameEvent = new GameEvent(), EventId = 100};
                var content = Encoding.Default.GetBytes(DatacontractSerializerHelper.Serialize(packet));
                var message = new Message
                {
                    CorrelationId = _correlation,
                    Body = content
                };
                client.SendAsync(message);
            }

            var sw = new Stopwatch();

            sw.Start();
            while (sw.ElapsedMilliseconds < 50000 &&
                   (_errors != 6 || _success != 50))
                Thread.Sleep(50);
            sw.Stop();

            Assert.AreEqual(6, _errors);
            Assert.AreEqual(50, _success);
        }

        [TestMethod]
        public void GamePacketReceiver_WithSession()
        {
            _correlation = Guid.NewGuid().ToString();

            var factory = new ServiceBusMessagingFactory(_backboneConfiguration);

            var receiver = new TestGamePacketReceiver(_configuration, factory);
            receiver.OnGamePacketCompleted += Receiver_OnGamePacketCompleted;
            receiver.OnListenerError += Receiver_OnListenerError;

            Task.WaitAll(receiver.StartListening("GameEventsWithSession", "ASession"));

            var client = factory.GetQueueClientByName("GameEventsWithSession", true).Result;

            for (var msg = 0; msg < 10; msg++)
            {
                var packet = new GamePacket {GameEvent = new GameEvent(), EventId = 100};
                var content = Encoding.Default.GetBytes(DatacontractSerializerHelper.Serialize(packet));
                var message = new Message
                {
                    CorrelationId = _correlation,
                    SessionId = msg % 2 == 1 ? "ASession" : "Different",
                    Body = content
                };
                client.SendAsync(message);
            }

            var sw = new Stopwatch();

            sw.Start();
            while (sw.ElapsedMilliseconds < 50000 &&
                   (_errors != 0 || _success != 5))
                Thread.Sleep(50);
            sw.Stop();

            Assert.AreEqual(0, _errors);
            Assert.AreEqual(5, _success);
        }

        private void Receiver_OnListenerError(object sender, EventArgs e)
        {
            _errors++;
        }

        private void Receiver_OnGamePacketCompleted(object sender, GamePacketArgs e)
        {
            _success++;
        }
    }
}