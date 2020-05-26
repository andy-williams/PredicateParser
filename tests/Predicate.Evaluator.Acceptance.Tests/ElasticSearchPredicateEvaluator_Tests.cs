using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Predicate.Evaluator.Acceptance.Tests
{
    [Collection("PredicateEvaluatorTest")]
    public class ElasticSearchPredicateEvaluator_Tests
    {
        private readonly ElasticSearchPredicateEvaluator _evaluator;

        public ElasticSearchPredicateEvaluator_Tests()
        {
            _evaluator = PredicateEvaluatorTestFixture.Evaluator;
        }

        [Fact]
        public async Task It_Returns_CorrectResults()
        {
            var result = await _evaluator.Evaluate("@current_job_title contains \"Developer\" and @experience_years < 4");
            Assert.Equal(2, result.Result.Items.Count);
            Assert.Contains(result.Result.Items, c => c.CurrentJobTitle == "Junior Developer");
            Assert.Contains(result.Result.Items, c => c.CurrentJobTitle == ".NET Developer");
        }
    }
}
