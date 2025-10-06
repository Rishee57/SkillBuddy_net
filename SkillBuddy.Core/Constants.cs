using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SkillBuddy.Core
{
    /// <summary>
    /// Provides centralized configuration constants and regex utilities.
    /// </summary>
    public sealed class Constants
    {
        private static readonly IConfiguration? Configuration;

        /// <summary>
        /// Maximum number of retries for operations (from config).
        /// </summary>
        public static int MaxRetry { get; private set; }

        /// <summary>
        /// Retry interval in milliseconds (from config).
        /// </summary>
        public static int RetryInterval { get; private set; }

        /// <summary>
        /// Common ISO date/time formats used for parsing timestamps.
        /// </summary>
        public static readonly string[] DateFormats = {
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmssZ",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyyMMddTHHmmzzz",
            "yyyyMMddTHHmmzz",
            "yyyyMMddTHHmmZ",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy-MM-ddTHH:mmzz",
            "yyyy-MM-ddTHH:mmZ",
            "yyyyMMddTHHzzz",
            "yyyyMMddTHHzz",
            "yyyyMMddTHHZ",
            "yyyy-MM-ddTHHzzz",
            "yyyy-MM-ddTHHzz",
            "yyyy-MM-ddTHHZ",
            "yyyy-MM-ddTHH:mm:ssK"
        };

        /// <summary>
        /// Regex to validate URLs.
        /// </summary>
        public static Regex UrlRegex { get; private set; }

        /// <summary>
        /// Regex to validate filenames.
        /// </summary>
        public static Regex InvalidFilenameRegex { get; private set; }

        static Constants()
        {
            try
            {
                var configurationBuilder = GetConfigurationBuilder().AddEnvironmentVariables();
                Configuration = configurationBuilder.Build();

                if (int.TryParse(Configuration["Constants:MaxRetry"], out int maxRetry))
                    MaxRetry = maxRetry;
                else
                    MaxRetry = 3;

                if (int.TryParse(Configuration["Constants:RetryInterval"], out int retryInterval))
                    RetryInterval = retryInterval;
                else
                    RetryInterval = 2000;

                var invalidUrlRegexPattern = Configuration["Constants:UrlRegex"];
                UrlRegex = !string.IsNullOrEmpty(invalidUrlRegexPattern)
                    ? new Regex(invalidUrlRegexPattern, RegexOptions.Compiled)
                    : new Regex(@"^((https?|s?ftp):\/\/)[\w/\-?=%.]+\.[\w/\-&?=%.+,;]+$", RegexOptions.Compiled);

                var invalidPattern = Configuration["Constants:InvalidFilenameRegex"];
                InvalidFilenameRegex = !string.IsNullOrEmpty(invalidPattern)
                    ? new Regex(invalidPattern, RegexOptions.Compiled)
                    : new Regex(@"[\\/:%?*|""<>]", RegexOptions.Compiled);

            }
            catch (Exception ex)
            {
                // fallback defaults if configuration fails
                MaxRetry = 3;
                RetryInterval = 2000;
                UrlRegex = new Regex(@"^((https?|s?ftp):\/\/)[\w/\-?=%.]+\.[\w/\-&?=%.+,;]+$", RegexOptions.Compiled);
                InvalidFilenameRegex = new Regex(@"[\\/:%?*|""<>]", RegexOptions.Compiled);

                Console.WriteLine($"[Constants] Initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds a configuration builder with JSON configuration sources.
        /// </summary>
        public static IConfigurationBuilder GetConfigurationBuilder()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? Directory.GetCurrentDirectory();

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("providers.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("chatbotsettings.json", optional: true, reloadOnChange: true);
        }
    }
}
