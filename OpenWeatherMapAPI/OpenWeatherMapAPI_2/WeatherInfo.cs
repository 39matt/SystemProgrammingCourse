using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI_2
{
    internal class WeatherInfo
    {
        public string City { get; set; }
        public string Time { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public string Weather { get; set; }
        public string Description { get; set; }
        public double Pressure { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public int Cloudiness { get; set; }
        public override string ToString()
        {
            return $"--------------------------------\nTime: {Time}\n---------------------------\nCity: {City}\nTempMin: {TempMin} C\nTempMax: {TempMax} C\nWeather: {Weather}\nDescription: {Description}\nPressure: {Pressure} hPa\nHumidity: {Humidity}%\nWindSpeed: {WindSpeed}m/s\nWindDirection: {WindDirection}\nCloudiness: {Cloudiness}%\n";
        }
    }
}
