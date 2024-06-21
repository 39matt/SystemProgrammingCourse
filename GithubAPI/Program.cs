using System.Net;
using Newtonsoft.Json;
using GithubAPI;
using GithubAPI.Models;
using GithubAPI.ReactiveLayers;
using GitHub;


HttpServer server = new();
server.Start();

Reader reader = new();
reader.Subscribe(server);

CommentAnalyzer commentAnalyzer = new();
commentAnalyzer.Subscribe(reader);


Console.ReadLine();

reader.Unsubscribe();
server.Dispose();