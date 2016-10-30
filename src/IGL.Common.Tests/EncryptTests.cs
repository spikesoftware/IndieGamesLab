using Microsoft.VisualStudio.TestTools.UnitTesting;
using IGL.Common.Tests.Helpers;
using System.Collections.Specialized;

namespace IGL.Common.Tests
{
    [TestClass]
    public class EncryptTests
    {
        [TestMethod]
        public void GameEventEncryptionTest()
        {
#if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Faker.FakeOut();
#endif
                var event1 = SampleGenerator.GameEventTest1();
                var packet = SampleGenerator.GamePacketTest1();

                packet.GameEvent = event1;

                // is the content encrypted
                Assert.IsTrue(packet.Content.Contains("76561198024856042") == false);
                
                Assert.AreEqual(event1.Properties["stat_avg_level_1"], packet.GameEvent.Properties["stat_avg_level_1"]);
#if !DO_NOT_FAKE
            }
#endif
        }
    }
}
