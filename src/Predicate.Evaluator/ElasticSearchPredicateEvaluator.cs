using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Predicate.Evaluator
{
    public class ElasticSearchPredicateEvaluator : IPredicateEvaluator<CandidateSearchResult>
    {
        public ElasticSearchPredicateEvaluator()
        {
           
        }

        public async Task<PredicateEvaluationOperation<CandidateSearchResult>> Evaluate(string predicateExpression)
        {
            throw new NotImplementedException();
        }
    }

    public class CandidateSearchResult
    {
        public IList<CandidateSearchResultItem> Items { get; set; }
    }

    public class CandidateSearchResultItem
    {
        public string CurrentJobTitle { get; set; }
        public int ExperienceInYears { get; set; }
        public int Salary { get; set; }
    }

    public class CandidateDocument
    {
        public string CurrentJobTitle { get; set; }
        public int ExperienceInYears { get; set; }
        public int Salary { get; set; }
    }
}