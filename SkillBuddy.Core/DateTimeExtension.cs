using Newtonsoft.Json;
using System;
using System.Reflection;

namespace SkillBuddy.Core
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Converts Unix epoch seconds (since 1970-01-01) to UTC DateTime.
        /// </summary>
        public static DateTime ToDateTime(this long epoch)
        {
            return DateTime.UnixEpoch.AddSeconds(epoch).ToUniversalTime();
        }

        /// <summary>
        /// Converts a DateTime to Unix epoch seconds.
        /// </summary>
        public static double ToUnixTimeSeconds(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Checks if an object is considered "empty".
        /// Handles null, string, array, DateTime, primitives, and class objects recursively.
        /// </summary>
        public static bool IsEmpty(this object? obj)
        {
            if (obj == null)
                return true;

            var type = obj.GetType();

            // String
            if (type == typeof(string))
                return string.IsNullOrEmpty((string)obj);

            // Array
            if (type.IsArray)
                return ((Array)obj).Length == 0;

            // DateTime
            if (type == typeof(DateTime))
                return (DateTime)obj == default;

            // Value types (int, long, etc.)
            if (type.IsValueType)
                return obj.Equals(Activator.CreateInstance(type));

            // Complex class → check properties
            if (type.IsClass)
            {
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = prop.GetValue(obj);
                    if (value != null && !value.IsEmpty())
                        return false;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs a deep copy of an object using JSON serialization.
        /// Note: Private members are not cloned.
        /// </summary>
        public static T? Clone<T>(this T source)
        {
            if (source == null) return default;

            var settings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            return JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(source),
                settings
            ) ?? default;
        }
    }
}
