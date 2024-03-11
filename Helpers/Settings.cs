using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisterTeamsUsersParserParser.Helpers
{
    public sealed class Settings
    {
        public bool EnableLogs { get; set; }
        public int CommandTimeOut { get; set; }
        public int DefaultWarnigMailDelay { get; set; }
        public string DefaultCulture { get; set; }
        public string DefaultNumericCulture { get; set; }
        public string ConnectionString { get; set; }

        //maybe add MisterControlHubURL.
    }
}
