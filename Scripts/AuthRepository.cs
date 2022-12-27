using Godot;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Game
{
    public interface IAuthRepository
    {
        string GetAccessToken();

        void SaveAccessToken(string accessToken);
    }

    public class AuthRepository : IAuthRepository
    {
        const string PATH = "user://auth.yml";

        private readonly File file = new File();

        private readonly ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private readonly IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private AuthModel authModel;

        public AuthRepository()
        {
            if (!file.FileExists(PATH))
            {
                CreateFile();
            }

            authModel = ReadFile();
        }

        public string GetAccessToken()
        {
            return authModel.accessToken;
        }

        public void SaveAccessToken(string accessToken)
        {
            authModel.accessToken = accessToken;

            WriteFile(authModel);
        }

        private void CreateFile()
        {
            WriteFile(new AuthModel { });
        }

        private AuthModel ReadFile()
        {
            OpenFile(File.ModeFlags.Read);

            var yaml = file.GetAsText();

            var model = deserializer.Deserialize<AuthModel>(yaml);

            CloseFile();

            return model;
        }

        private void WriteFile(AuthModel model)
        {
            OpenFile(File.ModeFlags.Write);

            var yaml = serializer.Serialize(model);

            file.StoreString(yaml);

            CloseFile();
        }

        private void OpenFile(File.ModeFlags flag)
        {
            var error = file.Open(PATH, flag);

            if (error != Error.Ok)
            {
                throw new System.Exception("Could not open the file.");
            }
        }

        private void CloseFile()
        {
            file.Close();
        }
    }
}
