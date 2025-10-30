using System.Text.Json.Serialization;

namespace GuiderBlazor.Client.Models
{
    public class Place
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("img_link")]
        public List<string> ImgLink { get; set; } = new();

        [JsonPropertyName("address")]
        public Address Address { get; set; } = new();

        [JsonPropertyName("phone")]
        public Phone Phone { get; set; } = new();

        [JsonPropertyName("social_network")]
        public SocialNetwork SocialNetwork { get; set; } = new();

        [JsonPropertyName("web")]
        public string Web { get; set; } = string.Empty;

        [JsonPropertyName("preview_link")]
        public string PreviewLink { get; set; } = string.Empty;

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new();

        [JsonPropertyName("owner")]
        public Owner? Owner { get; set; }

        [JsonPropertyName("location")]
        public Location? Location { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("province")]
        public string Province { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
    }

    public class Phone
    {
        [JsonPropertyName("callable")]
        public long Callable { get; set; }

        [JsonPropertyName("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty;
    }

    public class SocialNetwork
    {
        [JsonPropertyName("facebook")]
        public string Facebook { get; set; } = string.Empty;

        [JsonPropertyName("instagram")]
        public string Instagram { get; set; } = string.Empty;
    }

    public class Owner
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
    }

    public class Location
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new();
    }

    public class PlacesResponse
    {
        [JsonPropertyName("data")]
        public List<Place> Data { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("perPage")]
        public int PerPage { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }

    public class PlaceFilterParams
    {
        public string? SearchTerm { get; set; }
        public List<string>? Tags { get; set; }
        public string TagsMode { get; set; } = "any";
        public string? Category { get; set; }
        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 12;
        public string SortField { get; set; } = "name";
        public string SortOrder { get; set; } = "ASC";
    }
}
