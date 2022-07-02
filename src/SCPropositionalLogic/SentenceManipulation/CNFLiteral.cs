using System;

namespace SCPropositionalLogic.SentenceManipulation
{
    /// <summary>
    /// Representation of a literal of propositional logic. That is, an atomic sentence or a negated atomic sentence.
    /// </summary>
    /// <remarks>
    /// Yes, literals are a meaningful notion regardless of CNF, but we only use THIS type within our CNF representation. For now then, its called CNFLiteral and resides in this namespace.
    /// </remarks>
    public class CNFLiteral
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="lambda">The literal, represented as a sentence.</param>
        public CNFLiteral(Sentence sentence)
        {
            if (sentence is Negation negation)
            {
                IsNegated = true;
                sentence = negation.Sentence;
            }

            if (sentence is Proposition proposition)
            {
                Proposition = proposition;
            }
            else
            {
                throw new ArgumentException($"Provided sentence must be either a proposition or a negated proposition. {sentence} is neither.");
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="proposition">The atomic sentence to which this literal refers.</param>
        /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
        public CNFLiteral(Proposition proposition, bool isNegated)
        {
            Proposition = proposition;
            IsNegated = isNegated;
        }

        /// <summary>
        /// Gets a value indicating whether this literal is a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsNegated { get; }

        /// <summary>
        /// Gets a value indicating whether this literal is not a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsPositive => !IsNegated;

        /// <summary>
        /// Gets the underlying atomic sentence of this literal.
        /// </summary>
        public Proposition Proposition { get; }

        /// <summary>
        /// Constructs and returns a literal that is the negation of this one.
        /// </summary>
        /// <returns>A literal that is the negation of this one.</returns>
        public CNFLiteral Negate() => new CNFLiteral(Proposition, !IsNegated);

        /// <inheritdoc />
        public override string ToString() => $"{(IsNegated ? "¬" : "")}{Proposition}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CNFLiteral literal
                && literal.Proposition.Equals(Proposition)
                && literal.IsNegated.Equals(IsNegated);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Proposition, IsNegated);

        /// <summary>
        /// Defines the (implicit) conversion of a <see cref="Sentence"/> instance to a <see cref="CNFLiteral"/>.
        /// </summary>
        /// <param name="sentence">The sentence to convert.</param>
        /// <remarks>
        /// NB: This conversion is explicit because it can fail (if the sentence isn't actually a literal).
        /// </remarks>
        public static explicit operator CNFLiteral(Sentence sentence) => new CNFLiteral(sentence);

        /// <summary>
        /// Defines the (implicit) conversion of a <see cref="Proposition"/> instance to a <see cref="CNFLiteral"/>.
        /// </summary>
        /// <param name="sentence">The proposition to convert.</param>
        /// <remarks>
        /// NB: This conversion is implicit because it is always valid.
        /// </remarks>
        public static implicit operator CNFLiteral(Proposition proposition) => new CNFLiteral(proposition);
    }
}