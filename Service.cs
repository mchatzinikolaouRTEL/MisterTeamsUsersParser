using Microsoft.Extensions.Hosting;
using MisterControlHubApiDto.RequestDtos;
using MisterProtypoParser.Helpers;
using MisterProtypoParser.MainProcess;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using RtelLogException;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MisterProtypoParser
{
    public class Service : IHostedService, IDisposable
    {
        private System.Timers.Timer _timer;        
        //TODO: make it parameters
        private int _timeInterval = 1;
        private bool _synchroniseModeEnable = false;
        private Dictionary<SysApplicationProcess, IMainProcess> mainProcesses;
        private Dictionary<SysApplicationProcess, List<SysParameters>> processParameters;
        private Dictionary<SysApplicationProcess, List<SysParametersDetails>> processParameterDetails;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mainProcesses = new();
            processParameters = new();
            processParameterDetails = new();

            Program.Logger.WriteLogs("Service StartAsync");

            _timer = new System.Timers.Timer
            {
                Interval = _timeInterval * 60 * 1000
            };
            _timer.Elapsed += Timer_Elapsed;

            try
            {
                processParameters.Add(SysApplicationProcess.ReplicateLDAPData, new());
                Parameters.GetParameters(SysApplicationProcess.ReplicateLDAPData, ref processParameters, ref processParameterDetails);
                Parameters parameters = new(processParameters[SysApplicationProcess.ReplicateLDAPData], processParameterDetails[SysApplicationProcess.ReplicateLDAPData], SysApplicationProcess.ReplicateLDAPData);
                mainProcesses.Add(
                    SysApplicationProcess.ReplicateLDAPData,
                    new TeamsUserParserProcess(parameters)
                    );

                Thread ThreatStarter = new Thread(new ThreadStart(() => Synchroniser(_synchroniseModeEnable, Statics.GetProperDateTime("##:#0:00"))));
                ThreatStarter.Start();

                Program.Logger.WriteLogs("Service StartAsync Ended");
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }

            return Task.CompletedTask;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (SysApplicationProcess sysApplicationProcess in mainProcesses.Keys)
                {
                    switch (mainProcesses[sysApplicationProcess].Status)
                    {                                     
                        case ProcessStatus.RepeatProcess:
                        case ProcessStatus.ProcessStart:
                        case ProcessStatus.ProcessBuzy:
                            Program.Logger.WriteLogs($"Timer_Elapsed. {mainProcesses[sysApplicationProcess].Status}");
                            break;
                        default:
                            Parameters.GetParameters(sysApplicationProcess, ref processParameters, ref processParameterDetails);
                            Thread ThreatStarter = new(new ThreadStart(() => mainProcesses[sysApplicationProcess].Start()));
                            ThreatStarter.Start();
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Program.Logger.WriteLogs("Service StopAsync");

            _timer?.Stop();
            _timer?.Dispose();
            Thread.Sleep(1000);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Program.Logger.WriteLogs("Service Dispose");
            _timer = null;
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private void Synchroniser(bool SynchroniseMode, TimeSpan myDateResult)
        {
            Program.Logger.WriteLogs("Service Synchroniser");
            if (SynchroniseMode)
            {
                Program.Logger.WriteLogs("Service Synchroniser before sleep");
                Thread.Sleep(myDateResult);
                Program.Logger.WriteLogs("Service Synchroniser after sleep");
                Timer_Elapsed(null, null);
                _timer.Start();                
            }
            else
            {
                _timer.Start();
            }
        }//Synchroniser
    }
}
