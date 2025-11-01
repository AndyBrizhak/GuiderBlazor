
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Utils
{
    public class NumberToStringConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Проверяем, какой тип токена пришел
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    // Если пришла строка (включая "")
                    return reader.GetString();

                case JsonTokenType.Number:
                    // Если пришло число, читаем его и преобразуем в строку
                    // Используем TryGet... для безопасности
                    if (reader.TryGetInt64(out long longValue))
                    {
                        return longValue.ToString();
                    }
                    if (reader.TryGetDouble(out double doubleValue))
                    {
                        return doubleValue.ToString();
                    }
                    // Если по какой-то причине не удалось прочитать
                    return null;

                case JsonTokenType.Null:
                    // Если пришел null
                    return null;

                default:
                    // Для других типов, генерируем исключение или возвращаем null/empty
                    throw new JsonException($"Unexpected token type {reader.TokenType} when trying to convert to string.");
            }
        }
        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            //if (long.TryParse(value, out long number))
            //{
            //    writer.WriteNumberValue(number);
            //}
            //else
            //{
            //    writer.WriteStringValue(value);
            //}

            // Для сериализации обратно, просто записываем строку.
            writer.WriteStringValue(value);
        }
    }
}
