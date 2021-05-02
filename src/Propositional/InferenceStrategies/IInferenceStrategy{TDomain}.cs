using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.Propositional.InferenceStrategies
{
    /// <summary>
    /// Interface for inference strategies - objects which can determine if one statement of propositional logic - a query - is entailed by a set of other statements of propositional logic
    /// </summary>
    /// <typeparam name="TDomain">The domain of discourse</typeparam>
    public interface IInferenceStrategy<TDomain>
        where TDomain : new()
    {
        /// <summary>
        /// Determine whether a given query is entailed by a given set of rules.
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="setters"></param>
        /// <param name="query"></param>
        /// <returns>True if the query is entailed by the rules, otherwise false.</returns>
        public bool Entails(
            IEnumerable<Expression<Predicate<TDomain>>> rules,
            IEnumerable<Action<TDomain, bool>> setters,
            Expression<Predicate<TDomain>> query);
    }
}
