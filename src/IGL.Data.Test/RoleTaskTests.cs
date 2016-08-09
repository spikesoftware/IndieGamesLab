using Microsoft.VisualStudio.TestTools.UnitTesting;
using IGL.Service.Common;
using IGL.Data.Repositories;

namespace IGL.Data.Test
{
    [TestClass]
    public class RoleTaskTests
    {
        [TestMethod]
        public void InsertDefinition()
        {
#if !FAKES_NOT_SUPPORTED
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
#if !FAKES_NOT_SUPPORTED
            }
#endif
        }
    }
}
