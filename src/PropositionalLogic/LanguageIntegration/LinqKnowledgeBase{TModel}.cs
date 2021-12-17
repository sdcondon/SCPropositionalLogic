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

        public LinqKnowledgeBase(IKnowledgeBase innerKnowledgeBase) => this.innerKnowledgeBase = innerKnowledgeBase;

        public void Tell(Expression<Predicate<TModel>> expression)
        {
            innerKnowledgeBase.Tell(SentenceFactory.Create(expression)); 
        }

        public bool Ask(Expression<Predicate<TModel>> expression)
        {
            return innerKnowledgeBase.Ask(SentenceFactory.Create(expression));
        }
    }
}
