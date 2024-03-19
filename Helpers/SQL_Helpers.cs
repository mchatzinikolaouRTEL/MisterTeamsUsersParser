using Dapper;
using MisterTeamsUsersParser.Models;
using RtelEncryptionLibrary;
using RtelLibrary.TableModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MisterTeamsUsersParserParser.Helpers.Enums;

namespace MisterTeamsUsersParserParser.Helpers
{
    internal class SQL_Helpers
    {
        public static readonly int CommandTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeOut"]);
        public static int GetCommandTimeout(SqlConnection connection)
        {
            int retv = CommandTimeOut;
            try
            {
                string result = null;

                SqlCommand GetCommand = new SqlCommand(" SELECT ISNULL([ParamValue],'" + CommandTimeOut + "') FROM [SysParameters] "
                                                     + " WHERE ([AppID] = " + (int)Applications.AppID + ") AND ([UserID] = " + (int)ApplicationUsers.SystemAdministratorID + ") AND (LTRIM(RTRIM(LOWER([ParamName]))) = LTRIM(RTRIM(LOWER('CommandTimeOut')))) ", connection);
                GetCommand.CommandTimeout = CommandTimeOut;
                result = Convert.ToString(GetCommand.ExecuteScalar());

                if (!String.IsNullOrEmpty(result))
                {
                    retv = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Exception LogException = new Exception(new StackTrace(ex, true).GetFrame(0).GetMethod().Name + "\t" + ex.Message, ex.InnerException);
                //log
                throw LogException;
            }
            return retv;
        }//GetCommandTimeout

        private static SqlConnection SysParamsConnection;

        public static IEnumerable<SysParameter> GetSysParameters(int? AppID = null)
        {
            SysParamsConnection = new SqlConnection();
            SysParamsConnection.ConnectionString = Program.ConnectionString;
            SysParamsConnection.Open();
            IEnumerable<SysParameter> retIenum = null;

            try
            {
                string whereStatement = string.Empty;
                if (AppID != null)
                {
                    whereStatement = " AND (AppID = @AppID) ";
                }

                object Parameters = new { AppID };
                retIenum = SysParamsConnection.Query<SysParameter>($"SELECT * FROM SysParameters WHERE (1 = 1) {whereStatement}", Parameters, commandTimeout: CommandTimeOut, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                SysParamsConnection.Close();
            }

            return retIenum;
        }//GetSysParameters
    }
}
