using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GithubAPI.Models;
using Octokit;

namespace GithubAPI.ReactiveLayers
{
    internal class IssueObserver : IObserver<Octokit.Issue>
    {
        
        private readonly string _name;

        public IssueObserver(string name)
        {
            _name = name;
        }

        public void OnCompleted()
        {
            Console.WriteLine($"{_name}: All businesses returned successfully!");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"{_name}: Error happened: {error.Message}");
        }

        public void OnNext(Octokit.Issue issue)
        {
            if (issue.Title != null)
                Console.WriteLine($"{_name}: {issue.Title}\n-----------------\n{issue.Body}");
            else return;
        }
    }
}
