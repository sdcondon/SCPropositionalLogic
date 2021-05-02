using LinqToKB.Propositional.InferenceStrategies;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.Propositional
{
    /// <summary>
    /// A store of knowledge expressed as statements of propositional logic (in turn expressed as LINQ expressions).
    /// </summary>
    /// <typeparam name="TDomain">
    /// The domain of discourse. NB: Must contain only boolean-valued properties, since that is all the KB can handle.
    /// An exception will be thrown during type initialization otherwise.
    /// </typeparam>
    public class KnowledgeBase<TDomain>
        where TDomain : class, new()
    {
        public static readonly IDictionary<PropertyInfo, Action<TDomain, bool>> symbols; // TODO: Factor out into a Symbol class.
        public readonly List<Expression<Predicate<TDomain>>> sentences = new List<Expression<Predicate<TDomain>>>();

        static KnowledgeBase()
        {
            // There are almost certainly a tonne of better (still type-safe) ways to do this, 
            // but this will suffice for getting started.
            // One approach worthy of investigation is to use a source generator to generate this at compile time (or
            // throw a compile error).
            // Ultimately don't want to force the type to have settable props. How does Moq work - Reflection.Emit?
            // (Or do all analysis with Linq expression visitors rather than actual invocation - though the side
            // benefits of using LINQ start to dissipate then - and suspect it would be a fair bit slower)
            symbols = GetSymbols();
        }

        /// <summary>
        /// Gets or sets the inference strategy that will be used to fulfill <see cref="Ask"/> invocations.
        /// </summary>
        public IInferenceStrategy<TDomain> InferenceStrategy { get; set; } = new TruthTableInferenceStrategy<TDomain>();

        /// <summary>
        /// Inform the knowledge base that a given statement about the domain is always true.
        /// </summary>
        /// <param name="sentence">The statement that is always true.</param>
        public void Tell(Expression<Predicate<TDomain>> sentence) => sentences.Add(sentence);

        /// <summary>
        /// Ask the knowledge base if a given statement about the domain is true, given what it knows.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>True if the statement is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Expression<Predicate<TDomain>> query) => InferenceStrategy.Entails(sentences, symbols.Values, query);

        private static IDictionary<PropertyInfo, Action<TDomain, bool>> GetSymbols()
        {
            var dictionary = new Dictionary<PropertyInfo, Action<TDomain, bool>>();

            var objectParam = Expression.Parameter(typeof(TDomain));
            var valueParam = Expression.Parameter(typeof(bool));

            foreach (var prop in typeof(TDomain).GetRuntimeProperties())
            {
                var setterPropValue = Expression.Property(objectParam, prop.SetMethod);
                var propAssignment = Expression.Assign(setterPropValue, valueParam);
                // NB: this lambda won't work for value types (can't do pass-by-ref in LINQ expressions), hence the type constraint.
                dictionary[prop] = Expression.Lambda<Action<TDomain, bool>>(propAssignment, objectParam, valueParam).Compile();
            }

            return dictionary;
        }
    }
}
