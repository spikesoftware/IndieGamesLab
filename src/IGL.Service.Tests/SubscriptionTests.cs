using System;
using System.Diagnostics;
using System.Threading;
using IGL.Configuration;
using IGL.Service.Tests.Helpers;
using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IGL.Service.Tests
{
    [TestClass]
    public class SubscriptionTests
    {
        [TestMethod]
        public void SubscriptionByPlayerId()
        {
            var correlation = Guid.NewGuid().ToString();

            CommonConfiguration.Instance.BackboneConfiguration.IssuerName = "IGLGuestClient";
            CommonConfiguration.Instance.BackboneConfiguration.IssuerSecret =
                "2PenhRgdmlf6F1oNglk9Wra1FRH31pcOwbB3q4X0vDs=";
            CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "indiegameslab";

            for (int i = 100; i < 105; i++)
            {
                ServiceBusMessagingFactory.CreateSubscription("playerevents", $"{i}", $"PlayerId = '{i}'");            
            }

            SubscriptionClient subsClient100 = ServiceBusMessagingFactory.CreateSubscriptionClient("playerevents", "100");
            SubscriptionClient subsClient101 = ServiceBusMessagingFactory.CreateSubscriptionClient("playerevents", "101");
            SubscriptionClient subsClient102 = ServiceBusMessagingFactory.CreateSubscriptionClient("playerevents", "102");
            SubscriptionClient subsClient103 = ServiceBusMessagingFactory.CreateSubscriptionClient("playerevents", "103");
            SubscriptionClient subsClient104 = ServiceBusMessagingFactory.CreateSubscriptionClient("playerevents", "104");

            // send messages to each
            var topicclient = ServiceBusMessagingFactory.GetTopicClientByName("playerevents");

            for (int msg = 0; msg < 20; msg++)
            {
                for (int i = 100; i < 105; i++)
                {
                    using (BrokeredMessage message = new BrokeredMessage())
                    {
                        message.CorrelationId = correlation;
                        message.Properties.Add("PlayerId", $"{i}");
                        topicclient.Send(message);
                    }
                }
            }

            var sw = new Stopwatch();
            var player100 = 0;
            var player101 = 0;
            var player102 = 0;
            var player103 = 0;
            var player104 = 0;

            sw.Start();
            while (sw.ElapsedMilliseconds < 100000 &&
                   player100 != 20 && player101 != 20 && player102 != 20 && player103 != 20 && player104 != 20)
            {
                if (ReceiveMessage(subsClient100, correlation, "100"))
                    player100++;
                if (ReceiveMessage(subsClient101, correlation, "101"))
                    player101++;
                if (ReceiveMessage(subsClient102, correlation, "102"))
                    player102++;
                if (ReceiveMessage(subsClient103, correlation, "103"))
                    player103++;
                if (ReceiveMessage(subsClient104, correlation, "104"))
                    player104++;

            }

            Assert.AreEqual(20, player100);
            Assert.AreEqual(20, player101);
            Assert.AreEqual(20, player102);
            Assert.AreEqual(20, player103);
            Assert.AreEqual(20, player104);
        }

        bool ReceiveMessage(SubscriptionClient client, string correlation, string playerId)
        {
            BrokeredMessage receivedMessage = client.Receive(TimeSpan.FromSeconds(3));

            return (receivedMessage != null &&
                    receivedMessage.CorrelationId == correlation &&
                    receivedMessage.Properties["PlayerId"].ToString() == playerId);

        }
    }
}
