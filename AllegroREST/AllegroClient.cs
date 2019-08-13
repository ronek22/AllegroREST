using AllegroREST.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AllegroREST
{
    public class AllegroClient
    {
        private HttpClient _client { get; }
        // Dane z utworzonej aplikacji https://apps.developer.allegro.pl/new
        // Aplikacja typu device_code
        private readonly IConfiguration _configuration;
        private readonly string clientId;
        private readonly string secretId;
        private readonly Uri AUTH_LINK = new Uri("https://allegro.pl/auth/oauth/device");
        private readonly Uri API_LINK = new Uri("https://api.allegro.pl/");
        // Token jako prywatna zmienna klasy, latwe dokonowynanie requestow dla zalogowanego uzytkownika
        private Token Token { set; get; }

        public AllegroClient(HttpClient client, IConfigurationRoot configuration)
        {
            _client = client;
            _configuration = configuration;
            clientId = _configuration.GetSection("API")["CLIENT_ID"];
            secretId = _configuration.GetSection("API")["SECRET_ID"];
        }

        public async Task<String> GetOfferDetails(string nrAukcji)

        {

            UriBuilder builder = new UriBuilder("https://api.allegro.pl/sale/offers/" + nrAukcji);
            //var paramValues = HttpUtility.ParseQueryString(builder.Query);

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", Token.AuthorizationHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

            var response = await _client.GetAsync(builder.Uri);
            var contents = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(contents);
            string plu = "";
            int poz = json.ToString().IndexOf("Kod PLU: __");
            if (poz != 0)
            {
                plu = json.ToString().Substring(poz + 11);
                poz = plu.IndexOf("__");
                plu = plu.Substring(0, poz);
            }

            return plu;
        }

        public async Task GetListingByPhrase(string phrase)
        {
            UriBuilder builder = new UriBuilder("https://api.allegro.pl/offers/listing");
            // Dodwania parametrow
            var paramValues = HttpUtility.ParseQueryString(builder.Query);
            paramValues.Add("phrase", phrase);
            paramValues.Add("limit", "20");
            builder.Query = paramValues.ToString();


            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", Token.AuthorizationHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

            var response = await _client.GetAsync(builder.Uri);
            var contents = await response.Content.ReadAsStringAsync();
            // Parsowanie otrzymanego jsona
            var json = JObject.Parse(contents);
            var promotedItems = json.SelectTokens("items.promoted[*]");

            Console.WriteLine("LISTING OFFERT");
            Console.WriteLine(promotedItems.Count());
            foreach (var offer in promotedItems)
            {
                var id = offer["id"];
                var name = offer["name"];
                var price = offer["sellingMode"]["price"]["amount"];
                // formatowanie wyjscia
                var output = string.Format("{0,-15} | {1,-50} | {2,5}", id, name, price);
                Console.WriteLine(output);
            }
        }

        public async Task GetMyOffers()
        {
            Uri endpoint = new Uri(API_LINK, "/sale/offers");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", Token.AuthorizationHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

            var response = await _client.GetAsync(endpoint);
            var contents = await response.Content.ReadAsStringAsync();

            Console.WriteLine("MOJE OFERTY");
            Console.WriteLine(contents);
        }

        #region Authentication

        public async Task Authorize()
        {
            Token = Utility.DeserializeToken;

            if (Token == default(Token))
            {
                await RequestAccessToken();
            }

            Console.WriteLine("UZYKALEM TOKEN: " + Token.AccessToken);
            Console.WriteLine("=================");
        }

        private async Task RequestAccessToken()
        {
            var authData = await GetAuthData();
            Utility.OpenUrl(authData.VerificationUriComplete);
            Token = await AskServerForToken(clientId, secretId, authData);
            Console.WriteLine("UZYKALEM TOKEN: " + Token.AccessToken);
            Utility.SerializeToken(Token);
        }

        public async Task RefreshAccessToken()
        {
            Uri endpoint = new Uri("https://allegro.pl/auth/oauth/token");

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "refresh_token" },
                {"refresh_token", Token.RefreshToken}
            });

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", GetAuthParameters(clientId, secretId));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsync(endpoint, formContent);
            var contents = await response.Content.ReadAsStreamAsync();
            Token = Utility.Deserialize<Token>(contents);
            Utility.SerializeToken(Token);
        }

        private async Task<Token> AskServerForToken(string clientId, string secretId, AuthorizationData authData)
        {
            return await Task.Run(async () =>
            {
                Token token = null;
                Thread.Sleep(1000 * 15);
                var rtdf = "https://allegro.pl/auth/oauth/token?grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Adevice_code&device_code=";
                var url = new Uri(rtdf + authData.DeviceCode);
                var oAuth = GetAuthParameters(clientId, secretId);
                while (true)
                {
                    var res = await SendRequest(url, oAuth, "application/json", "");

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


        private async Task<AuthorizationData> GetAuthData()
        {
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", clientId }
            });

            _client.BaseAddress = AUTH_LINK;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", GetAuthParameters(clientId, secretId));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsync(AUTH_LINK, formContent);
            var contents = await response.Content.ReadAsStreamAsync();
            var authData = Utility.Deserialize<AuthorizationData>(contents);

            return authData;

        }

        private async Task<Response> SendRequest(Uri url, string authHeader, string allegroHeader, string data)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", authHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(data, Encoding.UTF8, allegroHeader);
            var res = await _client.PostAsync(url, content);

            var response = Response.Initalize(res.StatusCode, res.IsSuccessStatusCode, res.Content.ReadAsStreamAsync().Result);
            return response;
        }

        private string GetAuthParameters(string clientId, string secretId)
        {
            string tuple = clientId + ":" + secretId;
            byte[] bytes = Encoding.UTF8.GetBytes(tuple);
            return "Basic " + Convert.ToBase64String(bytes);
        }
        #endregion
    }
}
