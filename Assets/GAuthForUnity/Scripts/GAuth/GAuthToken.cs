using System.Collections.Generic;

namespace GAuthForUnity
{
    public class GAuthToken
    {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public GAuthToken(Dictionary<string, string> dic)
        {
            AccessToken = dic["accessToken"];
            RefreshToken = dic["refreshToken"];
        }
    }
}