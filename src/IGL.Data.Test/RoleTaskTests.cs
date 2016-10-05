using Microsoft.VisualStudio.TestTools.UnitTesting;
using IGL.Service.Common;
using IGL.Data.Repositories;
using System.Collections.Generic;

namespace IGL.Data.Test
{
    [TestClass]
    public class RoleTaskTests
    {
        [TestMethod]
        public void InsertDefinition()
        {
#if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Helpers.Faker.FakeOut();
#endif

                var taskDef = new RoleTaskDefinition
                {
                    Name = "GameEvents",
                    QueueName = "gameevents",
                    Type = "IGL.Service.GameEventsRoleTask, IGL.Service",
                    Version = 1.0
                };

                var rep = new RoleTaskRepository();

                var result = rep.InsertOrReplaceDefinition(taskDef);

                Assert.AreEqual(true, result.WasSuccessful);
#if !DO_NOT_FAKE
            }
#endif
        }

        [TestMethod]
        public void GetRoleTaskDefinitions()
        {
#if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Helpers.Faker.FakeOut();
#endif

            var rep = new RoleTaskRepository();

            var result = rep.GetRoleTaskDefinitions();

            Assert.AreEqual(true, result.WasSuccessful);
#if !DO_NOT_FAKE
            }
#endif
        }
        
    }
}
