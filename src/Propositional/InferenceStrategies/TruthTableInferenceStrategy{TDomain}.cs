using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKB.Propositional.InferenceStrategies
{
    /// <summary>
    /// Inference strategy that enumerates all possible models, and returns true if and only if the query holds
    /// true in all models in which the KB holds true. Obviously incredibly slow for non-trivial domains.
    /// </summary>
    /// <typeparam name="TDomain">The domain of discourse.</typeparam>
    public class TruthTableInferenceStrategy<TDomain> : IInferenceStrategy<TDomain>
        where TDomain : new()
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

        /// <inheritdoc />
        public bool Entails(
            IEnumerable<Expression<Predicate<TDomain>>> rules,
            IEnumerable<Action<TDomain, bool>> setters,
            Expression<Predicate<TDomain>> query)
        {
            var m = new TDomain();

            // TODO: Bleugh, recursion. Should use our own stack instead.
            bool CheckAll(TDomain model, IEnumerable<Action<TDomain, bool>> setters)
            {
                var setter = setters.FirstOrDefault();
                if (setter == null)
                {
                    // TODO-PERFORMANCE: Haven't verified, but presumably compilation result of lambda expression compilation is cached..
                    return rules.All(r => r.Compile()(model)) ? query.Compile()(model) : true;
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

            return CheckAll(m, setters);
        }
    }
}
