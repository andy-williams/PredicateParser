using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using Predicate.Parser.Symbols;

[assembly: InternalsVisibleTo("Predicate.Evaluator")]
namespace Predicate.Parser
{
    internal class PredicateSyntaxTreeBuilderVisitor : PredicateBaseVisitor<PredicateNode>
    {
        public override PredicateNode VisitExpr(PredicateParser.ExprContext context)
        {
            return Visit(context.predicate());
        }

        public override PredicateNode VisitPredicate(PredicateParser.PredicateContext context)
        {
            // OpenParen predicate CloseParen
            if (context.OpenParen() != null)
            {
                return Visit(context.predicate().First());
            }

            // predicate booleanOperator predicate
            if (context.booleanOperator() != null)
            {
                var booleanOperator = Visit(context.booleanOperator());
                booleanOperator.Children.Add(Visit(context.predicate()[0]));
                booleanOperator.Children.Add(Visit(context.predicate()[1]));
                return booleanOperator;
            }

            // operand operator operand
            if (context.@operator() != null)
            {
                var @operator = Visit(context.@operator());
                @operator.Children.Add(Visit(context.operand()[0]));
                @operator.Children.Add(Visit(context.operand()[1]));
                return @operator;
            }

            throw new Exception("Unhandled Predicate");
        }

        public override PredicateNode VisitOperand(PredicateParser.OperandContext context)
        {
            var terminal = (ITerminalNode)context.GetChild(0);
            var symbolType = terminal.Symbol.Type;

            switch (symbolType)
            {
                case PredicateLexer.String:
                    return new PredicateNode(SymbolType.Operand, new Operand(OperandType.String, terminal.Symbol.Text.Trim('"')));
                case PredicateLexer.Number:
                    return new PredicateNode(SymbolType.Operand, new Operand(OperandType.Number, decimal.Parse(terminal.Symbol.Text)));
                case PredicateLexer.Property:
                    return new PredicateNode(SymbolType.Operand, new Operand(OperandType.Property, terminal.Symbol.Text.Trim('@')));
            }

            throw new Exception("Unhandled Operand");
        }

        public override PredicateNode VisitBooleanOperator(PredicateParser.BooleanOperatorContext context)
        {
            var terminal = (ITerminalNode)context.GetChild(0);
            var symbolType = terminal.Symbol.Type;

            switch (symbolType)
            {
                case PredicateLexer.And:
                    return new PredicateNode(SymbolType.BooleanOperator, BooleanOperator.And);
                case PredicateLexer.Or:
                    return new PredicateNode(SymbolType.BooleanOperator, BooleanOperator.Or);
            }

            throw new Exception("Unhandled Boolean Operator");
        }

        public override PredicateNode VisitOperator(PredicateParser.OperatorContext context)
        {
            var terminal = (ITerminalNode)context.GetChild(0);
            var symbolType = terminal.Symbol.Type;

            switch (symbolType)
            {
                case PredicateLexer.GreaterThan:
                    return CreateOperatorNode(Operator.GreaterThan);
                case PredicateLexer.GreaterThanEqual:
                    return CreateOperatorNode(Operator.GreaterThanEqual);
                case PredicateLexer.LessThan:
                    return CreateOperatorNode(Operator.LessThan);
                case PredicateLexer.LessThanEqual:
                    return CreateOperatorNode(Operator.LessThanEqual);
                case PredicateLexer.Equal:
                    return CreateOperatorNode(Operator.Equal);
                case PredicateLexer.NotEqual:
                    return CreateOperatorNode(Operator.NotEqual);
                case PredicateLexer.Contains:
                    return CreateOperatorNode(Operator.Contains);
            }

            throw new Exception("Unhandled Operator");
        }

        private PredicateNode CreateOperatorNode(Operator value)
        {
            return new PredicateNode(SymbolType.Operator, value);
        }
    }
}
