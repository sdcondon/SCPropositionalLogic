using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of the rules and queries refer to.</typeparam>
    public class ResolutionKnowledgeBase<TModel> : IKnowledgeBase<TModel>
    {
        private readonly List<CNFExpression<TModel>> sentences = new List<CNFExpression<TModel>>();

        /// <inheritdoc />
        public void Tell(Expression<Predicate<TModel>> expression)
        {
            sentences.Add(new CNFExpression<TModel>(expression));
        }

        /// <inheritdoc />
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
