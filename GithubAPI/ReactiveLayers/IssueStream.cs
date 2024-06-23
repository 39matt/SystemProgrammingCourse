using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Net;
using Octokit;

namespace GithubAPI.ReactiveLayers
{
    internal class IssueStream : IObservable<Octokit.Issue>
    {
        private readonly Subject<Octokit.Issue> issueSubject;
        public IssueStream()
        {
            issueSubject = new Subject<Octokit.Issue>();
        }
        public void GetIssues(string username, string repo, IScheduler scheduler)
        {
            Observable.Start(async () =>
            {
                try
                {
                    GitHubClient client = new GitHubClient(new Octokit.ProductHeaderValue("github"));
                    IIssuesClient issuesclient = client.Issue;
                    var myissues = await issuesclient.GetAllForRepository(username, repo);
                    foreach (var issue in myissues)
                    {
                        issueSubject.OnNext(issue);
                    }   
                    issueSubject.OnCompleted();
                }
                catch (Exception ex)
                {
                    issueSubject.OnError(ex);
                }

            }, scheduler);
        }
        public IDisposable Subscribe(IObserver<Octokit.Issue> observer)
        {
            return issueSubject.Subscribe(observer);
        }
    }
}
