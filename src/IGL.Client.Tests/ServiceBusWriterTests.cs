using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using System.Linq;
using IGL.Client.Tests.Helpers;

namespace IGL.Client.Tests
{
    [TestClass]
    public class ServiceBusWriterTests
    {
        [TestInitialize]
        public void Init()
        {
            Configuration.ServiceNamespace = "indiegameslab";
        }

        [TestMethod]
        public void SubmitGameEvents()
        {
#if DO_NOT_FAKE
            IGL.Client.Configuration.IssuerName = "[IssuerName]";
            IGL.Client.Configuration.IssuerSecret = "[IssuerSecret]";
            IGL.Client.Configuration.ServiceNamespace = "[ServiceNamespace]";
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

                for (int i = 0; i < 10; i++)
                    Assert.IsTrue(ServiceBusWriter.SubmitGameEvent("gameevents", 100, event1, additional.ToArray()));
#if !DO_NOT_FAKE
            }
#endif
        }

        private string GetWebData(string v)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestNamespace()
        {
            Assert.AreEqual("uri:igl:v1", GamePacket.Namespace);
        }
    }
}
