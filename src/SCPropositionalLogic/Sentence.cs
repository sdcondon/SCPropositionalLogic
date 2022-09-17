using SCPropositionalLogic.SentenceFormatting;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a sentence of propositional logic.
    /// </summary>
    public abstract class Sentence
    {
        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
