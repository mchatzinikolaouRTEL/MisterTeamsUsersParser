using System.ComponentModel;

namespace MisterTeamsUsersParserParser.Helpers
{
    class Enums
    {
        public enum ApplicationUsers
        {
            [Description("System Administrator ID")]
            SystemAdministratorID = 0,
        }//ApplicationUsers
        public enum Applications
        {
            //TODO: Fix app ids
            [Description("Unknown")]
            Unknown = 0,
            [Description("AppID")]
            AppID = 10805,
            [Description("AppID")]
            MisterControlHubApi = 100000,
            [Description("Active Directory Controller")]
            ActiveDirectoryController = 100010,
            [Description("Authentication Controller")]
            AuthenticationController = 100020,
            [Description("Jira Controller")]
            JiraController = 100030,
            [Description("Intermediate Controller")]
            IntermediateController = 100040,
        }//Applications
    }
}
