using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenWeatherMapAPI
{
    internal class WeatherAPI
    {
        private readonly string _apiKey;
        private readonly string _apiURL;
        private HttpClient httpClient;

        public WeatherAPI()
        {
            _apiKey = "7d7c4b776423f11d3fe44a90e34a78d2";
        }

        public JObject FetchWeatherInfo()
        {
            HttpResponseMessage response = httpClient.GetAsync(_apiURL).Result;
            response.EnsureSuccessStatusCode();


        }
    }
}
