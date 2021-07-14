using System;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// A store of knowledge expressed as statements of propositional logic (in turn expressed as LINQ expressions).
    /// </summary>
    /// <typeparam name="TModel">
    /// The type that the expressions passed to this class refer to.
    /// </typeparam>
    public interface IKnowledgeBase<TModel>
    {
        /// <summary>
        /// Tells the knowledge base that a given expression evaluates as true for all models that it will be asked about.
        /// </summary>
        /// <param name="expression">The expression that is always true.</param>
        public void Tell(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// Asks the knowledge base if a given expression about the model must evaluate as true, given what it knows.
        /// </summary>
        /// <param name="expression">The expression to ask about.</param>
        /// <returns>True if the expression is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Expression<Predicate<TModel>> expression);
    }
}
