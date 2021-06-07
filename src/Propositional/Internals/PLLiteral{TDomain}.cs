using System;
using System.Linq.Expressions;

namespace LinqToKB.Propositional.Internals
{
    /// <summary>
    /// Representation of a predicate logic literal. That is, an atomic sentence or a negated atomic sentence.
    /// </summary>
    public class PLLiteral<TDomain>
    {
        private bool isNegated;
        private Expression<Predicate<TDomain>> atomicSentence;

        /// <summary>
        /// Initialises a new instance of the <see cref="PLLiteral{TDomain}"/> class.
        /// </summary>
        /// <param name="lambda"></param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is a literal. If it were public we'd need to verify that.
        /// </remarks>
        internal PLLiteral(Expression<Predicate<TDomain>> lambda)
        {
            Lambda = lambda; // Assumed to be a disjunction of literals
            new LiteralExaminer(this).Visit(lambda.Body);
        }

        /// <summary>
        /// Gets a representation of this literal as a lambda expression.
        /// </summary>
        public Expression<Predicate<TDomain>> Lambda { get; }

        /// <summary>
        /// Gets a value indicating whether this literal is a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsNegated => isNegated;

        /// <summary>
        /// Gets a representation of the atomic sentence within this literal as a lambda expression.
        /// </summary>
        public Expression<Predicate<TDomain>> AtomicSentence => atomicSentence;

        /// <summary>
        /// Gets a string representation of the atomic sentence within this literal.
        /// </summary>
        public string AtomicSentenceSymbol => AtomicSentence.Body.ToString();

        private class LiteralExaminer: ExpressionVisitor
        {
            private readonly PLLiteral<TDomain> owner;

            public LiteralExaminer(PLLiteral<TDomain> owner) => this.owner = owner;

            public override Expression Visit(Expression node)
            {
                if (node is UnaryExpression not && not.NodeType == ExpressionType.IsFalse)
                {
                    // Since we have assumed that the provided lambda is a literal, we know that a Not expression (if there is one) will be at the root..
                    owner.isNegated = true;
                    return base.Visit(node);
                }
                else
                {
                    owner.atomicSentence = Expression.Lambda<Predicate<TDomain>>(node, owner.Lambda.Parameters);
                    return node; // no need to explore further, so not base.Visit
                }
            }
        }
    }
}
