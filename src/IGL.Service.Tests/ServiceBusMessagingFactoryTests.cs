using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IGL.Service.Tests
{
    [TestClass]
    public class ServiceBusMessagingFactoryTests
    {
        private readonly BackboneConfiguration _backboneConfiguration = new BackboneConfiguration();
        private IConfiguration _configuration;
        private string _correlation;
        private int _player100;
        private int _player101;
        private int _player102;
        private int _player103;
        private int _player104;

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
        public void SubscriptionByPlayerId()
        {
            _correlation = Guid.NewGuid().ToString();

            var factory = new ServiceBusMessagingFactory(_backboneConfiguration);

            for (var i = 100; i < 105; i++)
                Task.WaitAll(factory.CreateSubscription("playerevents", $"{i}", $"PlayerId = '{i}'"));

            var options = new MessageHandlerOptions(exceptionHandler)
            {
                AutoComplete = true
            };

            var subsClient100 = new SubscriptionClient(_backboneConfiguration.ConnectionString, "playerevents", "100");
            subsClient100.RegisterMessageHandler(handler, options);
            var subsClient101 = new SubscriptionClient(_backboneConfiguration.ConnectionString, "playerevents", "101");
            subsClient101.RegisterMessageHandler(handler, options);
            var subsClient102 = new SubscriptionClient(_backboneConfiguration.ConnectionString, "playerevents", "102");
            subsClient102.RegisterMessageHandler(handler, options);
            var subsClient103 = new SubscriptionClient(_backboneConfiguration.ConnectionString, "playerevents", "103");
            subsClient103.RegisterMessageHandler(handler, options);
            var subsClient104 = new SubscriptionClient(_backboneConfiguration.ConnectionString, "playerevents", "104");
            subsClient104.RegisterMessageHandler(handler, options);

            // send messages to each
            var topicclient = factory.GetTopicClientByName("playerevents").Result;
            var messages = new List<Message>();

            for (var msg = 0; msg < 20; msg++)
            for (var i = 100; i < 105; i++)
            {
                var message = new Message();
                message.CorrelationId = _correlation;
                message.UserProperties.Add("PlayerId", $"{i}");
                messages.Add(message);
            }
            Task.WaitAll(topicclient.SendAsync(messages));

            var sw = new Stopwatch();

            sw.Start();
            while (sw.ElapsedMilliseconds < 100000 &&
                   (_player100 != 20 ||
                    _player101 != 20 ||
                    _player102 != 20 ||
                    _player103 != 20 ||
                    _player104 != 20))
                Thread.Sleep(50);
            sw.Stop();

            Assert.AreEqual(20, _player100);
            Assert.AreEqual(20, _player101);
            Assert.AreEqual(20, _player102);
            Assert.AreEqual(20, _player103);
            Assert.AreEqual(20, _player104);
        }

        private Task exceptionHandler(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private Task handler(Message message, CancellationToken token)
        {
            return Task.Run(() =>
            {
                if (message.CorrelationId == _correlation)
                    switch (message.UserProperties["PlayerId"])
                    {
                        case "100":
                            _player100++;
                            break;
                        case "101":
                            _player101++;
                            break;
                        case "102":
                            _player102++;
                            break;
                        case "103":
                            _player103++;
                            break;
                        case "104":
                            _player104++;
                            break;
                    }
            });
        }
    }
}