namespace Game
{
    public static class Constant
    {
        private const string CLOUDFRONT_URI = "https://cdn.eastonline.kr";

        private const string API_URI = "https://api.eastonline.kr";

        public static Uri CloudfrontUri(string endpoint)
        {
            if (endpoint.StartsWith("/"))
            {
                return new Uri(CLOUDFRONT_URI, endpoint);
            }

            return new Uri(CLOUDFRONT_URI, "/" + endpoint);
        }

        public static Uri ApiUri(string endpoint)
        {
            if (endpoint.StartsWith("/dev/"))
            {
                return new Uri(API_URI, endpoint);
            }

            return new Uri(API_URI, "/dev/" + endpoint);
        }
    }
}
