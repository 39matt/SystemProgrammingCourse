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
        public static string PerformTopicModeling(IReadOnlyList<Issue> issues)
        {
            var mlContext = new MLContext();

            // Convert Issues to Documents
            var documents = issues.Select(i => new Document { Text = i.Title }).ToList();
            var data = mlContext.Data.LoadFromEnumerable(documents);

            // Define the Data Processing Pipeline
            var textPipeline = mlContext.Transforms.Text
                .NormalizeText("NormalizedText", nameof(Document.Text))
                .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "NormalizedText"))
                .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens"))
                .Append(mlContext.Transforms.Text.ProduceWordBags("BagOfWords", "Tokens"))
                .Append(mlContext.Transforms.Text.LatentDirichletAllocation("Topics", "BagOfWords", numberOfTopics: 2));

            // Train the Model
            var model = textPipeline.Fit(data);

            // Transform the Data
            var transformedData = model.Transform(data);

            // Extract and Return Topics
            var topics = mlContext.Data.CreateEnumerable<TransformedDocument>(transformedData, reuseRowObject: false).ToList();

            var result = topics.Select(doc => new
            {
                doc.Text,
                Topics = doc.Topics.Select((topicWeight, index) => new { Topic = $"Topic {index + 1}", Weight = topicWeight }).ToList()
            }).ToList();

            return JsonSerializer.Serialize(result);
        }
    }

    public class Document
    {
        public string Text { get; set; }
    }

    public class TransformedDocument : Document
    {
        public float[] Topics { get; set; }
    }
}
