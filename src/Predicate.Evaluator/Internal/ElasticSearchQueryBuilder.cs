using System;
using Nest;
using Predicate.Parser.Symbols;
using Operator = Predicate.Parser.Symbols.Operator;

namespace Predicate.Evaluator.Internal
{
    internal class ElasticSearchQueryBuilder
    {
        private readonly IPropertyDetailsProvider _propertyDetailsProvider;

        public ElasticSearchQueryBuilder(IPropertyDetailsProvider propertyDetailsProvider)
        {
            _propertyDetailsProvider = propertyDetailsProvider;
        }

        public QueryContainer BuildNestQuery(PredicateNode root)
        {
            return Visit(root);
        }

        private QueryContainer Visit(PredicateNode node)
        {
            if (node.Type == SymbolType.BooleanOperator)
                return VisitBooleanOperator(node);
            if (node.Type == SymbolType.Operator)
                return VisitOperator(node);

            throw new Exception("Unable to create from search query from tree");
        }

        private QueryContainer VisitBooleanOperator(PredicateNode node)
        {
            var value = (BooleanOperator) node.Value;

            var predicate1 = node.Children[0];
            var predicate2 = node.Children[1];

            if (value == BooleanOperator.And)
                return Visit(predicate1) && Visit(predicate2);

            return Visit(predicate1) || Visit(predicate2);
        }


        private QueryContainer VisitOperator(PredicateNode node)
        {
            var value = (Operator) node.Value;
            var op1 = (Operand) node.Children[0].Value;
            var op2 = (Operand) node.Children[1].Value;

            if (op1.OperandType != OperandType.Property)
                throw new Exception("First operand must be a property");

            var prop1Name = (string) op1.Value;
            var prop1Details = _propertyDetailsProvider.GetPropertyDetails(prop1Name);
            if (prop1Details == null)
                throw new Exception($"Property {prop1Name} does not exist");

            if (op2.OperandType == OperandType.Property)
                throw new Exception("Secon operand cannot be a property");

            switch (value)
            {
                case Operator.Contains:
                    if (op2.OperandType != OperandType.String)
                        throw new Exception("Second operand must be string");

                    return new QueryContainer(new MatchQuery
                    {
                        Query = (string) op2.Value,
                        Field = prop1Details.SourceName
                    });
                case Operator.Equal:
                    // can be a string or number
                    return new QueryContainer(new MatchQuery
                    {
                        Query = (string) op2.Value,
                        Field = prop1Details.SourceName
                    });
                case Operator.NotEqual:
                    return !(new QueryContainer(new MatchQuery
                    {
                        Query = (string)op2.Value,
                        Field = prop1Details.SourceName
                    }));
                case Operator.LessThan:
                case Operator.LessThanEqual:
                case Operator.GreaterThan:
                case Operator.GreaterThanEqual:
                    if (op2.OperandType != OperandType.Number)
                        throw new Exception($"Second operator {op2.Value} must be a number");

                    if (prop1Details.Type != ConcreteType.Number)
                        throw new Exception($"First operator {op1.Value} must be a property of type number");

                    var rangeQuery = new NumericRangeQuery
                    {
                        Field = prop1Details.SourceName
                    };

                    if (value == Operator.LessThan)
                        rangeQuery.LessThan = (double)(decimal)op2.Value;
                    if (value == Operator.LessThanEqual)
                        rangeQuery.LessThanOrEqualTo = (double)(decimal)op2.Value;
                    if (value == Operator.GreaterThan)
                        rangeQuery.GreaterThan = (double) (decimal) op2.Value;
                    if (value == Operator.GreaterThanEqual)
                        rangeQuery.GreaterThanOrEqualTo = (double)(decimal)op2.Value;
                    return new QueryContainer(rangeQuery);
                default:
                    throw new Exception($"Unknown Operator {node.Value}");
            }
        }
    }
}
