
using MisterControlHubApiDto.RequestDtos;
using MisterTeamsUsersParserParser.Models.Dtos;
using RestSharp;
using RtelLibrary.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace MisterTeamsUsersParserParser.Helpers
{
    public class JWTTokens
    {
        private static JwtSecurityToken misterControlHubApiToken;

        private static JwtSecurityToken GetToken(string URL)
        {
            var client = new RestClient($"{URL}api/Authentication/Authenticate")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            LoginDto login = new()
            {
                LoginTypeID = 1,
                SysApplicationsID = (int)SysApplications.MisterControlHub,
                UserName = Program.RtelApiUserName,
                Password = Program.RtelAPiPassword
            };

            request.AddParameter("application/json", JsonSerializer.Serialize(login), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JwtSecurityToken responceToken;
            if (response.IsSuccessful)
            {
                JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

                responceToken = jwtSecurityTokenHandler.ReadJwtToken(JsonSerializer.Deserialize<AuthTokenResponseDTO>(response.Content, Program.JsonSerializerOptions)?.Token);
                if (responceToken is null || DateTime.Compare(DateTime.Now, responceToken.ValidTo.AddDays(1)) >= 0)
                    throw new Exception("Token is null or invalid");
            }
            else throw new Exception($"Responce from: {URL}api/Authentication/Authenticate is not Successful.\n\rBody:\\n\r{JsonSerializer.Serialize(login)}\n\rResponse:\n\r{response.Content}");

            return responceToken;
        }

        public static JwtSecurityToken GetToken(SysApplications sysApplication)
        {

            switch (sysApplication)
            {
                case SysApplications.MisterTeamsUsersParser:
                case SysApplications.MisterControlHub:
                    if (misterControlHubApiToken is null || DateTime.Compare(DateTime.Now, misterControlHubApiToken.ValidTo.AddDays(1)) >= 0)
                        misterControlHubApiToken = GetToken(Program.MisterControlHubURL);
                    return misterControlHubApiToken;
                default: throw new Exception("Invalid Application");
            }
        }


    }
}
