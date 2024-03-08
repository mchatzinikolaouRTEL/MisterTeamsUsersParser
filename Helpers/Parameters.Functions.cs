using RestSharp;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MisterProtypoParser.Helpers
{
    public partial record Parameters
    {
        //TODO: we don't check if responce IsSuccessful 
        public static void GetParameters(
            SysApplicationProcess sysApplicationProcess, 
            ref Dictionary<SysApplicationProcess, List<SysParameters>> processParameters, 
            ref Dictionary<SysApplicationProcess, List<SysParametersDetails>> processParameterDetails)
        {
            string SysApplicationsURL;
            switch (Program.sysApplication)
            {
                case SysApplications.LDAPParser:
                case SysApplications.MisterControlHub:
                    SysApplicationsURL = Program.MisterControlHubURL;
                    break;
                default: throw new Exception("Unknown Application");
            }

            //Get application Parameters
            var client = new RestClient($"{SysApplicationsURL}api/Parameter/application/{Program.sysApplication}")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            IRestResponse response = client.Execute(request);
            List<SysParameters> parentPrameters = JsonSerializer.Deserialize<List<SysParameters>>(response.Content, Program.JsonSerializerOptions);

            //Get application process Parameters 
            client = new RestClient($"{SysApplicationsURL}api/Parameter/applicationprocess/{sysApplicationProcess}")
            {
                Timeout = -1
            };

            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            response = client.Execute(request);
            processParameters[sysApplicationProcess] = JsonSerializer.Deserialize<List<SysParameters>>(response.Content, Program.JsonSerializerOptions);
            processParameters[sysApplicationProcess].AddRange(parentPrameters);

            //Get application Parameters details
            client = new RestClient($"{SysApplicationsURL}api/Parameter/application/{Program.sysApplication}/Details")
            {
                Timeout = -1
            };
            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            response = client.Execute(request);
            List<SysParametersDetails> parentPrametersDetails = JsonSerializer.Deserialize<List<SysParametersDetails>>(response.Content, Program.JsonSerializerOptions);

            //Get application process Parameters Details
            client = new RestClient($"{SysApplicationsURL}api/Parameter/applicationprocess/{sysApplicationProcess}/Details")
            {
                Timeout = -1
            };

            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {JWTTokens.GetToken(Program.sysApplication).RawData}");
            response = client.Execute(request);
            processParameterDetails[sysApplicationProcess] = JsonSerializer.Deserialize<List<SysParametersDetails>>(response.Content, Program.JsonSerializerOptions);
            processParameterDetails[sysApplicationProcess].AddRange(parentPrametersDetails);
        }
    }
}
