using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using System.Collections.Specialized;
using Microsoft.ServiceBus.Fakes;
using Microsoft.WindowsAzure.Storage.Fakes;
using Microsoft.WindowsAzure.Storage.Table.Fakes;
using Microsoft.WindowsAzure.Storage.Queue.Fakes;
using Microsoft.ServiceBus.Messaging.Fakes;
using Microsoft.ServiceBus.Messaging;
using IGL.Service.Tests.Helpers;

namespace IGL.Service.Tests
{
    [TestClass]
    public class RoleTaskRunnerTests
    {
        int _errors = 0;
        int _success = 0;

        [TestMethod]
        public void AddTaskRunnerTest()
        {         
#if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Helpers.Faker.FakeOut();
#endif

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                RoleTaskRunner.OnGamePacketCompleted += RoleTaskRunner_OnGamePacketCompleted;
                RoleTaskRunner.OnListenerError += RoleTaskRunner_OnListenerError;

                var roleTask = new TestRoleTask();

                var task = RoleTaskRunner.RunAsync<TestRoleTask>(cancellationTokenSource.Token, "test", 1, new TimeSpan(0, 0, 1));

                task.Wait(3000);

                cancellationTokenSource.Cancel();

                Thread.Sleep(500);

            Assert.AreEqual(6, _errors);
            Assert.AreEqual(50, _success);

#if !DO_NOT_FAKE
            }
#endif
        }

        private void RoleTaskRunner_OnListenerError(object sender, EventArgs e)
        {
            _errors++;
        }

        private void RoleTaskRunner_OnGamePacketCompleted(object sender, GamePacketArgs e)
        {
            _success++;
        }
    }
}
