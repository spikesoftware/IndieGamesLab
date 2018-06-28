using IGL.Common.Tests.Helpers;
using IGL.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IGL.Common.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void GameEventSerializerTest()
        {
            var gameEvent = SampleGenerator.GameEventTestLarge();
            var content = GameEventSerializer.Serialize(gameEvent);
            Assert.IsTrue(content.Length > 0);
            var gameEvent2 = GameEventSerializer.Deserialize(content);
            Assert.AreEqual(gameEvent.Properties.Count, gameEvent2.Properties.Count);
            Assert.AreEqual(gameEvent.Properties["stat_22"], gameEvent2.Properties["stat_22"]);
        }

        [TestMethod]
        public void JsonSerializerHelperTest()
        {
            var gameEvent = SampleGenerator.GameEventTestLarge();
            var content = JsonSerializerHelper.Serialize(gameEvent);
            Assert.IsTrue(content.Length > 0);
            var gameEvent2 = JsonSerializerHelper.Deserialize<GameEvent>(content);
            Assert.AreEqual(gameEvent.Properties.Count, gameEvent2.Properties.Count);
            Assert.AreEqual(gameEvent.Properties["stat_22"], gameEvent2.Properties["stat_22"]);
        }

        [TestMethod]
        public void DatacontractSerializerHelperTest()
        {
            var gameEvent = SampleGenerator.GameEventTestLarge();
            var content = DatacontractSerializerHelper.Serialize(gameEvent);
            Assert.IsTrue(content.Length > 0);
            var gameEvent2 = DatacontractSerializerHelper.Deserialize<GameEvent>(content);
            Assert.AreEqual(gameEvent.Properties.Count, gameEvent2.Properties.Count);
            Assert.AreEqual(gameEvent.Properties["stat_22"], gameEvent2.Properties["stat_22"]);
        }
    }
}