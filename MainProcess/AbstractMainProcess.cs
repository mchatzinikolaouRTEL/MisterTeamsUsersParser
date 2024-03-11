using MisterTeamsUsersParserParser.Helpers;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using RtelLogException;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MisterTeamsUsersParserParser.MainProcess
{

    public abstract class AbstractMainProcess : IMainProcess
    {
        internal const SysApplicationProcess _sysApplicationProcess = SysApplicationProcess.Unknown;

        internal Parameters _parameters;
        internal readonly MainForm _mainForm;

        internal DateTime _lastTimeExecuted;
        internal ProcessStatus _processStatus;        
        internal ActiveTimeRanges activeTimeRanges = new ();
        internal CancellationTokenSource tokenSource = new();
        internal SysApplicationsProcesses _applicationsProcess;
        internal Guid _Guid;

        public DateTime LastTimeExecuted => _lastTimeExecuted;
        public ProcessStatus Status => _processStatus;
        public Parameters Parameters 
        {
            set
            {
                _parameters = value;
                if (!activeTimeRanges.ICanRun(_parameters.SysParameters))
                {
                    switch (_processStatus)
                    {
                        case ProcessStatus.Unknown:
                        case ProcessStatus.ProcessRequestToStart:
                        case ProcessStatus.ProcessRequestedForApproval:
                        case ProcessStatus.ProcessAproveParentToStart:
                        case ProcessStatus.NotForProcess:
                        case ProcessStatus.ProcessStart:
                        case ProcessStatus.ProcessBuzy:
                        default: //Do nothing
                            break;                                   
                        case ProcessStatus.NotProcess:
                        case ProcessStatus.RepeatProcess:
                        case ProcessStatus.ProcessError:
                        case ProcessStatus.ProcessFinish:
                            _processStatus = ProcessStatus.NotForProcess;
                            break;
                    }
                }
            }
        }

        public void Stop()
        {
            tokenSource.Cancel();
            _processStatus = ProcessStatus.ProcessFinish;
            _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
            _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessFinish;            
            ApplicationsProcesses.UnblockChildProcesses(_applicationsProcess.ID);
            ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
            _mainForm?.WriteLogs(SysEventLevel.Information, $"SysApplicationProcess: {_sysApplicationProcess} Stoped");
            Program.Logger.WriteLogs($"SysApplicationProcess: {_sysApplicationProcess} Stoped");
        }

        public async Task Start()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            Task t1 = Task.Factory.StartNew(() =>
            {
                _processStatus = ProcessStatus.ProcessStart;
                while (!token.IsCancellationRequested || _processStatus != ProcessStatus.ProcessFinish || activeTimeRanges.ICanRun(_parameters.SysParameters))
                {
                    _processStatus = ProcessStatus.RepeatProcess;
                    _lastTimeExecuted = DateTime.Now;
                    Thread.Sleep(100); //Throw if cancelled
                    _mainForm?.WriteLogs(SysEventLevel.Information, $"SysApplicationProcess: {_sysApplicationProcess} Start");
                    Program.Logger.WriteLogs($"SysApplicationProcess: {_sysApplicationProcess} Start");
                    MainProcess();
                }
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
            },
            token);

            try
            {
                t1.Wait();
            }
            catch (AggregateException ex)
            {
                var traceId = Activity.Current?.Id ?? _Guid.ToString();
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                _mainForm?.WriteErrors(logException.ExceptionMessage);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
                _processStatus = ProcessStatus.ProcessError;
                _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
                _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessError;                
                try
                {
                    ApplicationsProcesses.UnblockChildProcesses(_applicationsProcess.ID);
                    ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
                }
                catch (Exception exc)
                {
                    logException = new(exc, new StackTrace(exc, true).GetFrames(), traceId);
                    _mainForm?.WriteErrors(logException.ExceptionMessage);
                    Program.Logger.WriteErrors(logException.ExceptionMessage);
                }
            }
        }

        public virtual void MainProcess()
        {
            _mainForm?.WriteLogs(SysEventLevel.Information, $"SysApplicationProcess: {_sysApplicationProcess} MainProcess Started");
            Program.Logger.WriteLogs($"SysApplicationProcess: {_sysApplicationProcess} MainProcess Started");
            SysApplicationsProcesses _applicationsProcess = ApplicationsProcesses.GetApplicationsProcess((int)_sysApplicationProcess);
            if (!ApplicationsProcesses.CheckIfApplicationsProcessCanRun((int)_sysApplicationProcess)) return;

            switch (_applicationsProcess.ProcessStatus)
            {
                //The if above ensure that is not any child bloking the process
                case (int)ProcessStatus.ProcessRequestToStart:
                    _processStatus = ProcessStatus.ProcessStart;
                    _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
                    _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessStart;
                    ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
                    break;
                //Aprove parent and close
                case (int)ProcessStatus.ProcessRequestedForApproval:
                    _processStatus = ProcessStatus.ProcessAproveParentToStart;
                    _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
                    _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessAproveParentToStart;
                    ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
                    return;
                //Wait until parent unblock the process
                case (int)ProcessStatus.ProcessAproveParentToStart:
                    _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
                    ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
                    return;
                case (int)ProcessStatus.NotProcess:
                case (int)ProcessStatus.ProcessFinish:
                case (int)ProcessStatus.ProcessError:
                    ApplicationsProcesses.BlockChildProcesses((int)_sysApplicationProcess);
                    _processStatus = ProcessStatus.ProcessRequestToStart;
                    _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
                    _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessRequestToStart;
                    ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
                    return;
            }

            #region Custom Code here
            #endregion

            _processStatus = ProcessStatus.ProcessFinish;
            _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
            _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessFinish;
            ApplicationsProcesses.UnblockChildProcesses(_applicationsProcess.ID);
            ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
            _mainForm?.WriteLogs(SysEventLevel.Information, $"SysApplicationProcess: {_sysApplicationProcess} MainProcess Finished");
            Program.Logger.WriteLogs($"SysApplicationProcess: {_sysApplicationProcess} MainProcess Finished");
        }

        protected AbstractMainProcess()
        {
            _lastTimeExecuted = DateTime.Now;
            _processStatus = ProcessStatus.ProcessStart;            
            _Guid = Guid.NewGuid();
            _applicationsProcess = new();
        }

        protected AbstractMainProcess(Parameters parameters, MainForm mainForm = null) : this()
        {
            _parameters = parameters;
            _mainForm = mainForm;
        }
    }
}
