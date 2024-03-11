using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RtelLogger;
using System.Diagnostics;
using RtelLogException;

namespace MisterTeamsUsersParserParser
{
    public class ServiceBaseLifeTime : ServiceBase, IHostLifetime
    {
        private readonly TaskCompletionSource<object> _delayStart;
       
        private IHostApplicationLifetime ApplicationLifetime { get; }

        public ServiceBaseLifeTime(IHostApplicationLifetime applicationLifetime)
        {
            _delayStart = new TaskCompletionSource<object>();
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                Program.Logger.WriteLogs("ServiceBaseLifeTime StopAsync.");
                Stop();
                Program.Logger.WriteLogs("ServiceBaseLifeTime StopAsync End.");
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Program.Logger.WriteLogs("ServiceBaseLifeTime WaitForStartAsync");
                cancellationToken.Register(() => _delayStart.TrySetCanceled());
                ApplicationLifetime.ApplicationStopping.Register(Stop);// Otherwise this would block and prevent IHost.StartAsync from finishing. 
                new Thread(Run).Start();                
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }

            return _delayStart.Task;
        }

        private void Run()
        {
            try
            {
                Program.Logger.WriteLogs("ServiceBaseLifeTime Run");

                Run(this); // This blocks until the ServiceBaseLifeTime is stopped.

                _delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));

                Program.Logger.WriteLogs("ServiceBaseLifeTime Run End");
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
        }

        protected override void OnStart(string[] args)
        {
                Program.Logger.WriteLogs("ServiceBaseLifeTime OnStart");

                _delayStart.TrySetResult(null);

                Program.Logger.WriteLogs("ServiceBaseLifeTime after TrySetResult");

                base.OnStart(args);

                Program.Logger.WriteLogs("ServiceBaseLifeTime after base.OnStart");
        }

        // Called by base.Stop. This may be called multiple times by  service Stop, ApplicationStopping, and StopAsync.
        // That's OK because StopApplication uses a CancellationTokenSource and prevents any recursion.
        protected override void OnStop()
        {
            try
            {
                Program.Logger.WriteLogs("ServiceBaseLifeTime OnStop");

                ApplicationLifetime.StopApplication();

                Program.Logger.WriteLogs("ServiceBaseLifeTime after ApplicationLifetime.StopApplication()");

                base.OnStop();

                Program.Logger.WriteLogs("ServiceBaseLifeTime base.OnStop() End");
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
        }
    }
}
