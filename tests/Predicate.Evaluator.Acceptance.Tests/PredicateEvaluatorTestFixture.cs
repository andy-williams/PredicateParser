using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Nest;
using Xunit;

namespace Predicate.Evaluator.Acceptance.Tests
{
    [CollectionDefinition("PredicateEvaluatorTest")]
    public class PredicateEvaluatorTestCollection : ICollectionFixture<PredicateEvaluatorTestFixture>
    {
    }

    public class PredicateEvaluatorTestFixture : IDisposable
    {
        public static ElasticSearchPredicateEvaluator Evaluator { get; private set; }

        public PredicateEvaluatorTestFixture()
        {
            InitDependencies();
            SetupElasticSearch();
        }

        private void InitDependencies()
        {
            Console.WriteLine("Starting Dependencies");
            var slash = Path.DirectorySeparatorChar;

            var isWindows =
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform
                    .Windows);

            var toExec = new ProcessStartInfo
            {
                WorkingDirectory = $"{Directory.GetCurrentDirectory()}{slash}..{slash}..{slash}..{slash}..{slash}..",
                Arguments = "-f docker-compose.yaml -p acceptance up -d --no-recreate",
                FileName = isWindows ? "docker-compose.exe" : "docker-compose",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(toExec);
            process.WaitForExit();
            var message = process.StandardOutput.ReadToEnd();
            var errors = process.StandardError.ReadToEnd();

            Console.Write($"message: ${message}");
            Console.Write($"errors: ${errors}");
            Assert.Equal(0, process.ExitCode);
        }

        private void SetupElasticSearch()
        {
            var index = "candidates";
            var settings = new ConnectionSettings(new Uri("http://localhost:9200/"));
            settings.DisableDirectStreaming();
            settings.EnableDebugMode(d =>
            {
                Console.Write(d.DebugInformation);
            });
            var client = new ElasticClient(settings);
            client.Indices.Delete(index);
            client.Indices.Create(index, c => c.Map(x => x.AutoMap<CandidateDocument>()));
            client.IndexMany(new List<CandidateDocument>
            {
                new CandidateDocument { CurrentJobTitle = "Software Engineer", ExperienceInYears = 5, Salary = 70000 },
                new CandidateDocument { CurrentJobTitle = "Full-stack Engineer", ExperienceInYears = 7, Salary = 85000 },
                new CandidateDocument { CurrentJobTitle = "Marketing Manager", ExperienceInYears = 4, Salary = 60000 },
                new CandidateDocument { CurrentJobTitle = "Head of Security", ExperienceInYears = 9, Salary = 100000 },
                new CandidateDocument { CurrentJobTitle = "Automation Engineer", ExperienceInYears = 5, Salary = 73000 },
                new CandidateDocument { CurrentJobTitle = ".NET Developer", ExperienceInYears = 3, Salary = 50000 },
                new CandidateDocument { CurrentJobTitle = "Developer", ExperienceInYears = 4, Salary = 58000 },
                new CandidateDocument { CurrentJobTitle = "Junior Developer", ExperienceInYears = 1, Salary = 38000 },
            }, index);

            Task.Delay(1000).GetAwaiter().GetResult();
            Evaluator = new ElasticSearchPredicateEvaluator(client, index, new PropertyDetailsProvider());
        }

        public void Dispose()
        {
        }
    }
}
