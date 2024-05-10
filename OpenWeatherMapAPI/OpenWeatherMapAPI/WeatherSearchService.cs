using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenWeatherMapAPI
{
    internal class WeatherSearchService
    {
        private static readonly string _apiKey = "7d7c4b776423f11d3fe44a90e34a78d2";
        private static readonly string _baseUrl = "http://api.openweathermap.org/data/2.5/forecast";

        public static List<WeatherInfo> FetchWeatherInfo(string city, string days)
        {
            HttpClient client = new();
            try
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = client.GetAsync($"?q={city}&cnt={days}&lng=sr&appid={_apiKey}").Result;
                if(!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Greska prilikom rada sa API-em");
                }
                var content = response.Content.ReadAsStringAsync().Result;
                var jsonObject = JsonConvert.DeserializeObject<JObject>(content);

                List<WeatherInfo> forecast = new List<WeatherInfo>();
                foreach (var item in jsonObject["list"])
                {
                    WeatherInfo w = new WeatherInfo();
                    w.City = jsonObject["city"]["name"].ToString();
                    w.Time = item["dt_txt"].ToString();
                    w.TempMin = Math.Round((double)item["main"]["temp_min"] - 270, 0);
                    w.TempMax = Math.Round((double)item["main"]["temp_max"] - 270, 0);
                    w.Weather = (string)item["weather"][0]["main"];
                    w.Description = (string)item["weather"][0]["description"];
                    w.Pressure = (double)item["main"]["pressure"];
                    w.Humidity = (int)item["main"]["humidity"];
                    w.WindSpeed = (double)item["wind"]["speed"];
                    w.WindDirection = (int)item["wind"]["deg"];
                    w.Cloudiness = (int)item["clouds"]["all"];
                    forecast.Add(w);
                }
                
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
