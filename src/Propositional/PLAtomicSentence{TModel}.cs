using System;
using System.Linq.Expressions;

namespace LinqToKnowledgeBase.Propositional
{
    public class PLAtomicSentence<TModel>
    {
        private object equalitySurrogate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PLAtomicSentence{TModel}"/> class.
        /// </summary>
        /// <param name="lambda">The atomic sentence, represented as a lambda expression.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is an atomic sentence. If it were public we'd need to verify that.
        /// TODO-FEATURE: Perhaps could add this in future.
        /// </remarks>
        internal PLAtomicSentence(Expression<Predicate<TModel>> lambda)
        {
            // TODO-ROBUSTNESS: Debug-only verification that it is actually an atomic sentence?
            Lambda = lambda; // Assumed to be an atomic sentence
            new AtomicSentenceExaminer(this).Visit(lambda.Body);
        }

        /// <summary>
        /// Gets a representation of this atomic sentence as a lambda expression.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets a string representation of this atomic sentence.
        /// </summary>
        public string Symbol => Lambda.Body.ToString();

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is PLAtomicSentence<TModel> atomicSentence && equalitySurrogate.Equals(atomicSentence.equalitySurrogate);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return equalitySurrogate.GetHashCode();
        }

        // TODO: no need for an expression visitor unless we end up making the ctor public and verifying that it is actually an atomic sentence,
        // because all we do at the mo is stop at the root node..
        private class AtomicSentenceExaminer : ExpressionVisitor
        {
            private readonly PLAtomicSentence<TModel> owner;

            public AtomicSentenceExaminer(PLAtomicSentence<TModel> owner) => this.owner = owner;

            public override Expression Visit(Expression node)
            {
                owner.equalitySurrogate = node switch
                {
                    MemberExpression memberExpr => memberExpr.Member,
                    _ => throw new NotSupportedException(),
                };

                return node; // no need to explore further, so not base.Visit
            }
        }
    }
}
