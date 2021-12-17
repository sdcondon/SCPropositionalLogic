using SCPropositionalLogic.KnowledgeBases;
using System;
using System.Linq.Expressions;

namespace SCPropositionalLogic.LanguageIntegration
{
    public interface ILinqKnowledgeBase<TModel>
    {
        void Tell(Expression<Predicate<TModel>> expression);

        public bool Ask(Expression<Predicate<TModel>> expression);
    }
}
