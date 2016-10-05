using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;
using IGL.Client.Tests.Helpers;

namespace IGL.Client.Tests
{
    [TestClass]
    public class ServiceBusListenerTests
    {

        bool _failed = false;

        [TestMethod]
        public void ReceiveGameEventsFromIndieGamesLab()
        {
            #if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Faker.FakeOut();
            #endif

                ServiceBusListener.PlayerId = "TestingTesting";

                ServiceBusListener.OnGameEventReceived += ServiceBusListener_OnGameEventReceived;
                ServiceBusListener.OnListenError += ServiceBusListener_OnListenError;

                using(var sbl = new ServiceBusListener())
                {
                    sbl.StartListening();

                    Thread.CurrentThread.Join(5000);

                    sbl.StopListening();
                }

                var i = _packets.Count;

                Assert.IsFalse(_failed);
            #if !DO_NOT_FAKE
            }
            #endif

        }

        [TestMethod]
        public void FailureTestCase()
        {
            ServiceBusListener.PlayerId = "TestingTesting";
            Configuration.ServiceNamespace = "testingtesting";

            ServiceBusListener.OnGameEventReceived += ServiceBusListener_OnGameEventReceived;
            ServiceBusListener.OnListenError += ServiceBusListener_OnListenError;

            using (var sbl = new ServiceBusListener())
            {
                sbl.StartListening();

                Thread.CurrentThread.Join(5000);

                sbl.StopListening();
            }

            var i = _packets.Count;

            Assert.IsTrue(_failed);
        }

        List<GameEvent> _packets = new List<GameEvent>();
        

        void ServiceBusListener_OnListenError(object sender, System.IO.ErrorEventArgs e)
        {
            _failed = true;
        }

        void ServiceBusListener_OnGameEventReceived(object sender, GamePacketArgs e)
        {
            _packets.Add(e.GamePacket.GameEvent);
        }
    }
}
