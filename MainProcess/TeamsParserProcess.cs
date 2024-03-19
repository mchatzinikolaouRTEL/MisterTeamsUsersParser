using RtelLibrary.TableModels;
using RtelLogException;
using RtelHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using RtelEncryptionLibrary;
using System.DirectoryServices;
using System.Data;
using System.Dynamic;
using System.Text.RegularExpressions;
using RtelLibrary.Enums;
using RtelLibrary.DataBaseObjects;
using MisterTeamsUsersParser;
using MisterTeamsUsersParserParser.MainProcess;
using MisterTeamsUsersParserParser.Helpers;
using MisterTeamsUsersParserParser;

namespace MisterTeamsUsersParser.MainProcess
{
    public class ParseTeamsUsersProcess : AbstractMainProcess
    {
        private new const SysApplicationProcess _sysApplicationProcess = SysApplicationProcess.MisterTeamsUsersParser;
        Parameters parameters;
        public ParseTeamsUsersProcess(Parameters parameters, MainForm mainForm = null) : base(parameters, mainForm)
        { this.parameters = parameters; }

        public override void MainProcess()
        {
            _mainForm?.WriteLogs(SysEventLevel.Information, $"SysApplicationProcess: {_sysApplicationProcess} MainProcess Started");
            Program.Logger.WriteLogs($"SysApplicationProcess: {_sysApplicationProcess} MainProcess Started");
            SysApplicationsProcesses _applicationsProcess = ApplicationsProcesses.GetApplicationsProcess((int)_sysApplicationProcess);
            if (!ApplicationsProcesses.CheckIfApplicationsProcessCanRun((int)_sysApplicationProcess)) 
                return;

            switch (_applicationsProcess.ProcessStatus)
            {
                //The if above ensure that is not any child blocking the process
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

            //var TeamsParser=new TeamsParsingFunctionality(parameters);
            //TeamsParser.ParseTeamsUsers();

            #endregion
            _processStatus = ProcessStatus.ProcessFinish;
            _applicationsProcess.UpdateDatetime = _lastTimeExecuted = DateTime.Now;
            _applicationsProcess.ProcessStatus = (int)ProcessStatus.ProcessFinish;
            try
            {
                ApplicationsProcesses.UnblockChildProcesses(_applicationsProcess.ID);
                ApplicationsProcesses.UpdateApplicationsProcess(_applicationsProcess);
            }
            catch (Exception exc)
            {
                var traceId = Activity.Current?.Id ?? _Guid.ToString();
                LogException logException = new(exc, new StackTrace(exc, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
        }

    }
}
