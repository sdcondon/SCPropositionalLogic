using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKnowledgeBase.Propositional.KnowledgeBases
{
    /// <summary>
    /// Knowledge base that uses resolution to answer queries.
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of the rules and queries refer to.</typeparam>
    public class ResolutionKnowledgeBase<TModel> : IKnowledgeBase<TModel>
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

        private readonly List<CNFExpression<TModel>> sentences = new List<CNFExpression<TModel>>();

        /// <summary>
        /// Inform the knowledge base that a given statement about the domain is true for all models that it will be asked about.
        /// </summary>
        /// <param name="sentence">The statement that is always true.</param>
        public void Tell(Expression<Predicate<TModel>> sentence)
        {
            sentences.Add(new CNFExpression<TModel>(sentence));
        }

        /// <summary>
        /// Ask the knowledge base if a given statement about the model is true, given what it knows.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>True if the statement is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Expression<Predicate<TModel>> query)
        {
            // NB: Very raw implementation - there are a number of possible performance improvements that could be made here
            // Will return to give them a go at some point..

            var queryAsCnf = new CNFExpression<TModel>(query);
            var clauses = sentences.Append(queryAsCnf).SelectMany(s => s.Clauses).ToList();
            List<CNFClause<TModel>> newClauses = new List<CNFClause<TModel>>();

            while (true)
            {
                // TODO-PERFORMANCE: While this is how the source book writes the algorithm, its very inefficient -
                // we'll end up resolving the same clauses again and again. Need to improve this (and move this 
                // implementation to the benchmarks project as a baseline).
                foreach (var ci in clauses)
                {
                    foreach (var cj in clauses)
                    {
                        var resolvents = CNFClause<TModel>.Resolve(ci, cj);
                        if (resolvents.IsEmpty)
                        {
                            return true;
                        }

                        newClauses.Add(resolvents);
                    }
                }

                // if new ⊆ clauses then return false // need clause equality..
                clauses.AddRange(newClauses);
            }

            return false;
        }
    }
}
