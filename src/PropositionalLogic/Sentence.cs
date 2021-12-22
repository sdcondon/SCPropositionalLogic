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

        /// <summary>
        /// Shorthand factory method for a new <see cref="Conjunction"/> instance.
        /// </summary>
        /// <param name="left">The left-hand operand of the conjunction.</param>
        /// <param name="right">The right-hand operand of the conjunction.</param>
        /// <returns>A new <see cref="Conjunction"/> instance.</returns>
        public static Sentence And(Sentence left, Sentence right) => new Conjunction(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Disjunction"/> instance.
        /// </summary>
        /// <param name="left">The left-hand operand of the disjunction.</param>
        /// <param name="right">The right-hand operand of the disjunction.</param>
        /// <returns>A new <see cref="Disjunction"/> instance.</returns>
        public static Sentence Or(Sentence left, Sentence right) => new Disjunction(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Implication"/> instance.
        /// </summary>
        /// <param name="left">The antecedent sentence of the implication.</param>
        /// <param name="right">The consequent sentence of the implication.</param>
        /// <returns>A new <see cref="Implication"/> instance.</returns>
        public static Sentence If(Sentence antecedent, Sentence consequent) => new Implication(antecedent, consequent);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Equivalence"/> instance.
        /// </summary>
        /// <param name="left">The left-hand operand of the equivalence.</param>
        /// <param name="right">The right-hand operand of the equivalence.</param>
        /// <returns>A new <see cref="Equivalence"/> instance.</returns>
        public static Sentence Iff(Sentence left, Sentence right) => new Equivalence(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Negation"/> instance.
        /// </summary>
        /// <param name="sentence">The negated sentence.</param>
        /// <returns>A new <see cref="Negation"/> instance.</returns>
        public static Sentence Not(Sentence sentence) => new Negation(sentence);

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
