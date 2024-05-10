using OpenWeatherMapAPI;
using System;
using System.Diagnostics;
using System.IO;

var server = new HTTPServer();
server.Start();
Console.WriteLine("Pritisni Enter da zaustavis server...");
while (Console.ReadKey().Key != ConsoleKey.Enter)
    server.Stop();
Console.WriteLine("Server zaustavljen!");