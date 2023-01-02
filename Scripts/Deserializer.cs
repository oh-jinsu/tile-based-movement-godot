using Godot;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Newtonsoft.Json;

namespace Game
{
    public static class Deserializer
    {
        public delegate void OnSuccess<T>(T model);

        public delegate void OnFailure(Model.Exception exception);

        private static readonly IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static void PrintMessage(string message)
        {
            GD.Print(message);
        }

        public static HttpClient.OnResponse FromYaml<T, U>(OnSuccess<T> onSuccess, OnFailure onFailure = null)
        {
            return (ok, result) =>
            {
                if (!ok)
                {
                    if (onFailure == null)
                    {
                        PrintMessage(result);

                        return;
                    }

                    onFailure(deserializer.Deserialize<Model.Exception>(result));

                    return;
                }

                onSuccess(deserializer.Deserialize<T>(result));
            };
        }

        public static HttpClient.OnResponse FromJson<T>(OnSuccess<T> onSuccess, OnFailure onFailure = null)
        {
            return (ok, result) =>
            {
                if (!ok)
                {
                    if (onFailure == null)
                    {
                        PrintMessage(result);

                        return;
                    }

                    onFailure(JsonConvert.DeserializeObject<Model.Exception>(result));

                    return;
                }

                onSuccess(JsonConvert.DeserializeObject<T>(result));
            };
        }
    }
}
