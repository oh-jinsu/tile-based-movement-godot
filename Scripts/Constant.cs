namespace Game
{
    public static class Constant
    {
        private const string CLOUDFRONT_URI = "https://d217z3neuqnaxg.cloudfront.net";

        public static Uri GetCloudfrontUri(string endpoint)
        {
            if (endpoint.StartsWith("/"))
            {
                return new Uri(CLOUDFRONT_URI, endpoint);
            }

            return new Uri(CLOUDFRONT_URI, "/" + endpoint);
        }
    }
}
