using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Octokit;

namespace GithubAPI.ReactiveLayers
{
    internal class IssueCommentObserver : IObserver<Octokit.IssueComment>
    {

        private readonly string _name;
        private Octokit.IssueComment issueComment;

        public IssueCommentObserver(string name)
        {
            _name = name;
        }

        public void OnCompleted()
        {
            Console.WriteLine($"{_name}: All issues returned successfully!");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"{_name}: Error happened: {error.Message}");
        }

        public void OnNext(Octokit.IssueComment issueComment)
        {
            if (issueComment.Url != null)
                Console.WriteLine($"{_name}: {issueComment.User.Name}\n-----------------\n{issueComment.Body}\n#################################");
            else return;
        }
    }
}
