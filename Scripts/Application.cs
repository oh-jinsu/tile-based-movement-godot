namespace Game
{
    public class Application : Singleton
    {
        public const string NODE_PATH = "/root/Application";

        public AuthRepository AuthRepository { get; private set; }

        public Application()
        {
            AuthRepository = new AuthRepository();
        }
    }
}
