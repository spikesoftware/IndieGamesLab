using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;
using IGL.Client.Tests.Helpers;
using IGL.Configuration;

namespace IGL.Client.Tests
{
    [TestClass]
    public class ServiceBusListenerTests
    {

        bool _failed = false;

        [TestMethod]
        public void ReceiveGameEventsFromIndieGamesLab()
        {
#if DO_NOT_FAKE
            CommonConfiguration.Instance.BackboneConfiguration.IssuerName = "[IssuerName]";
            CommonConfiguration.Instance.BackboneConfiguration.IssuerSecret = "[IssuerSecret]";
            CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "[ServiceNamespace]";            
            {
#elif USE_IGL_BACKBONE
            CommonConfiguration.Instance.BackboneConfiguration.IssuerName = "IGLGuestClient";
            CommonConfiguration.Instance.BackboneConfiguration.IssuerSecret = "2PenhRgdmlf6F1oNglk9Wra1FRH31pcOwbB3q4X0vDs=";
            CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "indiegameslab";
            { 
#else
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "avalidnamespace";

                Faker.FakeOut();
#endif                
                CommonConfiguration.Instance.PlayerId = "TestingTesting";

                ServiceBusListener.OnGameEventReceived += ServiceBusListener_OnGameEventReceived;
                ServiceBusListener.OnListenError += ServiceBusListener_OnListenError;

                using(var sbl = new ServiceBusListenerThread())
                {
                    sbl.StartListening();

                    Thread.CurrentThread.Join(5000);

                    sbl.StopListening();
                }

                var i = _packets.Count;

                Assert.IsFalse(_failed);
            }
        }

        [TestMethod]
        public void FailureTestCase()
        {
#if DO_NOT_FAKE
            CommonConfiguration.Instance.BackboneConfiguration.IssuerName = "[IssuerName]";
            CommonConfiguration.Instance.BackboneConfiguration.IssuerSecret = "[IssuerSecret]";
            CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "[ServiceNamespace]";            
            {
#elif USE_IGL_BACKBONE
            CommonConfiguration.Instance.BackboneConfiguration.IssuerName = "IGLGuestClient";
            CommonConfiguration.Instance.BackboneConfiguration.IssuerSecret = "2PenhRgdmlf6F1oNglk9Wra1FRH31pcOwbB3q4X0vDs=";
            CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "indiegameslab";
            {
#else
                using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Faker.FakeOut();
            #endif

                CommonConfiguration.Instance.PlayerId = "TestingTesting";
                CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "testingtesting";

                ServiceBusListener.OnGameEventReceived += ServiceBusListener_OnGameEventReceived;
                ServiceBusListener.OnListenError += ServiceBusListener_OnListenError;

                using (var sbl = new ServiceBusListenerThread())
                {
                    sbl.StartListening();

                    Thread.CurrentThread.Join(5000);

                    sbl.StopListening();
                }

                var i = _packets.Count;

                Assert.IsTrue(_failed);
            }
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
