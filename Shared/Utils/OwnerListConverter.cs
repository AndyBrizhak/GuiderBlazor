using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuiderBlazor.Shared.Models; // Убедитесь, что namespace здесь правильный

namespace Shared.Utils; // Или ваш namespace для конвертеров

public class OwnerListConverter : JsonConverter<List<Owner>?>
{
    public override List<Owner>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            // Если API прислал null
            case JsonTokenType.Null:
                return null;

            // Если API прислал ОДИН ОБЪЕКТ: { "name": "..." }
            case JsonTokenType.StartObject:
                var singleOwner = JsonSerializer.Deserialize<Owner>(ref reader, options);
                if (singleOwner != null)
                {
                    // Заворачиваем один объект в список
                    return new List<Owner> { singleOwner };
                }
                return null;

            // Если API прислал МАССИВ: [ { "name": "..." } ]
            case JsonTokenType.StartArray:
                var list = JsonSerializer.Deserialize<List<Owner>>(ref reader, options);
                return list;

            // Если пришло что-то странное (например, "owner": "")
            case JsonTokenType.String:
                reader.Skip(); // Пропускаем токен
                return null; // Возвращаем null (или new List<Owner>())

            default:
                throw new JsonException($"Unexpected token type {reader.TokenType} for Owner list.");
        }
    }

    public override void Write(Utf8JsonWriter writer, List<Owner>? value, JsonSerializerOptions options)
    {
        // Мы только читаем, писать не обязательно
        JsonSerializer.Serialize(writer, value, options);
    }
}
