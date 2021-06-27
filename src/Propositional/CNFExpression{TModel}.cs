using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKnowledgeBase.Propositional
{
    /// <summary>
    /// Representation of a predicate lambda expression in conjunctive normal form (CNF).
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of this expression refer to.</typeparam>
    public class CNFExpression<TModel>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFExpression{TModel}"/> class, implicitly converting the provided lambda to CNF in the process.
        /// </summary>
        /// <param name="lambda">The predicate to represent.</param>
        public CNFExpression(Expression<Predicate<TModel>> lambda)
        {
            Lambda = CNFConverter.ConvertToCNF(lambda);
            var clauses = new List<CNFClause<TModel>>();
            new ClauseBuilder(this, clauses).Visit(Lambda.Body);
            Clauses = clauses.AsReadOnly();
        }

        /// <summary>
        /// Gets the expression (in conjunctive normal form) as a lambda.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets the collection of clauses that comprise this expression.
        /// </summary>
        public IReadOnlyCollection<CNFClause<TModel>> Clauses { get; }

        /// <summary>
        /// Expression visitor that constructs a set of <see cref="CNFClause{TModel}"/> objects from a lambda in CNF.
        /// </summary>
        private class ClauseBuilder : ExpressionVisitor
        {
            private readonly CNFExpression<TModel> owner;
            private readonly List<CNFClause<TModel>> clauses;

            public ClauseBuilder(CNFExpression<TModel> owner, List<CNFClause<TModel>> clauses) => (this.owner, this.clauses) = (owner, clauses);

            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                if (node is BinaryExpression andAlso && andAlso.NodeType == ExpressionType.AndAlso)
                {
                    // The expression is already in CNF - so the root down until the individual clauses will all be AndAlso - we just skip past those.
                    return base.Visit(node);
                }
                else
                {
                    // We've hit a clause.
                    // NB: CNFClause accepts a lambda - not just an Expression. This is for maximum flexibility,
                    // so that e.g. clauses can be evaluated individually should we so wish. Performance hit to build, but meh..
                    // So, we need to create a lambda here - easy enough - we just point at the parameters from the
                    // overall expression (after all, this is just a sub-expression).
                    clauses.Add(new CNFClause<TModel>(Expression.Lambda<Predicate<TModel>>(node, owner.Lambda.Parameters)));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor, above,
                    // does so to figure out the details of the clause). So we can just return node rather than invoking base.Visit. 
                    return node;
                }
            }
        }
    }
}
