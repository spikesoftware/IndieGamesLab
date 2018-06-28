using System.Diagnostics;
using IGL.Common.Tests.Helpers;
using IGL.Configuration;
using IGL.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IGL.Common.Tests
{
    [TestClass]
    public class EncryptTests
    {
        [TestMethod]
        public void GameEventEncryptionTest()
        {
            CommonConfiguration.Instance.EncryptionConfiguration.IsEncryptionEnabled = true;
            CommonConfiguration.Instance.EncryptionConfiguration.Salt = "o6806642kbM7c5";

            var event1 = SampleGenerator.GameEventTest1();
            var packet = SampleGenerator.GamePacketTest1();

            packet.GameEvent = event1;

            // is the content encrypted
            Assert.IsTrue(packet.Content.Contains("76561198024856042") == false);

            Assert.AreEqual(event1.Properties["stat_avg_level_1"], packet.GameEvent.Properties["stat_avg_level_1"]);
        }


        [TestMethod]
        public void EncryptionTests()
        {
            CommonConfiguration.Instance.EncryptionConfiguration.IsEncryptionEnabled = true;
            CommonConfiguration.Instance.EncryptionConfiguration.Salt = "o6806642kbM7c5";

            var sw = new Stopwatch();
            sw.Start();
            var value = SampleGenerator.GameEventTestLarge();
            var p1 = sw.ElapsedMilliseconds;
            var serialised = JsonSerializerHelper.Serialize(value);
            var p2 = sw.ElapsedMilliseconds;
            var encrypted = GamePacket.EncryptStringAES(serialised);
            var p3 = sw.ElapsedMilliseconds;

            var dSerialisation = p2 - p1;
            var dEncryption = p3 - p2;
        }
    }
}