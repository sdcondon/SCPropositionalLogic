using LinqToKB.PropositionalLogic.KnowledgeBases;
using System;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic.LanguageIntegration
{
    public interface ILinqKnowledgeBase<TModel>
    {
        void Tell(Expression<Predicate<TModel>> expression);

        public bool Ask(Expression<Predicate<TModel>> expression);
    }
}
