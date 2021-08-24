using System.Text.Json.Serialization;

namespace LibreCards.Core
{
    internal class JsonFileStructure
    {
        [JsonPropertyName("templates")]
        public string[] Templates { get; set; }
        [JsonPropertyName("responses")]
        public string[] Responses { get; set; }
    }
}
