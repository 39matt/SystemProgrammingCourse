using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace OpenWeatherMapAPI
{
    internal class HTTPServer
    {
        private readonly HttpListener _httpListener;
        private readonly Thread _listenerThread;
        private bool _running;

        public HTTPServer()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:5050/");
            _listenerThread = new Thread(Listen);
            _running = false;
        }

        private static void SendResponse(HttpListenerContext context, byte[] responseBody, string contentType, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var logString =
                $"REQUEST:\n{context.Request.HttpMethod} {context.Request.RawUrl} HTTP/{context.Request.ProtocolVersion}\n" +
                $"Host: {context.Request.UserHostName}\nUser-agent: {context.Request.UserAgent}\n-------------------\n" +
                $"RESPONSE:\nStatus: {statusCode}\nDate: {DateTime.Now}\nContent-Type: {contentType}" +
                $"\nContent-Length: {responseBody.Length}\n";
            context.Response.ContentType = contentType;
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentLength64 = responseBody.Length;
            using (Stream outputStream = context.Response.OutputStream)
            {
                outputStream.Write(responseBody, 0, responseBody.Length);
            }
            Console.WriteLine(logString);
        }

        private void AcceptConnection(HttpListenerContext context)
        {
            var request = context.Request;
            if (request.HttpMethod != "GET")
            {
                SendResponse(context, "Method not allowed!"u8.ToArray(), "text/plain", HttpStatusCode.MethodNotAllowed);
                return;
            }
            try
            {
                var uri = request.Url;
                var queryParams = HttpUtility.ParseQueryString(uri.Query);
                if (uri.ToString() == String.Empty)
                {
                    SendResponse(context, "No city and days given!"u8.ToArray(), "text/plain", HttpStatusCode.BadRequest);
                    return;
                }
                if (request.RawUrl == "/favicon.ico")
                {
                    return;
                }
                var city = queryParams["q"]!;
                var days = queryParams["cnt"]!;
                if (!city.All(Char.IsLetter))
                {
                    SendResponse(context, "City can only contain letters!"u8.ToArray(), "text/plain", HttpStatusCode.UnprocessableContent);
                    return;
                }

                List<WeatherInfo> weatherInfos = WeatherSearchService.FetchWeatherInfo(city, days);
                if (weatherInfos == null)
                {
                    SendResponse(context, "API returned an error!"u8.ToArray(), "text/plain", HttpStatusCode.InternalServerError);
                }
                if (weatherInfos!.Count == 0)
                {
                    SendResponse(context, "No data for given name and surname found!"u8.ToArray(), "text/plain", HttpStatusCode.NoContent);
                    return;
                }
                string allInfos = String.Join(Environment.NewLine, weatherInfos);
                byte[] dataAsBytes = System.Text.Encoding.UTF8.GetBytes(allInfos);
                SendResponse(context, dataAsBytes, "text/plain");
            }
            catch (HttpRequestException)
            {
                SendResponse(context, "API returned an error!"u8.ToArray(), "text/plain", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                SendResponse(context, "Unknown error!"u8.ToArray(), "text/plain", HttpStatusCode.InternalServerError);
                throw new Exception(ex.Message);
            }
        }

        public void Start()
        {
            _httpListener.Start();
            _listenerThread.Start();
            _running = true;
            Console.WriteLine("Server pokrenut!");
        }

        public void Stop()
        {
            _httpListener.Stop();
            _listenerThread.Join();
            _running = false;
            Console.WriteLine("Server zaustavljen!");
        }

        private void Listen()
        {
            while (_running)
            {
                try
                {
                    var context = _httpListener.GetContext();
                    if (_running)
                    {
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            AcceptConnection(context);
                        });
                    }

                }
                catch (HttpListenerException)
                {
                    Console.WriteLine("Server prestaje sa slusanjem!");
                }
            }
        }
    }
}
