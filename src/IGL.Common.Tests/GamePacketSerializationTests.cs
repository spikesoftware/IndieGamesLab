using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace IGL.Common.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeJsonTest()
        {
            var gameEvent = Helpers.SampleGenerator.GameEventTestLarge();            
            var content = GameEventSerializer.Serialize(gameEvent);
            Assert.IsTrue(content.Length > 0);
            var gameEvent2 = GameEventSerializer.Deserialize(content);
            Assert.AreEqual(gameEvent.Properties.Count, gameEvent2.Properties.Count);
            Assert.AreEqual(gameEvent.Properties["stat_22"], gameEvent2.Properties["stat_22"]);
        }

        [TestMethod]
        public void GamePacketSerializeTest()
        {
            var gameEvent = Helpers.SampleGenerator.GameEventTestLarge();
            var content = JsonSerializerHelper.Serialize(gameEvent);
            Assert.IsTrue(content.Length > 0);
            var gameEvent2 = JsonSerializerHelper.Deserialize<GameEvent>(content);
            Assert.AreEqual(gameEvent.Properties.Count, gameEvent2.Properties.Count);
            Assert.AreEqual(gameEvent.Properties["stat_22"], gameEvent2.Properties["stat_22"]);
        }
    }
}
