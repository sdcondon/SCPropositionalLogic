using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.Propositional.InferenceStrategies
{
    /// <remarks>
    /// Will need a class for CNF expressions (not just an expressionvisitor..),
    /// so that we can examine individual clauses more easily, check if theyre
    /// Horn clauses, etc.
    /// </remarks>
    public class ResolutionInferenceStrategy<TDomain> : IInferenceStrategy<TDomain>
        where TDomain : class, new()
    {
        /*
         * Figure 7.12 A simple resolution algorithm for propositional logic. The function PL-RESOLVE returns the set of all possible clauses obtained by resolving its two inputs. 
         * Russell, Stuart; Norvig, Peter. Artificial Intelligence: A Modern Approach, Global Edition (p. 255). Pearson Education Limited. Kindle Edition. 
         * 
        function PL-RESOLUTION(KB,α) returns true or false
            inputs:
                KB, the knowledge base, a sentence in propositional logic
                α, the query, a sentence in propositional logic
        
            clauses ← the set of clauses in the CNF representation of KB ∧ ¬α
            new ← {}
        
            loop do
                for each pair of clauses Ci, Cj in clauses do
                    resolvents ← PL-RESOLVE(Ci, Cj)
                    if resolvents contains the empty clause then return true
                    new ← new ∪ resolvents
                if new ⊆ clauses then return false
                clauses ← clauses ∪new 
         */
        public bool Entails(IEnumerable<Expression<Predicate<TDomain>>> rules, IEnumerable<Action<TDomain, bool>> setters, Expression<Predicate<TDomain>> query)
        {
            throw new NotImplementedException();
        }
    }
}
