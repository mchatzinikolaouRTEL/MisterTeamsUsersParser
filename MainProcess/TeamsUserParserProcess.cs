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
using MisterProtypoParser.Helpers;
using MisterControlHubApiDto.HybridDtos;
using Microsoft.Graph;
using Azure.Identity;

namespace MisterProtypoParser.MainProcess
{
    public class TeamsUserParserProcess : AbstractMainProcess
    {
        private new const SysApplicationProcess _sysApplicationProcess = SysApplicationProcess.ReplicateLDAPData;

        private readonly IEnumerable<ActiveDirectoryAccounts> _activeDirectoryAccounts;
        private List<string> _columnsAlreadyExist;
        private Dictionary<int,List<string>> _sAMAccountNamesAlreadyExist;

        public TeamsUserParserProcess(Parameters parameters, MainForm mainForm = null) : base(parameters, mainForm)
        {
            _activeDirectoryAccounts = GetActiveDirectoryAccounts();
            GetLDAPDataColumns();
            GetSAMAccountNames();
        }

        public override void MainProcess()
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

            //Get connection string from config files.
            string connectionString =Program.ConnectionString;
            
            using (var connection = new SqlConnection(connectionString))
            {
            }


            var param = _parameters;

            // connect to GraphApi and get token
            //CreateGraphService()
            using (var client = GraphServiceClient(_parameters.TenantID, _parameters.ClientID, _parameters.ClientSecret))
            {

            }

            //Handle above with logging in SysTransactions?
            //if not ok log and exit
            //Get list of users from GraphApi
            //
            //get list of users from DB.
            //
            //Parse their difference into the DB.
            //


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

        private GraphServiceClient setupClient(string tenantID, string ClientID, string ClientSecret)
        {
            // The client credentials flow requires default scope
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var tenantId = tenantID;
            var clientId = ClientID;
            var clientSecret = ClientSecret;

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);
            GraphServiceClient newClient = new GraphServiceClient(clientSecretCredential, scopes);

            return newClient;
        }

        private IEnumerable<ActiveDirectoryAccounts> GetActiveDirectoryAccounts()
        {
            _mainForm?.WriteLogs(SysEventLevel.Information, "GetActiveDirectoryAccounts");
            Program.Logger.WriteLogs("GetActiveDirectoryAccounts");
            var objParameters = new { Deleted = false };

            using var connection = new SqlConnection(Program.ConnectionString);
            connection.Open();

            List<ActiveDirectoryAccounts> result = connection.Query<ActiveDirectoryAccounts>(
                "SELECT ID, InsertDateTime, InsertUserID, UpdateDateTime, UpdateUserID, HostIP, HostName, Username, Password, Location, Version, DetailedVersion, Deleted, Description FROM ActiveDirectoryAccounts WHERE Deleted = @Deleted",
                objParameters,
                commandType: System.Data.CommandType.Text).ToList();

            var e = new RtelEncryption();

            foreach (ActiveDirectoryAccounts activeDirectoryAccount in result)
            {
                _mainForm?.WriteLogs(SysEventLevel.Information, $"ActiveDirectoryAccount: {activeDirectoryAccount}");
                Program.Logger.WriteLogs($"ActiveDirectoryAccount: {activeDirectoryAccount}");
                activeDirectoryAccount.Username = RtelEncryption.DecryptString(activeDirectoryAccount.Username);
                activeDirectoryAccount.Password = RtelEncryption.DecryptString(activeDirectoryAccount.Password);                
            }

            return result;
        }

        private void GetLDAPDataColumns()
        {
            _mainForm?.WriteLogs(SysEventLevel.Information, "GetLDAPDataColumns");
            Program.Logger.WriteLogs("GetLDAPDataColumns");

            _columnsAlreadyExist = new();
            object parameters = new { TABLE_NAME = TableAndViewNames.LDAPData };

            using SqlConnection connection = new(Program.ConnectionString);            
            connection.Open();
            List<KeyValuePair<string,string>> tableColumns = connection.Query<KeyValuePair<string, string>>("SELECT COLUMN_NAME AS [Key],DATA_TYPE AS [Value] FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TABLE_NAME", parameters, commandType: CommandType.Text).ToList();
           
            foreach (KeyValuePair<string, string> column in tableColumns)
                _columnsAlreadyExist.Add(column.Key);
        }

        private bool AppendColumnFromProperty(string columnName, ResultPropertyValueCollection resultPropertyValueCollection, Parameters parameters)
        {
            columnName = columnName.Trim();
            Type DataType = resultPropertyValueCollection[0].GetType();

            SqlDbType DBType;

            if (DataType.FullName == "System.DBNull") DBType = SqlDbType.NVarChar;
            else DBType = DateTimeAndTypeConverters.ConvertToSqlDbType(DataType, resultPropertyValueCollection[0]);

            string commandText = DBType switch
            {
                SqlDbType.NVarChar => $"ALTER TABLE {TableAndViewNames.LDAPData} ADD [{columnName}] [{DBType}]({parameters.LDAP_DataTableNVarcharDefaultSize}) NULL ",
                SqlDbType.DateTime2 or SqlDbType.Date or SqlDbType.DateTime => $"ALTER TABLE {TableAndViewNames.LDAPData} ADD[{columnName}] [datetime2](7) NULL ",
                SqlDbType.Time => $"ALTER TABLE {TableAndViewNames.LDAPData} ADD[{columnName}] [time](0) NULL ",
                SqlDbType.VarBinary => $"ALTER TABLE {TableAndViewNames.LDAPData} ADD[{columnName}] [VarBinary]({parameters.LDAP_DataTableNVarcharDefaultSize}) NULL ",
                _ => $"ALTER TABLE {TableAndViewNames.LDAPData} ADD [{columnName}] [{DBType}] NULL ",
            };

            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();
            try
            {
                _mainForm?.WriteLogs(SysEventLevel.Information, $"AppendColumnFromProperty.\n\rcolumnName: {columnName}\n\rCommandText: {commandText}");
                Program.Logger.WriteLogs($"AppendColumnFromProperty.\n\rcolumnName: {columnName}\n\rCommandText: {commandText}");
                return connection.Execute(commandText, commandType: CommandType.Text) > 0;
            }
            catch (Exception ex)
            {
                _mainForm?.WriteErrors(ex.Message);
                var traceId = Activity.Current?.Id ?? _Guid.ToString();
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
                return false;
            }
        }

        private void GetSAMAccountNames()
        {
            _mainForm?.WriteLogs(SysEventLevel.Information, "GetSAMAccountNames");
            Program.Logger.WriteLogs("GetSAMAccountNames");

            _sAMAccountNamesAlreadyExist = new();
            using SqlConnection connection = new(Program.ConnectionString);            
            connection.Open();
            foreach (ActiveDirectoryAccounts activeDirectoryAccount in _activeDirectoryAccounts)
            {
                object parameters = new { ActiveDirectoryAccountsID = activeDirectoryAccount.ID };
                _sAMAccountNamesAlreadyExist.Add(activeDirectoryAccount.ID,
                    connection.Query<string>($"SELECT DISTINCT sAMAccountName FROM {TableAndViewNames.LDAPData} WHERE ActiveDirectoryAccountsID = @ActiveDirectoryAccountsID", parameters, commandType: CommandType.Text).ToList()
                    );
            }
        }

        private object GetPropertyValue(string propertyName, ResultPropertyValueCollection resultPropertyValueCollection, Parameters parameters)
        {
            if (resultPropertyValueCollection?.Count == 0) return null;

            try
            {
                object retvObject;
                if (propertyName.ToLower() == "memberof".ToLower())
                {
                    string retv = string.Empty;
                    for (int i = 0; i < resultPropertyValueCollection.Count; i++)
                    {
                        if ((retv + resultPropertyValueCollection[i].ToString() + "|").Length + 1 >= parameters.LDAP_DataTableNVarcharDefaultSize) break;
                        retv += resultPropertyValueCollection[i].ToString() + "|";
                    }
                    retvObject = $"|{Trim.Left(retv, parameters.LDAP_DataTableNVarcharDefaultSize - 1)}";
                }
                else retvObject = resultPropertyValueCollection[0];

                if (string.IsNullOrEmpty(Convert.ToString(retvObject)))
                {
                    _mainForm?.WriteLogs(SysEventLevel.Warning, $"PropertyName: {propertyName} is NULL or Empty");
                    Program.Logger.WriteLogs($"PropertyName: {propertyName} is NULL or Empty");
                    return null;
                }

                _mainForm?.WriteLogs(SysEventLevel.Information, $"GetPropertyValue.\n\rPropertyName: {propertyName}\n\rRetvObject: {retvObject}");
                Program.Logger.WriteLogs($"GetPropertyValue.\n\rPropertyName: {propertyName}\n\rRetvObject: {retvObject}");
                return retvObject;
            }
            catch (Exception ex)
            {
                _mainForm?.WriteErrors(ex.Message);
                var traceId = Activity.Current?.Id ?? _Guid.ToString();
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
                return null;
            }
        }

        private void InsertLDAPDataRow(ExpandoObject rowObject)
        {
            var dictionary = (IDictionary<string, object>)rowObject;
            string insertColumns = string.Empty;
            string parameterColumns = string.Empty;

            string lastKey = dictionary.Last().Key;
            foreach (string columnName in dictionary.Keys)
            {
                insertColumns += columnName;
                parameterColumns += $"@{columnName}";
                if (columnName != lastKey)
                {
                    insertColumns += ",";
                    parameterColumns += ",";
                }
            }
            string commandText = $"INSERT INTO {TableAndViewNames.LDAPData} ({insertColumns}) VALUES ({parameterColumns})";
            try
            {
                using SqlConnection connection = new(Program.ConnectionString);
                connection.Open();
                if (connection.Execute(commandText, rowObject, commandType: CommandType.Text) != 1)
                {
                    _mainForm?.WriteLogs(SysEventLevel.Warning, $"Insert Command return <> 1.\n\rCommandtext: {commandText}");
                    Program.Logger.WriteLogs($"Insert Command return <> 1.\n\rCommandtext: {commandText}");
                    return;
                }
                else
                {
                    string message = $"sAMAccountName: {rowObject.GetProperty("sAMAccountName")} inserted.";
                    _mainForm?.WriteLogs(SysEventLevel.Information, message);
                    _mainForm?.AddTotalRecords(_sysApplicationProcess, 1);
                    _mainForm?.AddInsertedRecords(_sysApplicationProcess, 1);
                    Program.Logger.WriteLogs(message);
                }
                _sAMAccountNamesAlreadyExist[(int)rowObject.GetProperty("ActiveDirectoryAccountsID")].Add((string)rowObject.GetProperty("sAMAccountName"));
            }
            catch (Exception ex)
            {
                _mainForm?.AddTotalRecords(_sysApplicationProcess, 1);
                _mainForm?.AddErrorRecords(_sysApplicationProcess, 1);
                _mainForm?.WriteErrors(ex.Message);
                var traceId = Activity.Current?.Id ?? _Guid.ToString();
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
            
        }

        private void UpdateLDAPDataRow(ExpandoObject rowObject)
        {
            var dictionary = (IDictionary<string, object>)rowObject;
            string lastKey = dictionary.Last().Key;

            string updateStatment = string.Empty;

            foreach (string columnName in dictionary.Keys)
            {
                updateStatment += $"{columnName}=@{columnName}";
                if (columnName != lastKey)
                    updateStatment += ",";
            }

            string commandText = $"UPDATE {TableAndViewNames.LDAPData} SET {updateStatment} WHERE ActiveDirectoryAccountsID = @ActiveDirectoryAccountsID AND sAMAccountName = @sAMAccountName";

            try
            {
                using SqlConnection connection = new(Program.ConnectionString);
                connection.Open();
                if (connection.Execute(commandText, rowObject, commandType: CommandType.Text) != 1)
                {
                    _mainForm?.WriteLogs(SysEventLevel.Warning, $"Update Command return <> 1.\n\rCommandtext: {commandText}");
                    Program.Logger.WriteLogs($"Update Command return <> 1.\n\rCommandtext: {commandText}");
                    return;
                }
                else
                {
                    string message = $"sAMAccountName: {rowObject.GetProperty("sAMAccountName")} updated.";
                    _mainForm?.WriteLogs(SysEventLevel.Information, message);
                    _mainForm?.AddTotalRecords(_sysApplicationProcess, 1);
                    _mainForm?.AddUpdatedRecords(_sysApplicationProcess, 1);
                    Program.Logger.WriteLogs(message);
                }
            }
            catch (Exception ex)
            {
                _mainForm?.AddTotalRecords(_sysApplicationProcess, 1);
                _mainForm?.AddErrorRecords(_sysApplicationProcess, 1);
                _mainForm?.WriteErrors(ex.Message);
                var traceId = Activity.Current?.Id ?? _Guid.ToString();
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
        }

        private void SetAllLDAPRecordsToStatusFalse()
        {
            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();

            connection.Execute($"UPDATE {TableAndViewNames.LDAPData} SET  [Status] = 0, UpdateDateTime = GETDATE()", commandType: CommandType.Text);

            _mainForm?.WriteLogs(SysEventLevel.Information, $"Status of all records of {TableAndViewNames.LDAPData} updated to 0");
            Program.Logger.WriteLogs($"Status of all records of {TableAndViewNames.LDAPData} updated to 0");
        }
    }
}
