using AllegroREST.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllegroREST
{
    public class AllegroClient
    {
        private HttpClient _client { get; }
        private readonly string clientId = "c07b8c4498ca4598942f9b2477adf6f0";
        private readonly string secretId = "S1VjLES6FQMiSOelofq21ZAMRwg7qQlTdT0L17Otz6E7bbt1NxCFZDOjvbJ7CrOd";
        private readonly Uri AUTH_LINK = new Uri("https://allegro.pl/auth/oauth/device");
        private Token Token { set; get; }

        public AllegroClient(HttpClient client)
        {
            _client = client;
        }

        public async Task Authorize()
        {
            Token = Utility.DeserializeToken;
            Console.WriteLine("UZYKALEM TOKEN: " + Token.AccessToken);

            if (Token is null)
            {
                var authData = await getAuthData();
                Utility.OpenUrl(authData.VerificationUriComplete);
                Token = await askServerForToken(clientId, secretId, authData);
                Console.WriteLine("UZYKALEM TOKEN: " + Token.AccessToken);
                Utility.SerializeToken(Token);
            }
        }

        private async Task<Token> askServerForToken(string clientId, string secretId, AuthorizationData authData)
        {
            return await Task.Run(async () =>
            {
                Token token = null;
                Thread.Sleep(1000 * 15);
                var rtdf = "https://allegro.pl/auth/oauth/token?grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Adevice_code&device_code=";
                var url = new Uri(rtdf + authData.DeviceCode);
                var oAuth = getAuthParameters(clientId, secretId);
                while (true)
                {
                    var res = await sendRequest(url, oAuth, "application/json", "");

                    if (res.ResultOk)
                    {
                        token = Utility.Deserialize<Token>(res.Stream);
                        break;
                    }
                    Thread.Sleep(1000 * authData.Interval);
                }
                return token;
            });
        }


        private async Task<AuthorizationData> getAuthData()
        {
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", clientId }
            });

            _client.BaseAddress = AUTH_LINK;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", getAuthParameters(clientId, secretId));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsync(AUTH_LINK, formContent);
            var contents = await response.Content.ReadAsStreamAsync();
            var authData = Utility.Deserialize<AuthorizationData>(contents);

            return authData;

        }

        private async Task<Response> sendRequest(Uri url, string authHeader, string allegroHeader, string data)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", authHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(data, Encoding.UTF8, allegroHeader);
            var res = await _client.PostAsync(url, content);

            var response = Response.Initalize(res.StatusCode, res.IsSuccessStatusCode, res.Content.ReadAsStreamAsync().Result);
            return response;
        }

        private string getAuthParameters(string clientId, string secretId)
        {
            string tuple = clientId + ":" + secretId;
            byte[] bytes = Encoding.UTF8.GetBytes(tuple);
            return "Basic " + Convert.ToBase64String(bytes);
        }
    }
}
