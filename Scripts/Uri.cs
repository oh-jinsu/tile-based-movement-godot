namespace Game
{
    public struct Uri
    {
        public string host;
        public string endpoint;

        public Uri(string host, string endpoint)
        {
            this.host = host;

            this.endpoint = endpoint;
        }
    }
}
