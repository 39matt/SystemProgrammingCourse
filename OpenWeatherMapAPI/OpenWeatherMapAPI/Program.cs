using OpenWeatherMapAPI;
using System;
using System.Diagnostics;
using System.IO;

var server = new HTTPServer();
server.Start();
Console.WriteLine("Press Enter to stop the server...");
while (Console.ReadKey().Key != ConsoleKey.Enter)
    server.Stop();
Console.WriteLine("Server stopped!");