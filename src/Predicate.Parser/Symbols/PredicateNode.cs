using System.Collections.Generic;

namespace Predicate.Parser.Symbols
{
    public class PredicateNode
    {
        public PredicateNode(SymbolType type, object value)
        {
            Type = type;
            Value = value;
            Children = new List<PredicateNode>();
        }

        public SymbolType Type { get; set; }

        public object Value { get; set; }

        public IList<PredicateNode> Children { get; }
    }

    public enum SymbolType
    {
        Operand,
        Operator,
        BooleanOperator
    }

    public class Operand
    {
        public Operand(OperandType type, object value)
        {
            OperandType = type;
            Value = value;
        }
        public OperandType OperandType { get; set; }
        public object Value { get; set; }
    }

    public enum OperandType
    {
        String,
        Number,
        Property
    }

    public enum BooleanOperator
    {
        And,
        Or
    }

    public enum Operator
    {
        GreaterThan,
        GreaterThanEqual,
        LessThan,
        LessThanEqual,
        Equal,
        NotEqual,
        Contains
    }
}
