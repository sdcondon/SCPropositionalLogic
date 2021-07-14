using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.PropositionalLogic.KnowledgeBases
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
    public class TruthTableKnowledgeBase<TModel> : IKnowledgeBase<TModel>
        where TModel : class, new()
    {
        public static readonly IDictionary<PropertyInfo, Action<TModel, bool>> symbols; // TODO-MAINTAINABILITY: Factor out into a Symbol class.
        public readonly List<Expression<Predicate<TModel>>> sentences = new List<Expression<Predicate<TModel>>>();

        static TruthTableKnowledgeBase()
        {
            // There are almost certainly a tonne of better (still type-safe) ways to do this, 
            // but this will suffice for getting started.
            // One approach worthy of investigation is to use a source generator to generate this at compile time (or
            // throw a compile error).
            // Ultimately don't want to force the type to have settable props. How does Moq work - perhaps use Reflection.Emit?
            // (Or do all analysis with Linq expression visitors rather than actual invocation - though the side
            // benefits of using LINQ start to dissipate then - and suspect it would be a fair bit slower)
            symbols = GetSymbols();
        }

        /// <summary>
        /// Tells the knowledge base that a given expression evaluates as true for all models that it will be asked about.
        /// </summary>
        /// <param name="expression">The expression that is always true.</param>
        public void Tell(Expression<Predicate<TModel>> sentence) => sentences.Add(sentence);

        /// <summary>
        /// Asks the knowledge base if a given expression about the model must evaluate as true, given what it knows.
        /// </summary>
        /// <param name="expression">The expression to ask about.</param>
        /// <returns>True if the expression is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Expression<Predicate<TModel>> query)
        {
            /*
             * Pseudocode from 'Artificial Intelligence: A Modern Approach, 3rd Ed' section 7.4, figure 7.10:
             * 
            function TT-ENTAILS?(KB, α) returns true or false
                inputs: 
                  KB, the knowledge base, a sentence in propositional logic
                  α, the query, a sentence in propositional logic  
                symbols ← a list of the proposition symbols in KB and α
                return TT-CHECK-ALL(KB,α, symbols, {})

            function TT-CHECK-ALL(KB, α, symbols, model) returns true or false
                if EMPTY?(symbols) then
                    if PL-TRUE?(KB, model) then return PL-TRUE?(α, model)
                    else return true// when KB is false, always return true
                else do
                    P ← FIRST(symbols)
                    rest ← REST(symbols) 
                    return (TT-CHECK-ALL(KB,α, rest, model ∪ {P = true})  and  TT-CHECK-ALL(KB,α, rest, model ∪ {P = false}))
            */

            var m = new TModel();

            // TODO-MAINTAINABILITY: Bleugh, recursion. Should use our own stack instead.
            bool CheckAll(TModel model, IEnumerable<Action<TModel, bool>> setters)
            {
                var setter = setters.FirstOrDefault();
                if (setter == null)
                {
                    // TODO-PERFORMANCE: Haven't verified, but presumably compilation result of lambda expression compilation is cached..
                    return !sentences.All(r => r.Compile()(model)) || query.Compile()(model);
                }

                var rest = setters.Skip(1); // TODO-PERFORMANCE: inefficient nested enums. Will do for now - to tweak later so that this is (readable and) efficient..

                setter(m, true);
                if (!CheckAll(m, rest))
                {
                    return false;
                }

                setter(m, false);
                return CheckAll(m, rest);
            }

            return CheckAll(m, symbols.Values);
        }

        private static IDictionary<PropertyInfo, Action<TModel, bool>> GetSymbols()
        {
            var dictionary = new Dictionary<PropertyInfo, Action<TModel, bool>>();

            var objectParam = Expression.Parameter(typeof(TModel));
            var valueParam = Expression.Parameter(typeof(bool));

            foreach (var prop in typeof(TModel).GetRuntimeProperties())
            {
                var setterPropValue = Expression.Property(objectParam, prop.SetMethod);
                var propAssignment = Expression.Assign(setterPropValue, valueParam);
                // NB: this lambda won't work for value types (can't do pass-by-ref in LINQ expressions), hence the type constraint.
                dictionary[prop] = Expression.Lambda<Action<TModel, bool>>(propAssignment, objectParam, valueParam).Compile();
            }

            return dictionary;
        }
    }
}
