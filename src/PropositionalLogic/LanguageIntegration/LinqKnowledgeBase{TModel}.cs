using SCPropositionalLogic.KnowledgeBases;
using System;
using System.Linq.Expressions;

namespace SCPropositionalLogic.LanguageIntegration
{
    /// <summary>
    /// Adapter class for <see cref="IKnowledgeBase"/> instances that lets predicate logic sentences be supplied as lambda expressions operating on a type representing the model.
    /// See library documentation for details of how lambdas are converted to sentences.
    /// </summary>
    public class LinqKnowledgeBase<TModel> : ILinqKnowledgeBase<TModel>
    {
        private readonly IKnowledgeBase innerKnowledgeBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqKnowledgeBase{TModel}"/> class.
        /// </summary>
        /// <param name="innerKnowledgeBase">The knowledge base instance to wrap.</param>
        public LinqKnowledgeBase(IKnowledgeBase innerKnowledgeBase) => this.innerKnowledgeBase = innerKnowledgeBase;

        /// <inheritdoc />
        public void Tell(Expression<Predicate<TModel>> expression)
        {
            innerKnowledgeBase.Tell(SentenceFactory.Create(expression)); 
        }

        /// <inheritdoc />
        public bool Ask(Expression<Predicate<TModel>> expression)
        {
            return innerKnowledgeBase.Ask(SentenceFactory.Create(expression));
        }
    }
}
