using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using System.Linq;
using IGL.Client.Tests.Helpers;
using System.Threading;
using IGL.Configuration;

namespace IGL.Client.Tests
{
    [TestClass]
    public class ServiceBusWriterTests
    {
        [TestInitialize]
        public void Init()
        {
            CommonConfiguration.Instance.BackboneConfiguration.ServiceNamespace = "indiegameslab";
        }

        [TestMethod]
        public void SubmitGameEvents()
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
                Dictionary<string, string> properties = new Dictionary<string, string>();

                // at the end of a level a number of statistics are uploaded
                properties.Add("stat_kill_roamer", "1");
                properties.Add("stat_kill_creepy", "3");
                properties.Add("stat_case_unlocked", "1");
                properties.Add("stat_level_1_complete", "1");
                properties.Add("stat_avg_level_1", "360");   // the total elapsed time in seconds
                properties.Add("stat_average_level_speed", "330");    // the played elapsed time in seconds taking pause into considertion

                var event1 = new GameEvent
                {
                    Properties = properties
                };

                Dictionary<string, string> additional = new Dictionary<string, string>();

                additional.Add("BUILD_ID", "65580");
                additional.Add("EVENT_DATE", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                additional.Add("SESSION_ID", Guid.NewGuid().ToString());

                // retrieve security token
                var token = ServiceBusWriter.Token;

                var retries = 0;
                while (token == null)
                {
                    Thread.Sleep(30);
                    token = ServiceBusWriter.Token;
                    retries++;

                    Assert.IsTrue(retries < 100, "Failed number of retries.  Take a look at the configuration.");
                }                

                for (int i = 0; i < 10; i++)
                    Assert.IsTrue(ServiceBusWriter.SubmitGameEvent("gameevents", 100, event1, additional.ToArray()));
            }
        }

        [TestMethod]
        public void TestNamespace()
        {
            Assert.AreEqual("uri:igl:v1", GamePacket.Namespace);
        }
    }
}
