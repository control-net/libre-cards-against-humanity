using System.Text.Json.Serialization;

namespace LibreCards.Core
{
    internal class JsonFileStructure
    {
        [JsonPropertyName("templates")]
        public string[] Templates { get; set; }

        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
    }
}
