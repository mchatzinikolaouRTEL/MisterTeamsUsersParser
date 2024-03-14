using Azure.Identity;
using Dapper;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.ApplicationServices;
using MisterControlHubApiDto.RequestDtos;
using MisterControlHubApiDto.ResponceDtos;
using MisterTeamsUsersParser.Data.Models;
using MisterTeamsUsersParser.MainProcess;
using MisterTeamsUsersParserParser.Helpers;
using MisterTeamsUsersParserParser.Models;
using MisterTeamsUsersParserParser.Models.Dtos;
using Newtonsoft.Json.Linq;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MisterTeamsUsersParserParser.MainProcess
{
    internal class TeamsParsingFunctionality
    {

        #region Enums
        private Guid NewGUID;

        public Guid TransactionGuid
        {
            get { return NewGUID; }
            set { NewGUID = value; }
        }//TransactionGuid

        public string TransactionGuidString
        {
            get { return Convert.ToString(NewGUID).ToUpper(); }
        }//TransactionGuidString
        #endregion

        #region SysParameters
        SqlConnection Connection;
        GraphServiceClient GraphClient;
        string MisterMetricsConnectionString;
        private string TenantID;
        private string ClientId;
        private string ClientSecret;
        private int CommandTimeout;
        private bool dbMaintenanceMode;
        private bool ExeDebugMode;
        private string Scope;
        #endregion

        internal TeamsParsingFunctionality(Parameters parameters)
        {
            NewGUID = Guid.NewGuid();
            UnpackSysParameters(parameters);
            MisterMetricsConnectionString = Program.ConnectionString;
            GraphClient= SetupClient(TenantID, ClientId, ClientSecret);
        }//TeamsParsingFunctionality

        public async void ParseTeamsUsers()
        {
            int parsedUsers=0;
            var graphUserList = await GetGraphUsers(GraphClient);
            var databaseUserList = GetDatabaseUserUPNS(MisterMetricsConnectionString);

            foreach (var graphUser in graphUserList)
            {
                if (databaseUserList.Where(x => x.Equals(graphUser.UserPrincipalName)).IsNullOrEmpty())
                {
                    parsedUsers+=InsertUserIntoDatabase(MisterMetricsConnectionString, graphUser);
                }
                else
                {
                    parsedUsers += UpdateUserOnDatabase(MisterMetricsConnectionString, graphUser);
                }
            }
        }//ParseTeamsUsers

        void UnpackSysParameters(Parameters parameters)
        {
            //Unbox.
            List<SysParameters> sysParameters = parameters.SysParameters.ToList();
            foreach(SysParameters sysParameter in sysParameters)
            {
                sysParameter.ParamName = sysParameter.ParamName.ToLower();
            }
            TenantID = Convert.ToString(sysParameters.Where(x => x.ParamName == "TenantID".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ClientId = Convert.ToString(sysParameters.Where(x => x.ParamName == "ClientId".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ClientSecret = Convert.ToString(sysParameters.Where(x => x.ParamName == "ClientSecret".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            CommandTimeout = Convert.ToInt32(sysParameters.Where(x => x.ParamName == "CommandTimeout".ToLower()).Select(x => x.ParamValue).FirstOrDefault()); ;
            dbMaintenanceMode= Convert.ToBoolean(sysParameters.Where(x => x.ParamName == "dbMaintenanceMode".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ExeDebugMode =Convert.ToBoolean(sysParameters.Where(x => x.ParamName == "ExeDebugMode".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            Scope= Convert.ToString(sysParameters.Where(x => x.ParamName == "Scope".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
        }//UnpackSysParameters

        private List<string> GetDatabaseUserUPNS(string MetricsConnectionString)
        {
            List<string> result=new();
            using (var MetricsConnection= new SqlConnection(MetricsConnectionString))
            {
                var cmd = new SqlCommand("SELECT userPrincipalName FROM [Mister_Metrics].[dbo].[MG_UsersInformation]", MetricsConnection);
                cmd.CommandType = CommandType.Text;
                MetricsConnection.Open();
                using (SqlDataReader objReader = cmd.ExecuteReader())
                {
                    if (objReader.HasRows)
                    {
                        while (objReader.Read())
                        {
                            string item = objReader.GetString(objReader.GetOrdinal("userPrincipalName"));
                            result.Add(item);
                        }
                    }
                }
            }
            return result;
        }//GetDatabaseUserUPNS

        Dictionary<string,string> QueryPreparation(Microsoft.Graph.Models.User user,bool ForInsert)
        {
            Dictionary<string,string> result = new ();
            
            if (user.DisplayName != null)
            {
                result.Add("UserName",$"'{user.DisplayName}'");
                result.Add("DisplayName", $"'{user.DisplayName}'");
            }
            if (user.Surname != null)
            {
                result.Add("Surname", $"'{user.Surname}'");
            }
            if (user.PreferredLanguage != null){
                result.Add("preferredLanguage", $"'{user.PreferredLanguage}'");
            }
            if (user.OfficeLocation != null)
            {
                result.Add("officeLocation", $"'{user.OfficeLocation}'");
            }
            if (user.MobilePhone != null)
            {
                result.Add("mobilePhone", $"'{user.MobilePhone}'");
            }
            if (user.Mail != null)
            {
                result.Add("mail", $"'{user.Mail}'");
            }
            if (user.JobTitle != null)
            {
                result.Add("jobTitle", $"'{user.JobTitle}'");
            }
            if (user.GivenName != null)
            {
                result.Add("givenName", $"'{user.GivenName}'");
            }
            if (ForInsert)
            {
                result.Add("userPrincipalName", $"'{user.UserPrincipalName}'");
                result.Add("RecordID", $"'{user.Id}'");
                result.Add("InsertDateTime", $"'{DateTime.Now}'");
            }
            result.Add("UpdateDateTime", $"'{DateTime.Now}'");
            return result;
        }


        private int InsertUserIntoDatabase(string MetricsConnectionString, Microsoft.Graph.Models.User user)
        {
            var connection = new SqlConnection(MetricsConnectionString);
            connection.Open();
            var command = connection.CreateCommand();

            var queryDictionary = QueryPreparation(user, ForInsert:true);
           
            command.CommandText = $@"INSERT INTO [MG_UsersInformation] 
                                            (InsertUserId,UpdateUserId,{String.Join(',',queryDictionary.Keys)}) 
                                     VALUES (0,0,{String.Join(',',queryDictionary.Values)});";

            int rowsChanged= command.ExecuteNonQuery();
            connection.Close();
            return rowsChanged;
        }//InsertUserIntoDatabase


        private int UpdateUserOnDatabase(string MetricsConnectionString, Microsoft.Graph.Models.User user)
        {
            var queryDictionary = QueryPreparation(user,ForInsert:false);

            var connection = new SqlConnection(MetricsConnectionString);
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @$"UPDATE [MG_UsersInformation]
                              SET   UpdateUserID=0,{String.Join(',',queryDictionary.Select(x=>$"{x.Key}={x.Value}"))}
                              WHERE userPrincipalName='{user.UserPrincipalName}' OR RecordID='{user.Id}';";

            int rowsChanged = command.ExecuteNonQuery();
            connection.Close();
            return rowsChanged;
        }


        private async Task<List<Microsoft.Graph.Models.User>> GetGraphUsers(GraphServiceClient graphClient)
        {
            try
            {
                var users = await graphClient.Users.GetAsync();
                return users?.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new();
            }
        }//InsertUserIntoDatabase

        GraphServiceClient SetupClient(string TenantID, string ClientID, string ClientSecret)
        {
            // The client credentials flow requires default scope
            var scopes = new[] { "https://graph.microsoft.com/.default" }; //change this to a parameter

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            var clientSecretCredential = new ClientSecretCredential(
                TenantID, ClientID, ClientSecret, options);
            GraphServiceClient newClient = new GraphServiceClient(clientSecretCredential, scopes);

            return newClient;
        }//SetupClient
    }
}
