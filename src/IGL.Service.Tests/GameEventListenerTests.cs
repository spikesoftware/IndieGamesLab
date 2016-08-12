using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using IGL.Service.Tasks;
using IGL.Data;

namespace IGL.Service.Tests
{
    [TestClass]
    public class GameEventListenerTaskTests
    {
        [TestMethod]
        public void GameEventListenerTaskTest1()
        {
            string queueName = "GameEvents";
#if !DO_NOT_FAKE
            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                Helpers.Faker.FakeOut();
#endif

                long starting_count = ServiceBusMessagingFactory.GetQueueSize("GameEvents");

                _gameevents = 0;

                List<Task> tasks = new List<Task>();

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                for (int i = 0; i < 3; i++)
                {
                    tasks.Add(RoleTaskRunner.RunAsync<GameEventsListenerTask>(cancellationTokenSource.Token, "GameEvents", 1, new TimeSpan(0, 0, 5)));
                }


                Thread.CurrentThread.Join(10000);

                cancellationTokenSource.Cancel();

                Task.WaitAll(tasks.ToArray());
                Trace.TraceInformation("GameEventListenerTaskTest1 after wait all");

                foreach (var task in tasks)
                    Assert.IsTrue(task.IsCompleted);

                // were there any processed messages?
                long current_count = ServiceBusMessagingFactory.GetQueueSize("GameEvents");

                Assert.IsTrue(current_count + _gameevents + _gameerrors == starting_count);

                Trace.TraceInformation("GameEventListenerTaskTest1 ending {0}", _gameevents);

#if !DO_NOT_FAKE
            }
#endif
        }

        void GameEventsListenerTask_OnListenerError(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void GameEventsListenerTask_OnGameEventError(object sender, GamePacketErrorArgs e)
        {
            Trace.TraceInformation("GameEventListenerTaskTest1 on game packet error");

            _gameerrors += 1;
        }


        int _gameevents = 0;
        int _gameerrors = 0;

        void GameEventsListenerTask_OnGameEvent(object sender, GamePacketArgs e)
        {
            Trace.TraceInformation("GameEventListenerTaskTest1 on game event");

            _gameevents += 1;
        }


    }
}
