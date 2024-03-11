using Dapper;
using Microsoft.IdentityModel.Tokens;
using MisterTeamsUsersParserParser.Helpers;
using MisterTeamsUsersParserParser.Models;
using MisterTeamsUsersParserParser.Models.Dtos;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
        public enum Applications
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
            Connection=new SqlConnection(Program.ConnectionString);
            Connection.Open();
            UnpackSysParameters(parameters);

            Connection.Close();
        }//TeamsParsingFunctionality

        void UnpackSysParameters(Parameters parameters)
        {
            //Unbox.
            List<SysParameters> sysParameters = parameters.SysParameters.ToList();
            foreach(SysParameters sysParameter in sysParameters)
            {
                sysParameter.ParamName = sysParameter.ParamName.ToLower();
            }
            /*
            //Single values.
            DataTablesRetentionPeriod = Convert.ToInt32(sysParameters.Where(x => x.ParamName == "DataTablesRetentionPeriod".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            DataTablesRetentionPeriodMode = Convert.ToInt32(sysParameters.Where(x => x.ParamName == "DataTablesRetentionPeriodMode".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyDataTableCreateMetaDataAction = (DataTableCreateMetaDataAction)Convert.ToInt32(sysParameters.Where(x => x.ParamName == "MonthlyDataTableCreateMetaDataAction".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyDataTableMoveAction = (DataTableMoveAction)Convert.ToInt32(sysParameters.Where(x => x.ParamName == "MonthlyDataTableMoveAction".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyDataTablesCreateMetadataProcedure = Convert.ToString(sysParameters.Where(x => x.ParamName == "MonthlyDataTablesCreateMetadataProcedure".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyDataTablesMoveProcedure = Convert.ToString(sysParameters.Where(x => x.ParamName == "MonthlyDataTablesMoveProcedure".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyDataTablesRetentionPeriod = Convert.ToInt32(sysParameters.Where(x => x.ParamName == "MonthlyDataTablesRetentionPeriod".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyFoldersDestinationPath = Convert.ToString(sysParameters.Where(x => x.ParamName == "MonthlyFoldersDestinationPath".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyFoldersParseExactPattern = Convert.ToString(sysParameters.Where(x => x.ParamName == "MonthlyFoldersParseExactPattern".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            MonthlyFoldersRetentionPeriod = Convert.ToInt32(sysParameters.Where(x => x.ParamName == "MonthlyFoldersRetentionPeriod".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ProcedureToCreateCallsRecorded = Convert.ToString(sysParameters.Where(x => x.ParamName == "ProcedureToCreateCallsRecorded".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ProcedureToCreateNonClosedNormalSessions = Convert.ToString(sysParameters.Where(x => x.ParamName == "ProcedureToCreateNonClosedNormalSessions".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ProcedureToCreateParticipants = Convert.ToString(sysParameters.Where(x => x.ParamName == "ProcedureToCreateParticipants".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ProcedureToCreateSessions = Convert.ToString(sysParameters.Where(x => x.ParamName == "ProcedureToCreateSessions".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            ProcedureToCreateTracks = Convert.ToString(sysParameters.Where(x => x.ParamName == "ProcedureToCreateTracks".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            StorageDataConnectionString = Convert.ToString(sysParameters.Where(x => x.ParamName == "StorageDataConnectionString".ToLower()).Select(x => x.ParamValue).FirstOrDefault());
            StorageDataName = Convert.ToString(sysParameters.Where(x => x.ParamName == "StorageDataName".ToLower()).Select(x => x.ParamValue).FirstOrDefault());

            //Lists.
            
            
            DataTablesRetentionPeriodPerTable = new();
            foreach (var sysParameter in sysParameters.FindAll(x => x.ParamName.Contains("DataTablesRetentionPeriodPerTable".ToLower())))
            {
                DataTablesRetentionPeriodPerTable.Add(Convert.ToInt32(sysParameter.ParamValue));
            }
            */
        }//UnpackSysParameters
        
    }
}
