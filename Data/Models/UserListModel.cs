using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisterTeamsUsersParser.Data.Models
{
    internal class UserListModel
    {
        [Description("@odata.context")]
        string ODataContext { get; set; }
        [Description("value")]
        List<UserModel> value { get; set; }
    }
}
