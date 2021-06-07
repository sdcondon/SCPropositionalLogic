using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKB.Propositional.Internals
{
    /// <summary>
    /// Representation of an individual clause of a predicate expression in conjunctive normal form - that is, a disjunction of literals.
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    public class CNFClause<TDomain>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause{TDomain}"/> class.
        /// </summary>
        /// <param name="lambda"></param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is a disjunction of literals. If it were public we'd need to verify that.
        /// </remarks>
        internal CNFClause(Expression<Predicate<TDomain>> lambda)
        {
            Lambda = lambda; // Assumed to be a disjunction of literals
            var literals = new List<PLLiteral<TDomain>>();
            new ClauseExaminer(this, literals).Visit(lambda.Body);
            Literals = literals.AsReadOnly();
        }

        /// <summary>
        /// Gets a representation of this clause as a lambda expression.
        /// </summary>
        public Expression<Predicate<TDomain>> Lambda { get; }

        /// <summary>
        /// Gets the collection of literals that comprise this clause.
        /// </summary>
        public IReadOnlyCollection<PLLiteral<TDomain>> Literals { get; }

        /// <summary>
        /// Gets a value indicating whether this is a Horn clause - that is, whether at most one of its literals is positive.
        /// </summary>
        public bool IsHornClause => Literals.Count(l => !l.IsNegated) <= 1;

        /// <summary>
        /// Gets a value indicating whether this is a definite clause - that is, whether exactly one of its literals is positive.
        /// </summary>
        public bool IsDefiniteClause => Literals.Count(l => !l.IsNegated) == 1;

        /// <summary>
        /// Gets a value indicating whether this is a goal clause - that is, whether none of its literals is positive.
        /// </summary>
        public bool IsGoalClause => Literals.Count(l => !l.IsNegated) == 0;

        /// <summary>
        /// Gets a value indicating whether this is a unit clause - that is, whetherit contains exactly one literal.
        /// </summary>
        public bool IsUnitClause => Literals.Count() == 1;

        private class ClauseExaminer : ExpressionVisitor
        {
            private readonly CNFClause<TDomain> owner;
            private readonly List<PLLiteral<TDomain>> literals;

            public ClauseExaminer(CNFClause<TDomain> owner, List<PLLiteral<TDomain>> literals) => (this.owner, this.literals) = (owner, literals);

            public override Expression Visit(Expression node)
            {
                if (node is BinaryExpression orElse && orElse.NodeType == ExpressionType.OrElse)
                {
                    // The expression is guaranteed to be  - so the root down until the individual clauses will all be OrElse - we just skip past those.
                    return base.Visit(node);
                }
                else
                {
                    // We've hit a literal.
                    // NB: PLLiteral accepts a lambda - not just an Expression. This is for maximum flexibility,
                    // so that e.g. literals can be evaluated individually should we so wish. Performance hit to build, but meh..
                    // So, we need to create a lambda here - easy enough - we just re-use the parameters from the
                    // overall clause.
                    literals.Add(new PLLiteral<TDomain>(Expression.Lambda<Predicate<TDomain>>(node, owner.Lambda.Parameters)));

                    // We don't need to look any further down the tree for the purposes of this class (though the PLLiteral ctor, above,
                    // does so to figure out the details of the literal). So we can just return node rather than invoking base.Visit. 
                    return node;
                }
            }
        }
    }
}
