// Указываем namespace, соответствующий новой папке
namespace GuiderBlazor.Shared.Models.JsonLd
{
    using System.Text.Json.Serialization;
    using System.Collections.Generic;

    // Базовый класс для @context и @type
    public class JsonLdBase
    {
        [JsonPropertyName("@context")]
        public string Context { get; set; } = "https://schema.org";

        [JsonPropertyName("@type")]
        public string Type { get; set; }
    }

    // Класс для LocalBusiness
    public class LocalBusinessLd : JsonLdBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [JsonPropertyName("image")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Image { get; set; } 

        [JsonPropertyName("address")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PostalAddressLd? Address { get; set; }

        [JsonPropertyName("geo")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeoCoordinatesLd? Geo { get; set; }

        [JsonPropertyName("telephone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Telephone { get; set; }

        [JsonPropertyName("openingHoursSpecification")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OpeningHoursSpecificationLd>? OpeningHoursSpecification { get; set; }

        [JsonPropertyName("sameAs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? SameAs { get; set; }
    }

    public class PostalAddressLd
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; } = "PostalAddress";

        [JsonPropertyName("streetAddress")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StreetAddress { get; set; }

        [JsonPropertyName("addressLocality")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AddressLocality { get; set; }

        [JsonPropertyName("addressRegion")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AddressRegion { get; set; }

        [JsonPropertyName("addressCountry")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AddressCountry { get; set; }
    }

    public class GeoCoordinatesLd
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; } = "GeoCoordinates";

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; }
    }

    public class OpeningHoursSpecificationLd
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; } = "OpeningHoursSpecification";

        [JsonPropertyName("dayOfWeek")]
        public List<string> DayOfWeek { get; set; } = new();

        [JsonPropertyName("opens")]
        public string Opens { get; set; }

        [JsonPropertyName("closes")]
        public string Closes { get; set; }
    }

    // Классы для Breadcrumbs
    public class BreadcrumbListLd : JsonLdBase
    {
        [JsonPropertyName("itemListElement")]
        public List<ListItemLd> ItemListElement { get; set; } = new();
    }

    public class ListItemLd
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; } = "ListItem";

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("item")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Item { get; set; } // У последнего элемента нет 'item'
    }
}
