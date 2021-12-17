using SCPropositionalLogic.SentenceManipulation;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a sentence of propositional logic.
    /// </summary>
    public abstract class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        //// Shorthand factory methods. Not sure if I like these here or not. Separating them into their own class would make it easier to include guidance
        //// (notably, how best to create your own properties for propositions).
        public static Sentence And(Sentence left, Sentence right) => new Conjunction(left, right);
        public static Sentence Or(Sentence left, Sentence right) => new Disjunction(left, right);
        public static Sentence If(Sentence antecedent, Sentence consequent) => new Implication(antecedent, consequent);
        public static Sentence Iff(Sentence left, Sentence right) => new Equivalence(left, right);
        public static Sentence Not(Sentence sentence) => new Negation(sentence);

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
