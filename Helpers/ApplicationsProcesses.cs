using RestSharp;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MisterProtypoParser.Helpers
{
    public static class ApplicationsProcesses
    {
        private static Dictionary<int, SysApplicationsProcesses> _applicationsProcesses { get; set; } = new();
        private static MainForm _mainForm = null;

        public static void SetMainForm(MainForm mainForm) { _mainForm = mainForm; }

        public static void SetApplicationsProcesses()
        {
            //TODO: Dublicate code
            string SysApplicationsURL;
            switch (Program.sysApplication)
            {
                case SysApplications.LDAPParser:
                case SysApplications.MisterControlHub:
                    SysApplicationsURL = Program.MisterControlHubURL;
                    break;
                default: throw new Exception("Unknown Application");
            }

            //Get application Parameters
            var client = new RestClient($"{SysApplicationsURL}api/ApplicationsProcesses")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var result = JsonSerializer.Deserialize<List<SysApplicationsProcesses>>(response.Content, Program.JsonSerializerOptions);
                foreach (SysApplicationsProcesses sysApplicationsProcesses in result)
                    if (_applicationsProcesses.ContainsKey(sysApplicationsProcesses.ID)) _applicationsProcesses[sysApplicationsProcesses.ID] = sysApplicationsProcesses;
                    else _applicationsProcesses.Add(sysApplicationsProcesses.ID, sysApplicationsProcesses);
                _mainForm?.WriteLogs(SysEventLevel.Information, $"Succed get Processes.\n\rResponse: {response.Content}");
                Program.Logger.WriteLogs($"Succed get Processes.\n\rResponse: {response.Content}");
            }
            else
            {
                _mainForm?.WriteLogs(SysEventLevel.Warning, $"Failed to get Processes.\n\rResponse: {response.Content}");
                Program.Logger.WriteWarnings($"Failed to get Processes.\n\rResponse: {response.Content}");
            }
        }

        public static void UpdateApplicationsProcess(SysApplicationsProcesses sysApplicationsProcess)
        {
            //TODO: Dublicate code
            string SysApplicationsURL;
            switch (Program.sysApplication)
            {
                case SysApplications.LDAPParser:
                case SysApplications.MisterControlHub:
                    SysApplicationsURL = Program.MisterControlHubURL;
                    break;
                default: throw new Exception("Unknown Application");
            }

            //Get application Parameters
            var client = new RestClient($"{SysApplicationsURL}api/ApplicationsProcesses")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            request.AddParameter("application/json", JsonSerializer.Serialize(new List<SysApplicationsProcesses>() { sysApplicationsProcess }), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                if (_applicationsProcesses.ContainsKey(sysApplicationsProcess.ID)) _applicationsProcesses[sysApplicationsProcess.ID] = sysApplicationsProcess;
                else _applicationsProcesses.Add(sysApplicationsProcess.ID, sysApplicationsProcess);
                _mainForm?.WriteLogs(SysEventLevel.Information, $"Processes: {sysApplicationsProcess.ProcessName} Updated.\n\rsysApplicationsProcess: {sysApplicationsProcess}");
                Program.Logger.WriteLogs($"Processes: {sysApplicationsProcess.ProcessName} Updated.\n\rsysApplicationsProcess: {sysApplicationsProcess}");
            }
            else
            {
                _mainForm?.WriteLogs(SysEventLevel.Warning, $"Processes: {sysApplicationsProcess.ProcessName} Failed to Update Processes.\n\rResponse: {response.Content}");
                Program.Logger.WriteWarnings($"Processes: {sysApplicationsProcess.ProcessName} Failed toUpdate Processes.\n\rResponse: {response.Content}");
            }
        }

        public static void BlockChildProcesses(int ID)
        {
            if (string.IsNullOrEmpty(_applicationsProcesses[ID].ChildProcesses))
            {
                _mainForm?.WriteLogs(SysEventLevel.Information, $"Processes: {_applicationsProcesses[ID].ProcessName} has no child processes to Block");
                Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[ID].ProcessName} has no child processes to Block");
                return;
            }

            //TODO: Dublicate code
            string SysApplicationsURL;
            switch (Program.sysApplication)
            {
                case SysApplications.LDAPParser:
                case SysApplications.MisterControlHub:
                    SysApplicationsURL = Program.MisterControlHubURL;
                    break;
                default: throw new Exception("Unknown Application");
            }

            //Get application Parameters
            var client = new RestClient($"{SysApplicationsURL}api/ApplicationsProcesses/BlockChilds")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            request.AddParameter("application/json", JsonSerializer.Serialize(_applicationsProcesses[ID]), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
                SetApplicationsProcesses();
            else
            {
                _mainForm?.WriteLogs(SysEventLevel.Warning, $"Processes: {_applicationsProcesses[ID].ProcessName} Failed to Block Child Processes.\n\rResponse: {response.Content}");
                Program.Logger.WriteWarnings($"Processes: {_applicationsProcesses[ID].ProcessName} Failed to Block Child Processes.\n\rResponse: {response.Content}");
            }
        }

        public static void UnblockChildProcesses(int ID)
        {
            if (string.IsNullOrEmpty(_applicationsProcesses[ID].ChildProcesses))
            {
                _mainForm?.WriteLogs(SysEventLevel.Information, $"Processes: {_applicationsProcesses[ID].ProcessName} has no child processes to Unblock");
                Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[ID].ProcessName} has no child processes to Unblock");
                return;
            }

            //TODO: Dublicate code
            string SysApplicationsURL;
            switch (Program.sysApplication)
            {
                case SysApplications.LDAPParser:
                case SysApplications.MisterControlHub:
                    SysApplicationsURL = Program.MisterControlHubURL;
                    break;
                default: throw new Exception("Unknown Application");
            }

            //Get application Parameters
            var client = new RestClient($"{SysApplicationsURL}api/ApplicationsProcesses/UnblockChilds")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            request.AddParameter("application/json", JsonSerializer.Serialize(_applicationsProcesses[ID]), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
                SetApplicationsProcesses();
            else
            {
                _mainForm?.WriteLogs(SysEventLevel.Warning, $"Processes: {_applicationsProcesses[ID].ProcessName} Failed to Unblock Child Processes.\n\rResponse: {response.Content}");
                Program.Logger.WriteWarnings($"Processes: {_applicationsProcesses[ID].ProcessName} Failed to Unblock Child Processes.\n\rResponse: {response.Content}");
            }
        }

        public static SysApplicationsProcesses GetApplicationsProcess(int ID)
        {
            if (_applicationsProcesses.ContainsKey(ID)) return _applicationsProcesses[ID];
            else
            {
                SetApplicationsProcesses();
                return GetApplicationsProcess(ID);
            }
        }

        public static bool CheckIfApplicationsProcessCanRun(int ID)
        {
            //Check if application must run only once per date and if so check if allready run for the day
            if ((_applicationsProcesses[ID].RunOncePerDay ?? false) && 
                (_applicationsProcesses[ID].ProcessStatus == (int)ProcessStatus.ProcessFinish) &&
                (_applicationsProcesses[ID].UpdateDatetime.Date.CompareTo(DateTime.Now.Date) == 0) )
            {
                _mainForm?.WriteLogs(SysEventLevel.Warning, $"Processes: {_applicationsProcesses[ID].ProcessName} has already run for today");
                Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[ID].ProcessName} has already run for today");
                return false;
            }

            //Check process dependencies
            //This will help to run processes in order
            if (string.IsNullOrEmpty(_applicationsProcesses[ID].DependedProcesses))
            {
                _mainForm?.WriteLogs(SysEventLevel.Information, $"Processes: {_applicationsProcesses[ID].ProcessName} has no Depended processes");
                Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[ID].ProcessName} has no Depended processes");
            }
            else
            {
                var dependedProcesses = _applicationsProcesses[ID].DependedProcesses?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? System.Array.Empty<string>();
                foreach (string parentID in dependedProcesses)
                {
                    if (_applicationsProcesses[Convert.ToInt32(parentID)].RunOncePerDay ?? false && _applicationsProcesses[ID].UpdateDatetime.Date.CompareTo(DateTime.Now.Date) != 0)
                        return false;

                    switch (_applicationsProcesses[Convert.ToInt32(parentID)].ProcessStatus)
                        {
                        case (int)ProcessStatus.ProcessRequestToStart:
                        case (int)ProcessStatus.ProcessRequestedForApproval:
                        case (int)ProcessStatus.ProcessAproveParentToStart:
                        case (int)ProcessStatus.NotProcess:
                        case (int)ProcessStatus.RepeatProcess:
                        case (int)ProcessStatus.ProcessStart:
                        case (int)ProcessStatus.ProcessBuzy:
                            _mainForm?.WriteLogs(SysEventLevel.Warning, $"Processes: {_applicationsProcesses[Convert.ToInt32(parentID)].ProcessName} dependency of Processes: {_applicationsProcesses[ID].ProcessName} bloking the execution with ProcessStatus ID: {_applicationsProcesses[Convert.ToInt32(parentID)].ProcessStatus}");
                            Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[Convert.ToInt32(parentID)].ProcessName} dependency of Processes: {_applicationsProcesses[ID].ProcessName} bloking the execution with ProcessStatus ID: {_applicationsProcesses[Convert.ToInt32(parentID)].ProcessStatus}");
                            return false;
                    }
                }
            }

            //Check if process have child processes 
            if (string.IsNullOrEmpty(_applicationsProcesses[ID].ChildProcesses))
            {
                _mainForm?.WriteLogs(SysEventLevel.Information, $"Processes: {_applicationsProcesses[ID].ProcessName} has no child processes");
                Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[ID].ProcessName} has no child processes");
                return true;
            }

            //Check if any child block process
            var childProcessses = _applicationsProcesses[ID].ChildProcesses?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? System.Array.Empty<string>();
            foreach (string childID in childProcessses)
                if (_applicationsProcesses[Convert.ToInt32(childID)].ProcessStatus == (int)ProcessStatus.ProcessRequestedForApproval)
                {
                    _mainForm?.WriteLogs(SysEventLevel.Warning, $"Processes: {_applicationsProcesses[Convert.ToInt32(childID)].ProcessName} child of Processes: {_applicationsProcesses[ID].ProcessName} bloking the execution");
                    Program.Logger.WriteLogs($"Processes: {_applicationsProcesses[Convert.ToInt32(childID)].ProcessName} child of Processes: {_applicationsProcesses[ID].ProcessName} bloking the execution");
                    return false;
                }

            return true;
        }
    }
}
