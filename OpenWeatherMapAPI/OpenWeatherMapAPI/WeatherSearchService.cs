using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI
{
    internal class WeatherSearchService
    {
        private static readonly string _apiKey = "7d7c4b776423f11d3fe44a90e34a78d2";
        private static readonly string _baseUrl = "http://api.openweathermap.org/data/2.5/forecast";

        public static async Task<List<WeatherInfo>> FetchWeatherInfoAsync(string city, string days)
        {
            HttpClient client = new();
            try
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = client.GetAsync($"?q={city}&cnt={days}&appid={_apiKey}").Result;
                if(!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Greska prilikom rada sa API-em");
                }
                var content = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(content);
                var forecast = jsonObject["list"].Select(x => new WeatherInfo
                {
                    Time = x["dt_txt"].ToString(),
                    TempMin = Math.Round((double)x["main"]["temp_min"] - 270, 0),
                    TempMax = Math.Round((double)x["main"]["temp_max"] - 270, 0),
                    Weather = (string)x["weather"][0]["main"],
                    Description = (string)x["weather"][0]["description"],
                    Pressure = (double)x["main"]["pressure"],
                    Humidity = (int)x["main"]["humidity"],
                    WindSpeed = (double)x["wind"]["speed"],
                    WindDirection = (int)x["wind"]["deg"],
                    Cloudiness = (int)x["clouds"]["all"],
                    Rain = (double)x["rain"]["3h"]
                }).ToList();
                return forecast;
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Greska prilikom izvrsenja: {e.Message}");
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
