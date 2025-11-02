using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Utils 
{
    public class DescriptionConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                // Случай 1: API прислал "description": "Обычная строка..."
                case JsonTokenType.String:
                    return reader.GetString();

                // Случай 2: API прислал "description": [ "Строка 1", "Строка 2" ]
                case JsonTokenType.StartArray:
                    var list = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    if (list != null)
                    {
                        // Объединяем массив в одну строку,
                        // оборачивая каждый элемент в <p> тег.
                        // Это идеально подойдет для @((MarkupString)place.Description)
                        return string.Join("", list.Select(s => $"<p>{s}</p>"));
                    }
                    return string.Empty; // Возвращаем пустую строку, если массив пуст

                // Случай 3: API прислал "description": null
                case JsonTokenType.Null:
                    return null;

                default:
                    throw new JsonException($"Unexpected token type {reader.TokenType} for Description.");
            }
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            // Мы только читаем, поэтому просто пишем как строку
            writer.WriteStringValue(value);
        }
    }
}