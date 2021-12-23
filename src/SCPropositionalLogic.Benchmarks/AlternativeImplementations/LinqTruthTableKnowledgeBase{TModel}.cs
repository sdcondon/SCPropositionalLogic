using SCPropositionalLogic.LanguageIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SCPropositionalLogic.Benchmarks.Alternatives
{
    /// <summary>
    /// Knowledge base that satisfies queries by enumerating all possible models, returning true if and only if the query holds
    /// true in all models in which all of the rules hold true. Obviously incredibly slow for non-trivial models - 
    /// intended only for demonstration purposes.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type that the literals of the rules and queries refer to. NB: Must contain only boolean-valued properties, since that is all the KB can handle.
    /// An exception will be thrown during type initialization otherwise.
    /// </typeparam>
    public class LinqTruthTableKnowledgeBase<TModel> : ILinqKnowledgeBase<TModel>
        where TModel : class, new()
    {
        public static readonly List<Action<TModel, bool>> propertySetters;
        public readonly List<Predicate<TModel>> sentenceSatisfactionChecks = new List<Predicate<TModel>>();

        static LinqTruthTableKnowledgeBase()
        {
            // There are almost certainly a tonne of better (still type-safe) ways to do this, 
            // but this will suffice for getting started.
            // Ultimately don't want to force the type to have settable props. How does Moq work - perhaps use Reflection.Emit?
            // (Or do all analysis with Linq expression visitors rather than actual invocation - though the side
            // benefits of using LINQ start to dissipate then - and suspect it would be a fair bit slower)
            propertySetters = new List<Action<TModel, bool>>();

            var objectParam = Expression.Parameter(typeof(TModel));
            var valueParam = Expression.Parameter(typeof(bool));

            foreach (var prop in typeof(TModel).GetRuntimeProperties())
            {
                var propAssignment = Expression.Assign(
                    Expression.Property(objectParam, prop.SetMethod),
                    valueParam);

                // NB: this lambda won't work for value types (can't do pass-by-ref in LINQ expressions), hence the 'class' type constraint.
                propertySetters.Add(Expression.Lambda<Action<TModel, bool>>(propAssignment, objectParam, valueParam).Compile());
            }
        }

        /// <inheritdoc />
        public void Tell(Expression<Predicate<TModel>> expression) => sentenceSatisfactionChecks.Add(expression.Compile());

        /// <inheritdoc />
        public bool Ask(Expression<Predicate<TModel>> query)
        {
            // One thing to note here is that for sentences in Tell, and the query here, all we do is immediately compile them.
            // As such, this class would be quicker if we just accepted Predicate<TModel>. The only reason we don't is for this
            // to be a more interesting performance comparison for the real implementation (because its closer to the implementation).
            var querySatisfactionCheck = query.Compile();

            bool CheckAll(TModel model, IEnumerable<Action<TModel, bool>> setters)
            {
                var setter = setters.FirstOrDefault();
                if (setter == null)
                {
                    return !sentenceSatisfactionChecks.All(check => check(model)) || querySatisfactionCheck(model);
                }

                var rest = setters.Skip(1); // TODO-PERFORMANCE: inefficient nested enums

                setter(model, true);
                if (!CheckAll(model, rest))
                {
                    return false;
                }

                setter(model, false);
                return CheckAll(model, rest);
            }

            return CheckAll(new TModel(), propertySetters);
        }
    }
}
