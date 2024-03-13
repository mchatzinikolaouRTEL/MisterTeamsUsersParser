using Azure.Identity;
using Dapper;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.ApplicationServices;
using MisterControlHubApiDto.RequestDtos;
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
        private const string ArgumentMonthlyDataTable = "MonthlyDataTable";
        private const string ArgumentMonthlyFolder = "MonthlyFolder";
        private const string Mister_Recording = "Mister_Recording";

        private const string CallsRecorded = "CallsRecorded";
        private const string NonClosedNormalSessions = "NonClosedNormalSessions";
        private const string Participants = "Participants";
        private const string Sessions = "Sessions";
        private const string Tracks = "Tracks";

        #region Enums
        /*public enum Applications
        {
            [Description("Common App ID")]
            CommonAppID = 0,
            [Description("App ID")]
            AppID = 20070,
            [Description("Main App ID")]
            MainAppID = 20000,
            [Description("Service ID")]
            ServiceID = 20071,
        }//Applications

        public enum ApplicationEntities
        {
            [Description("ApplicationEntities")]
            applicationEntities = 0,
        }//ApplicationEntities

        public enum DataTableCreateMetaDataAction
        {
            [Description("Procedure")]
            Procedure = 1,
            [Description("Code Same DataTable")]
            CodeSameDataTable = 2,
            [Description("Code MetaData DataTable")]
            CodeMetaDataDataTable = 3,
        }//DataTableCreateMetaDataAction

        public enum DataTableMoveAction
        {
            [Description("Procedure")]
            Procedure = 1,
            [Description("Code")]
            Code = 2,
            [Description("Code By Date")]
            CodeByDate = 3,
        }//DataTableMoveAction
        */
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
        string MetricsConnectionString;
        private string TenantID;
        private string ClientId;
        private string ClientSecret;
        private int CommandTimeout;
        private bool dbMaintenanceMode;
        private bool ExeDebugMode;
        private string Scope;
        #endregion

        internal TeamsParsingFunctionality(Parameters parameters,string metricsConnectionString= "Data Source=SQL-SRV01-RTEL;database=Mister_Metrics;User Id=Mister_Metrics;Password=m1st3RL0g!n;")
        {
            NewGUID = Guid.NewGuid();
            UnpackSysParameters(parameters);
            MetricsConnectionString = metricsConnectionString;
            GraphClient= SetupClient(TenantID, ClientId, ClientSecret);
        }//TeamsParsingFunctionality

        public async void ParseTeamsUsers()
        {
            int parsedUsers=0;
            var graphUserList = await GetGraphUsers(GraphClient);
            var databaseUserList = GetDatabaseUserUPNS(MetricsConnectionString);

            foreach (var graphUser in graphUserList)
            {
                if (databaseUserList.Where(x => x.Equals(graphUser.UserPrincipalName)).IsNullOrEmpty())
                {
                    parsedUsers+=InsertUserIntoDatabase(MetricsConnectionString, graphUser);
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

        private int InsertUserIntoDatabase(string MetricsConnectionString, Microsoft.Graph.Models.User user)
        {
            var connection = new SqlConnection(MetricsConnectionString);
            connection.Open();
            var command = connection.CreateCommand();

            
            command.CommandText = @"INSERT INTO [MG_UsersInformation]
                                (UserName,userPrincipalName,surname,preferredLanguage,officeLocation,mobilePhone,mail,jobTitle,givenName,DisplayName) 
                                VALUES(@DisplayName,@UserPrincipalName,@Surname,@PreferredLanguage,@OfficeLocation,@MobilePhone,@Mail,@JobTitle,@GivenName,@DisplayName);"
            ;
            command.CommandText = $@"INSERT INTO [MG_UsersInformation] ({COLUMNS}) VALUES ({VALUES})";

            command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            command.Parameters.AddWithValue("@UserPrincipalName", user.UserPrincipalName);
            command.Parameters.AddWithValue("@Surname", user.Surname);
            command.Parameters.AddWithValue("@PreferredLanguage", user.PreferredLanguage);
            command.Parameters.AddWithValue("@OfficeLocation", user.OfficeLocation);
            command.Parameters.AddWithValue("@MobilePhone", user.MobilePhone);
            command.Parameters.AddWithValue("@Mail", user.Mail);
            command.Parameters.AddWithValue("@JobTitle", user.JobTitle);
            command.Parameters.AddWithValue("@GivenName", user.GivenName);
            int rowsChanged= command.ExecuteNonQuery();
            connection.Close();
            return rowsChanged;
        }//InsertUserIntoDatabase

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
