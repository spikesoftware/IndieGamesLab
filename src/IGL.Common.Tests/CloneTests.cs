using System.Collections.Generic;
using IGL.Common.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IGL.Common.Tests
{
    [TestClass]
    public class CloneTests
    {
        [TestMethod]
        public void SomeCloneTests()
        {
            var event1 = SampleGenerator.GameEventTest1();
            var event2 = event1.CloneJson();

            Assert.AreEqual(event1.Properties.Count, event2.Properties.Count);
            Assert.AreEqual(event1.Properties["stat_kill_creepy"], event2.Properties["stat_kill_creepy"]);
        }

        [TestMethod]
        public void CloneListTests()
        {
            var list1 = new Dummy
            {
                MyIntList = new List<int> {1, 2, 3},
                MyProperty = 1,
                MyStringList = new List<string> {"1", "2", "3"}
            };
            var list2 = list1.CloneJson();

            Assert.AreEqual(list1.MyProperty, list2.MyProperty);
            Assert.AreEqual(list1.MyStringList.Count, list2.MyStringList.Count);
            Assert.AreEqual(list1.MyStringList[0], list2.MyStringList[0]);
        }
    }
}