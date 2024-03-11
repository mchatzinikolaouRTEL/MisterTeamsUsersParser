using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MisterTeamsUsersParser.Data.Models
{
    internal class UserModel
    {
        [Description("businessPhones")]
        public string[] BusinessPhones { get; set; }
        [Description("displayName")]
        public string DisplayName { get; set; }
        [Description("givenName")]
        public string GivenName { get; set; }
        [Description("jobTitle")]
        public string JobTitle { get; set; }
        [Description("mail")]
        public string Mail { get; set; }
        [Description("mobilePhone")]
        public string MobilePhone { get; set; }
        [Description("officeLocation")]
        public string OfficeLocation { get; set; }
        [Description("preferredLanguage")]
        public string PreferredLanguage { get; set; }
        [Description("surname")]
        public string Surname { get; set; }
        [Description("userPrincipalName")]
        public string UserPrincipalName { get; set; }
        [Description("id")]
        public string Id { get; set; }
    }
}
