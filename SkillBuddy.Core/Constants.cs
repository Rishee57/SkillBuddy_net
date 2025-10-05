using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SkillBuddy.Core
{
    public sealed class Constants
    {
        static IConfiguration Configuration;

        public static int MAX_RETRY { get; private set; }
        public static int RETRY_INTERVAL { get; private set; }

        public static readonly string[] DATE_FORMATS = { 
            // Basic formats
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmssZ",
            // Extended formats
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ssZ",
            // All of the above with reduced accuracy
            "yyyyMMddTHHmmzzz",
            "yyyyMMddTHHmmzz",
            "yyyyMMddTHHmmZ",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy-MM-ddTHH:mmzz",
            "yyyy-MM-ddTHH:mmZ",
            // Accuracy reduced to hours
            "yyyyMMddTHHzzz",
            "yyyyMMddTHHzz",
            "yyyyMMddTHHZ",
            "yyyy-MM-ddTHHzzz",
            "yyyy-MM-ddTHHzz",
            "yyyy-MM-ddTHHZ",
            // localTime
            "yyyy-MM-ddTHH:mm:ssK"
        };

        public static Regex UrlRegex { get; private set; }
        public static Regex InvalidFilenameRegex { get; private set; }

        public static IConfigurationBuilder GetConfigurationBuilder()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("providers.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("chatbotsettings.json", optional: true, reloadOnChange: true);
        }

        static Constants()
        {
            var configurationBuilder = GetConfigurationBuilder().AddEnvironmentVariables();


            Int32 int32Value;

            Configuration = configurationBuilder.Build();
            if (!String.IsNullOrEmpty(Configuration["Constants:MaxRetry"]) && Int32.TryParse(Configuration["Constants:MaxRetry"], out int32Value))
                MAX_RETRY = int32Value;

            if (!String.IsNullOrEmpty(Configuration["Constants:MaxRetry"]) && Int32.TryParse(Configuration["Constants:RetryInterval"], out int32Value))
                RETRY_INTERVAL = int32Value;

            if (!String.IsNullOrEmpty(Configuration["Constants:UrlRegex"]))
                UrlRegex = new Regex(Configuration["Constants:UrlRegex"], RegexOptions.Compiled);
            else
                UrlRegex = new Regex(@"^((https?|s?ftp):\/\/)[\w/\-?=%.]+\.[\w/\-&?=%.+,;]+$", RegexOptions.Compiled);



            if (!String.IsNullOrEmpty(Configuration["Constants:InvalidFilenameRegex"]))
                InvalidFilenameRegex = new Regex(Configuration["Constants:InvalidFilenameRegex"], RegexOptions.Compiled);
            else
                InvalidFilenameRegex = new Regex(@"[///\/%/?/*/:/|\""\<\>]", RegexOptions.Compiled);
        }
    }
}
