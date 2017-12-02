using Newtonsoft.Json;

namespace Telegram.Bot.Types
{
    /// <summary>
    /// Contains information about why a request was unsuccessful.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ResponseParameters
    {
        /// <summary>
        /// The group has been migrated to a supergroup with the specified identifier.
        /// </summary>
        [JsonProperty]
        public long MigrateToChatId { get; set; }

        /// <summary>
        /// In case of exceeding flood control, the number of seconds left to wait before the request can be repeated.
        /// </summary>
        [JsonProperty]
        public int RetryAfter { get; set; }
    }
}