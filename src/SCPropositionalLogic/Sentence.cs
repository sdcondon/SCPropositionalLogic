using SCPropositionalLogic.SentenceManipulation;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a sentence of propositional logic.
    /// </summary>
    public abstract partial class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
