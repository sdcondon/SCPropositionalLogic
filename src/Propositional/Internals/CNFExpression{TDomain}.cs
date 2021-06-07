using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.Propositional.Internals
{
    /// <summary>
    /// Representation of a predicate lambda expression in conjunctive normal form.
    /// </summary>
    /// <typeparam name="TDomain">The domain of discourse.</typeparam>
    public class CNFExpression<TDomain>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFExpression{TDomain}"/> class, implicitly converting the provided lambda to CNF in the process.
        /// </summary>
        /// <param name="lambda">The predicate to represent.</param>
        public CNFExpression(Expression<Predicate<TDomain>> lambda)
        {
            Lambda = Expression.Lambda<Predicate<TDomain>>(new CNFConverter().VisitAndConvert(lambda.Body, null), lambda.Parameters); // might be better if the converter could deal with a lambda directly, not just its body..
            var clauses = new List<CNFClause<TDomain>>();
            new ClauseBuilder(this, clauses).Visit(Lambda.Body);
            Clauses = clauses.AsReadOnly();
        }

        /// <summary>
        /// Gets the expression (in conjunctive normal form) as a lambda.
        /// </summary>
        public Expression<Predicate<TDomain>> Lambda { get; }

        /// <summary>
        /// Gets the collection of clauses that comprise this expression.
        /// </summary>
        public IReadOnlyCollection<CNFClause<TDomain>> Clauses { get; }

        /// <summary>
        /// Expression visitor that constructs a set of <see cref="CNFClause{TDomain}"/> objects from a lambda in CNF.
        /// </summary>
        private class ClauseBuilder : ExpressionVisitor
        {
            private readonly CNFExpression<TDomain> owner;
            private readonly List<CNFClause<TDomain>> clauses;

            public ClauseBuilder(CNFExpression<TDomain> owner, List<CNFClause<TDomain>> clauses) => (this.owner, this.clauses) = (owner, clauses);

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
                    // So, we need to create a lambda here - easy enough - we just re-use the parameters from the
                    // overall expression.
                    clauses.Add(new CNFClause<TDomain>(Expression.Lambda<Predicate<TDomain>>(node, owner.Lambda.Parameters)));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor, above,
                    // does so to figure out the details of the clause). So we can just return node rather than invoking base.Visit. 
                    return node;
                }
            }
        }
    }
}
