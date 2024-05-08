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
        private static readonly string _baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        public static async Task<WeatherInfo> FetchWeatherInfo(string query)
        {
            HttpClient client = new();
            try
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = client.GetAsync($"?q=Niš&lang=sr&appid=7d7c4b776423f11d3fe44a90e34a78d2").Result;
                if(!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Greska prilikom rada sa API-em");
                }
                var content = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(content);
                var weather = new WeatherInfo();
                weather.City = jsonObject["name"].ToString();
                weather.Weather = jsonObject["weather"][0]["main"].ToString();
                weather.Description = jsonObject["weather"][0]["description"].ToString();
                weather.Temperature = (int)jsonObject["main"]["temp"] - 270;
                weather.Pressure = (int)jsonObject["main"]["pressure"];
                weather.Humidity = (int)jsonObject["main"]["humidity"];
                weather.WindSpeed = (double)jsonObject["wind"]["speed"];
                weather.WindDirection = (int)jsonObject["wind"]["deg"];
                weather.Cloudiness = (int)jsonObject["clouds"]["all"];
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                weather.Sunrise = dateTime.AddSeconds((int)jsonObject["sys"]["sunrise"]).ToLocalTime();
                weather.Sunset = dateTime.AddSeconds((int)jsonObject["sys"]["sunset"]).ToLocalTime();
                return weather;
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
