using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace igiagbin
{
    public class TurnstileResponse
    {
        public bool Success { get; set; }
        // Add other properties you need from the response
    }

    public class TurnstileVerifier
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public TurnstileVerifier(string secretKey)
        {
            _httpClient = new HttpClient();
            _secretKey = secretKey;
        }

        public async Task<TurnstileResponse> VerifyTokenAsync(string token)
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", _secretKey),
            new KeyValuePair<string, string>("response", token),
        });

            var response = await _httpClient.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TurnstileResponse>(responseString)!;
        }
    }

}
