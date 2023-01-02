namespace Game.Model
{
    public struct Exception
    {
        public string message;
    }

    public struct SaveAuthRequest
    {
        public string username;
    }

    public struct SaveAuthResponse
    {
        public string token;
    }
}
