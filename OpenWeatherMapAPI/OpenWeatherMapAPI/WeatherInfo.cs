using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI
{
    internal class WeatherInfo
    {
        public string City { get; set; }
        public string Weather { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public int Cloudiness { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

    }
}
