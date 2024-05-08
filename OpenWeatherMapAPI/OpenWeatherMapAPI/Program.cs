using OpenWeatherMapAPI;
using System;
using System.Diagnostics;
using System.IO;

string city = "Sydney";
string days = "7";
Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

var response = await WeatherSearchService.FetchWeatherInfoAsync(city,days);
Console.WriteLine(city);
Console.WriteLine("------------------------------");
foreach (var item in response)
{
    Console.WriteLine($"Vreme: {item.Time}");
    Console.WriteLine($"Min: {item.TempMax}");
    Console.WriteLine($"Max: {item.TempMin}");
}
stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");