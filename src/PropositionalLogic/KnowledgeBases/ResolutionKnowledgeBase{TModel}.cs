using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// Knowledge base that uses resolution to answer queries.
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of the rules and queries refer to.</typeparam>
    /// <remarks>
    /// The "purest" form of the algorithm according to the source material in question - but that
    /// makes it rather inefficient. Not really useable in a real scenario.
    /// </remarks>
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
        /// Tells the knowledge base that a given expression evaluates as true for all models that it will be asked about.
        /// </summary>
        /// <param name="expression">The expression that is always true.</param>
        public void Tell(Expression<Predicate<TModel>> sentence)
        {
            sentences.Add(new CNFExpression<TModel>(sentence));
        }

        /// <summary>
        /// Asks the knowledge base if a given expression about the model must evaluate as true, given what it knows.
        /// </summary>
        /// <param name="expression">The expression to ask about.</param>
        /// <returns>True if the expression is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Expression<Predicate<TModel>> query)
        {
            var negationOfQuery = Expression.Lambda<Predicate<TModel>>(Expression.Not(query.Body), query.Parameters);
            var negationOfQueryAsCnf = new CNFExpression<TModel>(negationOfQuery);
            var clauses = sentences.Append(negationOfQueryAsCnf).SelectMany(s => s.Clauses).ToHashSet();
            var queue = new Queue<(CNFClause<TModel>, CNFClause<TModel>)>();

            foreach (var ci in clauses)
            {
                foreach (var cj in clauses)
                {
                    queue.Enqueue((ci, cj));
                }
            }

            while (queue.Count > 0)
            {
                var (ci, cj) = queue.Dequeue();
                var resolvents = CNFClause<TModel>.Resolve(ci, cj);

                foreach (var resolvent in resolvents)
                {
                    if (resolvent.Equals(CNFClause<TModel>.Empty))
                    {
                        return true;
                    }

                    if (!clauses.Contains(resolvent))
                    {
                        foreach (var clause in clauses)
                        {
                            queue.Enqueue((clause, resolvent));
                        }

                        clauses.Add(resolvent);
                    }
                }
            }

            return false;
        }
    }
}
