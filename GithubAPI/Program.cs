using System.Net;
using Newtonsoft.Json;
using GithubAPI;
using GithubAPI.ReactiveLayers;
using GitHub;


HttpServer server;
string url = "http://localhost:8080/";
server = new HttpServer(url);
server.Start();

Console.ReadLine();
server.Stop();