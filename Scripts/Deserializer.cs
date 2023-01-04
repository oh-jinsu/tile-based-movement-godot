using Godot;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Newtonsoft.Json;

namespace Game
{
    public static class Deserializer
    {
        private static readonly IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public static T FromYaml<T>(string text)
        {
            return deserializer.Deserialize<T>(text);
        }

        public static T FromJson<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
