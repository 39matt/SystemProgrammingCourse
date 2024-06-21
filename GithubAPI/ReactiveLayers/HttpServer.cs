using GithubAPI;
using System.Net;
using System.Reactive.Concurrency;
using System.Text;

namespace GithubAPI.ReactiveLayers;

public class HttpServer
{
    private readonly string url;
    private IssueStream? issueStream;
    private IDisposable? subscription1;
    private IDisposable? subscription2;
    private IDisposable? subscription3;

    public HttpServer(string url)
    {
        this.url = url;
    }

    public void Start()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(url);

        listener.Start();
        Console.WriteLine("Server started. Listening for incoming requests...");

        while (true)
        {
            var context = listener.GetContext();
            Task.Run(() => HandleRequest(context));
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        byte[] buffer;

        if (request.HttpMethod == "GET")
        {
            string username = request.QueryString["username"]!;
            string repo = request.QueryString["repo"]!;

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(repo))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                buffer = Encoding.UTF8.GetBytes("Bad request!");
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            else
            {

                IScheduler scheduler = NewThreadScheduler.Default;

                issueStream = new IssueStream();
                var observer1 = new IssueObserver("Observer 1");
                var observer2 = new IssueObserver("Observer 2");
                var observer3 = new IssueObserver("Observer 3");

                var filteredStream = issueStream;

                subscription1 = filteredStream.Subscribe(observer1);
                subscription2 = filteredStream.Subscribe(observer2);
                subscription3 = filteredStream.Subscribe(observer3);

                issueStream.GetIssues(username, repo, scheduler);

                response.StatusCode = (int)HttpStatusCode.OK;
                buffer = Encoding.UTF8.GetBytes("Request received. Processing businesses...");
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.OutputStream.Close();
        }
    }

    public void Stop()
    {
        subscription1!.Dispose();
        subscription2!.Dispose();
        subscription3!.Dispose();
    }
}