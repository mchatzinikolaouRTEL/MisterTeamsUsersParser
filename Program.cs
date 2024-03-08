using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MisterProtypoParser.Helpers;
using RtelConfigurationLibrary;
using RtelEncryptionLibrary;
using RtelLibrary.Enums;
using RtelLogException;
using RtelLogger;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MisterProtypoParser.Helpers.Enums;

namespace MisterProtypoParser
{
    static class Program
    {
        //Instruction: Change sysApplication
        public const SysApplications sysApplication = SysApplications.Unknown;
        public readonly static Logger Logger;
        public readonly static Settings settings;
        public readonly static JWTTokens jwtTokens;
        public readonly static string ConnectionString;
        public readonly static string ClientName;
        public readonly static string RtelApiUserName;
        public readonly static string RtelAPiPassword;
        public readonly static string MisterControlHubURL;
        public readonly static JsonSerializerOptions JsonSerializerOptions = new()
        {
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
        };

        static Program()
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                settings = config.GetRequiredSection("Settings").Get<Settings>();
                Logger = new Logger();
                Logger.EnableLogs = settings.EnableLogs;
                ConnectionString = string.IsNullOrEmpty(settings.ConnectionString) ? RtelConfiguration.GetApplicationConnectionString((int)Applications.AppID) : settings.ConnectionString;
                ClientName = RtelConfiguration.GetApplicationappSetting("ClientName");
                MisterControlHubURL = RtelConfiguration.GetApplicationappSetting("MisterControlHubURL");
                RtelApiUserName = RtelConfiguration.GetApplicationappSetting("RtelApiUserName");
                RtelAPiPassword = RtelConfiguration.GetApplicationappSetting("RtelAPiPassword");
                var x = new RtelEncryption();
                if (x.DecryptString(ref ConnectionString) == RtelEncryption.EncryptionStatus.Error)
                    throw new Exception("Decrypt ConnectionString failed");
                if (x.DecryptString(ref ClientName) == RtelEncryption.EncryptionStatus.Error)
                    throw new Exception("Decrypt ClientName failed");                
                if (x.DecryptString(ref RtelApiUserName) == RtelEncryption.EncryptionStatus.Error)
                    throw new Exception("Decrypt Rtel Api UserName failed");
                if (x.DecryptString(ref RtelAPiPassword) == RtelEncryption.EncryptionStatus.Error)
                    throw new Exception("Decrypt Rtel APi Password failed");

                jwtTokens = new();

                //Map DateTime to DbType.DateTime2
                SqlMapper.AddTypeMap(typeof(DateTime), System.Data.DbType.DateTime2);
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.Id ?? "Activity.Current is null";
                LogException logException = new(ex, new StackTrace(ex, true).GetFrames(), traceId);
                Program.Logger.WriteErrors(logException.ExceptionMessage);
            }
        }
        [STAThread]
        public static async Task Main()
        {
            // Run with wiindows form or service
            var asService = !(Debugger.IsAttached || Environment.UserInteractive);


            if (asService)
            {
                var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Service>();

            });
                builder.UseEnvironment(environment: asService ? Microsoft.Extensions.Hosting.Environments.Production :
                Microsoft.Extensions.Hosting.Environments.Development);

                await builder.RunAsServiceAsync();
            }
            else
            {
                WindowFormThread();
            }
        }

        [STAThread]
        static void WindowFormThread()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
