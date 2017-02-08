using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IGL.Configuration;

namespace IGL.Common.Tests
{
    [TestClass]
    public class GamePacketConfigurationTests
    {
        [TestMethod]
        public void ReadFromConfig()
        {

            GamePacketConfigurationSection config = (GamePacketConfigurationSection)System.Configuration.ConfigurationManager.GetSection("gamePacketConfigurationGroup/gamePacketConfiguration");

            Assert.IsTrue(config.SerializationConfiguration.IsJsonEnabled);
            Assert.IsTrue(config.EncryptionConfiguration.IsEncryptionEnabled);
            Assert.AreEqual("testing", config.EncryptionConfiguration.Salt);

            Assert.AreEqual("joeTester", config.PlayerId);
            Assert.AreEqual(100, config.GameId);

            Assert.AreEqual(config.EncryptionConfiguration.Salt, CommonConfiguration.Instance.EncryptionConfiguration.Salt);
            Assert.AreEqual(config.SerializationConfiguration.IsJsonEnabled, CommonConfiguration.Instance.SerializationConfiguration.IsJsonEnabled);

        }

        [TestMethod]
        public void AbleToOverride()
        {
            CommonConfiguration.Instance.EncryptionConfiguration.Salt = "fred";
            Assert.AreEqual("fred", CommonConfiguration.Instance.EncryptionConfiguration.Salt);

        }
    }
}
