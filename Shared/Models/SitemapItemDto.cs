using System.Text.Json.Serialization;

namespace GuiderBlazor.Shared.Models
{
    public class SitemapItemDto
    {
        // Сюда будет приходить либо "place/slug", либо "?Category=..."
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        // Дата в формате "yyyy-MM-dd"
        [JsonPropertyName("lastMod")]
        public string LastMod { get; set; } = string.Empty;
    }
}
