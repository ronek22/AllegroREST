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
        #region Constructor and Fields
        private static readonly bool SANDBOX = false;
        private HttpClient _client { get; }
        private readonly IConfiguration _configuration;
        private readonly string clientId;
        private readonly string secretId;
        private readonly Uri BASE_LINK;
        private readonly Uri API_LINK;

        private Token Token { set; get; }

        public AllegroClient(HttpClient client, IConfigurationRoot configuration)
        {
            _client = client;
            _configuration = configuration;
            configureEnvironemnt(out clientId, out secretId, out BASE_LINK, out API_LINK);
        }

        private void configureEnvironemnt(out string _clientId, out string _secretId, out Uri _base, out Uri _api)
        {
            IConfigurationSection environment = SANDBOX ? _configuration.GetSection("API:SANDBOX") : _configuration.GetSection("API:PRODUCTION");

            _clientId = environment.GetValue<string>("CLIENT_ID");
            _secretId = environment.GetValue<string>("SECRET_ID");
            _base = new Uri(environment.GetValue<string>("BASE_LINK"));
            _api = new Uri(environment.GetValue<string>("API_LINK"));

        }

        #endregion

        #region Feature functions

        public async Task EditOffer(string noAuction, string price, string currency, int available, string unit)
        {
            Offer editedOffer = await GetOffer(noAuction);
            editedOffer.SellingMode.Price.Amount = price; // cena w rest api podana jest jako string
            editedOffer.SellingMode.Price.Currency = currency;
            editedOffer.Stock.Available = available;
            editedOffer.Stock.Unit = unit;

            // TODO: How to PUT THIS TO REST API
            Uri uri = new Uri(API_LINK, $"/sale/offers/{noAuction}");
            SetDefaultHeaders();
            var jsonContent = new StringContent(Utility.Serialize(editedOffer), Encoding.UTF8, "application/vnd.allegro.public.v1+json");
            var response = await _client.PutAsync(uri, jsonContent);
            var contents = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Results");
            Console.WriteLine(JObject.Parse(contents));
        }

        private async Task<Offer> GetOffer(string nrAukcji)
        {
            UriBuilder builder = new UriBuilder($"{API_LINK}sale/offers/{nrAukcji}");
            SetDefaultHeaders();
            var json = await GetRequestAndParseAsync(builder.Uri);
            return json.ToObject<Offer>();
        }

        public async Task<String> GetOfferDetails(string nrAukcji)

        {
            Uri endpoint = new Uri($"{API_LINK}sale/offers/{nrAukcji}");
            SetDefaultHeaders();
            var json = await GetRequestAndParseAsync(endpoint);

            // Mapowanie json do obiektu klasy Offer
            Offer offer = json.ToObject<Offer>();

            string plu = "";
            int poz = offer.Description.IndexOf("Kod PLU: __");

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
            UriBuilder builder = new UriBuilder($"{API_LINK}offers/listing");
            // Dodwania parametrow
            var paramValues = HttpUtility.ParseQueryString(builder.Query);
            paramValues.Add("phrase", phrase);
            paramValues.Add("limit", "20");
            builder.Query = paramValues.ToString();

            SetDefaultHeaders();
            var json = await GetRequestAndParseAsync(builder.Uri);

            var promotedItems = json.SelectTokens("items.promoted[*]");

            Console.WriteLine("LISTING OFFERT");
            Console.WriteLine(promotedItems.Count());
            foreach (var offer in promotedItems)
            {
                var currentOffer = offer.ToObject<Offer>();
                var output = string.Format("{0,-15} | {1,-50} | {2,5}", 
                    currentOffer.Id, currentOffer.Name, currentOffer.SellingMode.Price.Amount);
                Console.WriteLine(output);
            }
        }

        public async Task GetMyOffers()
        {
            Uri endpoint = new Uri(API_LINK, "/sale/offers");
            SetDefaultHeaders();
            var json = await GetRequestAndParseAsync(endpoint);

            Console.WriteLine("MOJE OFERTY");
            Console.WriteLine(json);
        }
        #endregion

        #region Helpers

        private async Task<JObject> GetRequestAndParseAsync(Uri endpoint) 
        {
            var response = await _client.GetAsync(endpoint);
            var contents = await response.Content.ReadAsStringAsync();
            return JObject.Parse(contents);
        }

        private void SetDefaultHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", Token.AuthorizationHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));
        }
        #endregion

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
            Uri endpoint = new Uri(BASE_LINK, "/auth/oauth/token");

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
                var rtdf = $"{BASE_LINK}auth/oauth/token?grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Adevice_code&device_code=";
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

            var AUTH_LINK = new Uri(BASE_LINK, "/auth/oauth/device");


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
