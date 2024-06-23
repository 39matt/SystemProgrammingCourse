using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Octokit;
using System.Text.Json;

namespace GithubAPI.ReactiveLayers
{
    internal static class TopicModeling
    {
        public static void PerformTopicModeling(IReadOnlyList<IssueComment> issueComments, int numberOfTopics)
        {
            var mlContext = new MLContext();

            // Convert comments to documents
            var documents = issueComments.Select(r => new Document { Body = r.Body }).ToList();
            var data = mlContext.Data.LoadFromEnumerable(documents);

            // Define the text processing pipeline
            var textPipeline = mlContext.Transforms.Text
                .NormalizeText("NormalizedText", "Body")
                .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "NormalizedText"))
                .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens"))
                .Append(mlContext.Transforms.Text.ProduceWordBags("BagOfWords", "Tokens"))
                .Append(mlContext.Transforms.Text.LatentDirichletAllocation("Topics", "BagOfWords", numberOfTopics: numberOfTopics));

            // Train the model
            var model = textPipeline.Fit(data);

            // Transform the data
            var transformedData = model.Transform(data);

            // Extract and display topics
            var topics = mlContext.Data.CreateEnumerable<TransformedDocument>(transformedData, false);

            foreach (var doc in topics)
            {
                Console.WriteLine($"Comment: {doc.Body}");
                Console.WriteLine("Topics:");

                for (int i = 0; i < doc.Topics.Length; i++)
                {
                    string topicName = $"Topic {i + 1}";
                    Console.WriteLine($"  {topicName}: {doc.Topics[i]}");
                }

                Console.WriteLine();
            }
        }
    }

    public class Document
    {
        public string Username { get; set; }
        public string Body { get; set; }
    }

    public class TransformedDocument : Document
    {
        public float[] Topics { get; set; }
    }
}
