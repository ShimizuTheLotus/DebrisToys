using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace DebrisToys.Global.Helper
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options;

        static JsonHelper()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                TypeInfoResolver = AppJsonContext.Default
            };
        }

        public static string Serialize<T>(T value, JsonTypeInfo<T> typeInfo)
        {
            return JsonSerializer.Serialize(value, typeInfo);
        }

        public static T? Deserialize<T>(string? json, JsonTypeInfo<T> typeInfo)
        {
            if (string.IsNullOrEmpty(json))
                return default;
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = ShimizuToolkit.HotkeyWinUI.Helper.AppJsonContext.Default
            };
            return JsonSerializer.Deserialize(json, typeInfo);
        }
    }
}
