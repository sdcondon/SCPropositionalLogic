namespace LinqToKB.PropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// A store of knowledge expressed as sentences of propositional logic.
    /// </summary>
    public interface IKnowledgeBase
    {
        /// <summary>
        /// Tells the knowledge base that a given expression evaluates as true for all models that it will be asked about.
        /// </summary>
        /// <param name="expression">The expression that is true for all models that will be asked about.</param>
        public void Tell(Sentence sentence);

        /// <summary>
        /// Asks the knowledge base if a given expression about the model must evaluate as true, given what it knows.
        /// </summary>
        /// <param name="query">The expression to ask about.</param>
        /// <returns>True if the expression is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Sentence query);
    }
}
