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
    internal class IssueCommentStream : IObservable<Octokit.IssueComment>
    {
        private readonly Subject<Octokit.IssueComment> issueCommentsSubject;
        public IssueCommentStream()
        {
            issueCommentsSubject = new Subject<Octokit.IssueComment>();
        }
        public void GetIssueComments(string username, string repo, int issueId, IScheduler scheduler)
        {
            Observable.Start(async () =>
            {
                try
                {
                    GitHubClient client = new GitHubClient(new Octokit.ProductHeaderValue("github"));
                    IRepositoriesClient repositoryClient = client.Repository;
                    Repository repository = await repositoryClient.Get(username, repo);
                    IIssueCommentsClient issueCommentsClient = client.Issue.Comment;

                    var comments = await issueCommentsClient.GetAllForIssue(repository.Id, issueId);
                    foreach (var comment in comments)
                    {
                        issueCommentsSubject.OnNext(comment);
                    }
                    issueCommentsSubject.OnCompleted();
                    TopicModeling.PerformTopicModeling(comments, 5);
                }
                catch (Exception ex)
                {
                    issueCommentsSubject.OnError(ex);
                }

            }, scheduler);
        }
        public IDisposable Subscribe(IObserver<Octokit.IssueComment> observer)
        {
            return issueCommentsSubject.Subscribe(observer);
        }
    }
}
