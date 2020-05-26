using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Nest;
using Predicate.Evaluator.Internal;
using Predicate.Parser;

namespace Predicate.Evaluator
{
    public class ElasticSearchPredicateEvaluator : IPredicateEvaluator<CandidateSearchResult>
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _index;
        private readonly IPropertyDetailsProvider _propertyDetailsProvider;

        public ElasticSearchPredicateEvaluator(IElasticClient elasticClient, string index, IPropertyDetailsProvider propertyDetailsProvider)
        {
            _elasticClient = elasticClient;
            _index = index;
            _propertyDetailsProvider = propertyDetailsProvider;
        }

        public async Task<PredicateEvaluationOperation<CandidateSearchResult>> Evaluate(string predicateExpression)
        {
            var charStream = new AntlrInputStream(predicateExpression);
            var lexer = new PredicateLexer(charStream);
            var stream = new CommonTokenStream(lexer);

            stream.Fill();
            var tokens = stream.Get(0, stream.Size);
            stream.Reset();

            if (tokens.Any(x => x.Type == PredicateLexer.Discardable))
                throw new Exception("Contains unknown tokens");

            var parser = new PredicateParser(stream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener());
            var treeBuilder = new PredicateSyntaxTreeBuilderVisitor();
            var tree = treeBuilder.Visit(parser.expr());

            var searchBuilder = new ElasticSearchQueryBuilder(_propertyDetailsProvider);
            var query = searchBuilder.BuildNestQuery(tree);

            var searchResult = await _elasticClient.SearchAsync<CandidateDocument>(new SearchRequest(_index)
            {
                Query = query
            });

            var resultItems = searchResult.Documents
                .Select(x => new CandidateSearchResultItem
                {
                    CurrentJobTitle = x.CurrentJobTitle,
                    Salary = x.Salary,
                    ExperienceInYears = x.ExperienceInYears
                })
                .ToList();

            return new PredicateEvaluationOperation<CandidateSearchResult>(new CandidateSearchResult
            {
                Items = resultItems
            });
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

    internal class ThrowingErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException ex)
        {
            throw ex;
        }
    }
}