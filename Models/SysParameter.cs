using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisterTeamsUsersParser.Models
{
    public class SysParameter
    {
        public int AppID { get; set; }

        public int UserID { get; set; }

        public string ParamName { get; set; }

        public string ParamValue { get; set; }

        public bool ParamValueIsEncrypted { get; set; }

        public string Description { get; set; }
    }
}
