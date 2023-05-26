using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace GAuthForUnity.Core
{
    public class GAuthCore : GAuth
    {
        #region Private
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string GAuthServerURL = "https://server.gauth.co.kr/oauth";
        private readonly string ResourceServerURL = "https://open.gauth.co.kr";

        private enum TokenType
        {
            Access,
            Refresh
        }
        #endregion

        #region Async
        public async Task<GAuthToken> GenerateTokenAsync(string email, string password, string clientId, string clientSecret, string redirectUri)
        {
            var code = (await GenerateCodeAsync(email, password)).Code;
            return new GAuthToken(await GetTokenAsync(code, clientId, clientSecret, redirectUri));
        }

        public async Task<GAuthToken> GenerateTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            return new GAuthToken(await GetTokenAsync(code, clientId, clientSecret, redirectUri));
        }

        public async Task<GAuthCode> GenerateCodeAsync(string email, string password)
        {
            var body = new Dictionary<string, string>
            {
                { "email", email },
                { "password", password }
            };
            var response = await SendPostGAuthServerAsync(body, null, "/code");
            var code = response["code"];
            return new GAuthCode(code);
        }

        public async Task<GAuthToken> RefreshAsync(string refreshToken)
        {
            if (!refreshToken.StartsWith("Bearer "))
                refreshToken = "Bearer " + refreshToken;
            return new GAuthToken(await SendPatchGAuthServerAsync(null, refreshToken, "/token", TokenType.Refresh));
        }

        public async Task<GAuthUserInfo> GetUserInfoAsync(string accessToken)
        {
            if (!accessToken.StartsWith("Bearer "))
                accessToken = "Bearer " + accessToken;
            var dic = await SendGetResourceServerAsync(accessToken, "/user");
            return new GAuthUserInfo(dic);
        }

        private async Task<Dictionary<string, string>> GetTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var body = new Dictionary<string, string>
            {
                { "code", code },
                { "clientId", clientId },
                { "clientSecret", clientSecret },
                { "redirectUri", redirectUri }
            };
            return await SendPostGAuthServerAsync(body, null, "/token");
        }

        private async Task<Dictionary<string, string>> SendPostGAuthServerAsync(Dictionary<string, string> body, string token, string url)
        {
            return await SendPostAsync(body, token, GAuthServerURL + url);
        }

        private async Task<Dictionary<string, string>> SendPatchGAuthServerAsync(Dictionary<string, string> body, string token, string url, TokenType tokenType)
        {
            return await SendPatchAsync(body, token, GAuthServerURL + url, tokenType);
        }

        private async Task<Dictionary<string, object>> SendGetResourceServerAsync(string token, string url)
        {
            return await SendGetAsync(token, ResourceServerURL + url);
        }

        private async Task<Dictionary<string, object>> SendGetAsync(string token, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", token);

            var response = await _httpClient.SendAsync(request);
            var statusCode = (int)response.StatusCode;
            if (statusCode != 200)
                throw new GAuthException(statusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
        }

        private async Task<Dictionary<string, string>> SendPatchAsync(Dictionary<string, string> body, string token, string url, TokenType tokenType)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Connection", "keep-alive");

            if (tokenType == TokenType.Access)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            else
                request.Headers.Add("refreshToken", token);

            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var statusCode = (int)response.StatusCode;
            if (statusCode != 200)
                throw new GAuthException(statusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
        }

        private async Task<Dictionary<string, string>> SendPostAsync(Dictionary<string, string> body, string token, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var statusCode = (int)response.StatusCode;
            if (statusCode != 200)
                throw new GAuthException(statusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
        }
        #endregion
    }
}