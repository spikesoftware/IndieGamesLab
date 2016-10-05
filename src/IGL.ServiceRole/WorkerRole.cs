using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using IGL.Service.Common;
using System.Reflection;
using IGL.Service;

namespace IGL.ServiceRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("IGL.ServiceRole is running");

            try
            {
                var rep = new IGL.Data.Repositories.RoleTaskRepository();
                var workers = new List<Task>();

                var result = rep.GetRoleTaskDefinitions();

                if (result.WasSuccessful)
                {
                    foreach (var definition in result.ResultObject)
                    {
                        var runnerType = Type.GetType(definition.Type);

                        RoleTaskRunner.OnGamePacketCompleted += RoleTaskRunner_OnGamePacketCompleted;
                        RoleTaskRunner.OnListenerError += RoleTaskRunner_OnListenerError;

                        Task task = typeof(RoleTaskRunner).GetMethod("RunAsync")
                                    .MakeGenericMethod(runnerType)
                                    .Invoke(null, new object[] { cancellationTokenSource.Token, definition.QueueName, 1, new TimeSpan(0, 0, 30) }) as Task;

                        workers.Add(task);
                    }                    

                    Task.WaitAll(workers.ToArray());
                }
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        private void RoleTaskRunner_OnListenerError(object sender, EventArgs e)
        {
            Trace.TraceInformation("IGL.RoleTaskRunner_OnListenerError triggered!");
        }

        private void RoleTaskRunner_OnGamePacketCompleted(object sender, GamePacketArgs e)
        {
            Trace.TraceInformation("IGL.RoleTaskRunner_OnGamePacketCompleted triggered!");
        }

        public override bool OnStart()
        {
            bool result = base.OnStart();

            Trace.TraceInformation("IGL.ServiceRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("IGL.ServiceRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("IGL.ServiceRole has stopped");
        }        
    }
}
