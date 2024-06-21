using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Net;
using GithubAPI.Models;
using Octokit;

namespace GithubAPI.ReactiveLayers
{
    internal class IssueStream
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
                    GitHubClient client = new GitHubClient(new Octokit.ProductHeaderValue("Octokit.Samples"));
                    IIssuesClient issuesclient = client.Issue;
                    var myissues = await issuesclient.GetAllForRepository("octokit", "octokit.net");
                    foreach (var issue in myissues)
                    {
                        issueSubject.OnNext(issue);
                    }   
                    issueSubject.OnCompleted();
                    TopicModeling.PerformTopicModeling(myissues);
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
