namespace Game
{
    public enum Scene
    {
        Splash,
        Creation,
        Game,
    }

    public class GameSceneArguments
    {
        public Network.Incoming.Hello hello;
    }
}
