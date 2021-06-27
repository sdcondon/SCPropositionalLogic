using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKnowledgeBase.Propositional
{
    /// <summary>
    /// Representation of an individual clause of a predicate expression in conjunctive normal form - that is, a disjunction of literals.
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of this clause refer to.</typeparam>
    public class CNFClause<TModel>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause{TModel}"/> class.
        /// </summary>
        /// <param name="lambda">The clause, represented as a lambda expression.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is a disjunction of literals. If it were public we'd need to verify that.
        /// </remarks>
        internal CNFClause(Expression<Predicate<TModel>> lambda)
        {
            Lambda = lambda; // Assumed to be a disjunction of literals
            var literals = new List<PLLiteral<TModel>>();
            new ClauseExaminer(this, literals).Visit(lambda.Body);
            Literals = literals.AsReadOnly();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause{TModel}"/> class.
        /// </summary>
        /// <param name="lambda">The set of literals to be included in the clause.</param>
        /// <remarks>
        /// TODO: Hmm, perhaps could be public.
        /// </remarks>
        internal CNFClause(IEnumerable<PLLiteral<TModel>> literals)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a representation of this clause as a lambda expression.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets the collection of literals that comprise this clause.
        /// </summary>
        public IReadOnlyCollection<PLLiteral<TModel>> Literals { get; }

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

        /// <summary>
        /// Gets a value indicating whether this is an empty clause (that implicitly evaluates to false). Can occur as a result of resolution.
        /// </summary>
        public bool IsEmpty => Literals.Count() == 0;

        /// <summary>
        /// Resolves to clauses to create a new clause - eliminating any mutually-negating literals (and any duplicates) in the process.
        /// </summary>
        /// <param name="clause1"></param>
        /// <param name="clause2"></param>
        /// <returns>A new clause.</returns>
        public static CNFClause<TModel> Resolve(CNFClause<TModel> clause1, CNFClause<TModel> clause2)
        {
            var literals = new List<PLLiteral<TModel>>();

            // TODO-PERFORMANCE: There are myriad ways to improve performance here. Sorting literals in clauses is one..
            foreach (var l1 in clause1.Literals)
            {
                foreach (var l2 in clause2.Literals)
                {
                    // TODO
                }
            }

            return new CNFClause<TModel>(literals);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            // TODO: has the same literals
            return base.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // TODO: order-agnostic comibination of literal hashcodes.
            return base.GetHashCode();
        }

        private class ClauseExaminer : ExpressionVisitor
        {
            private readonly CNFClause<TModel> owner;
            private readonly List<PLLiteral<TModel>> literals;

            public ClauseExaminer(CNFClause<TModel> owner, List<PLLiteral<TModel>> literals) => (this.owner, this.literals) = (owner, literals);

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
                    literals.Add(new PLLiteral<TModel>(Expression.Lambda<Predicate<TModel>>(node, owner.Lambda.Parameters)));

                    // We don't need to look any further down the tree for the purposes of this class (though the PLLiteral ctor, above,
                    // does so to figure out the details of the literal). So we can just return node rather than invoking base.Visit. 
                    return node;
                }
            }
        }
    }
}
