using RtelLibrary;
using RtelLibrary.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MisterTeamsUsersParserParser.Models.Dtos
{
    public record AuthTokenRequestDTO
    {
        [JsonIgnore]
        public LoginType LoginType
        {
            get { return RtelEnumsConverters.TryParseEnum(LoginTypeID, LoginType.Unknown); }
        }
        [JsonIgnore]
        public SysApplications SysApplication
        {
            get { return RtelEnumsConverters.TryParseEnum(SysApplicationsID, SysApplications.Unknown); }
        }
        [Required]
        public int LoginTypeID { get; set; }
        [Required]
        public int SysApplicationsID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
