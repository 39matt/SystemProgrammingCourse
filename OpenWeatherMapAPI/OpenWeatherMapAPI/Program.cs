using OpenWeatherMapAPI;
using System;
using System.Diagnostics;
using System.IO;

string query = "London";
Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

WeatherInfo list = await WeatherSearchService.FetchWeatherInfo(query);
