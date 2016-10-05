using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using IGL.Service.Tasks;
using System.Threading;
using IGL.Service.Common;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using System.Collections.Specialized;
using Microsoft.ServiceBus.Fakes;
using Microsoft.WindowsAzure.Storage.Fakes;
using Microsoft.WindowsAzure.Storage.Table.Fakes;
using Microsoft.WindowsAzure.Storage.Queue.Fakes;
using Microsoft.ServiceBus.Messaging.Fakes;
using Microsoft.ServiceBus.Messaging;

namespace IGL.Service.Tests
{
    [TestClass]
    public class RoleTaskRunnerTests
    {
        [TestMethod]
        public void GetTaskRunnersTest()
        {            
            var iType = typeof(IRoleTask);
            var ass = Assembly.GetAssembly(typeof(RoleTaskRunner));

            var runners = ass.GetTypes().Where(t => iType.IsAssignableFrom(t) && !t.IsInterface);

            Assert.IsTrue(runners.Contains(typeof(GameEventsListenerTask)));
            Assert.IsFalse(runners.Contains(typeof(IRoleTask)));

        }

        [TestMethod]
        public void AddTaskRunnerTest()
        {
            var queueName = "GameEvents";
#if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Helpers.Faker.FakeOut();
#endif

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var runnerType = Type.GetType("IGL.Service.Tasks.GameEventsListenerTask, IGL.Service");

                RoleTaskRunner.OnGamePacketCompleted += RoleTaskRunner_OnGamePacketCompleted;
                RoleTaskRunner.OnListenerError += RoleTaskRunner_OnListenerError;

                var runner = typeof(RoleTaskRunner).GetMethod("RunAsync")
                            .MakeGenericMethod(runnerType)
                            .Invoke(null, new object[] { cancellationTokenSource.Token, queueName, 1, new TimeSpan(0, 0, 5) });

                cancellationTokenSource.Cancel();

                var task = (Task)runner;

                task.Wait();

                var name = runner.GetType().ToString();

#if !DO_NOT_FAKE
            }
#endif
        }

        private void RoleTaskRunner_OnListenerError(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RoleTaskRunner_OnGamePacketCompleted(object sender, GamePacketArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
