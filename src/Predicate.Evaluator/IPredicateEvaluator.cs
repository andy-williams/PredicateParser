using System.Threading.Tasks;

namespace Predicate.Evaluator
{
    public interface IPredicateEvaluator<T>
    {
        Task<PredicateEvaluationOperation<T>> Evaluate(string predicateExpression);
    }

    public class PredicateEvaluationOperation<T>
    {
        public PredicateEvaluationOperation(T result)
        {
            Result = result;
        }

        public bool IsSuccessful => Result != null;

        public T Result { get; }
    }
}
