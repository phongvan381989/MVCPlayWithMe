using Newtonsoft.Json;
using System;

namespace MVCPlayWithMe.General
{
    /// <summary>
    /// JsonConverter để convert Unix timestamp (milliseconds từ JavaScript) sang DateTime
    /// Sử dụng: [JsonConverter(typeof(UnixTimestampConverter))]
    /// </summary>
    public class UnixTimestampConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Serialize DateTime sang Unix timestamp (milliseconds)
        /// </summary>
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            long timestamp = new DateTimeOffset(value).ToUnixTimeMilliseconds();
            writer.WriteValue(timestamp);
        }

        /// <summary>
        /// Deserialize Unix timestamp (milliseconds) sang DateTime
        /// </summary>
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return DateTime.MinValue;
            }

            // Handle both long and double (JavaScript numbers can be either)
            long timestamp;
            if (reader.Value is long l)
            {
                timestamp = l;
            }
            else if (reader.Value is double d)
            {
                timestamp = (long)d;
            }
            else
            {
                return existingValue;
            }

            // Convert Unix timestamp (ms) to DateTime
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
        }
    }
}
