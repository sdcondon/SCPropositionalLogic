using System;

namespace LinqToKB.PropositionalLogic.SentenceManipulation.ConjunctiveNormalForm
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
            Sentence = sentence;

            if (sentence is Negation negation)
            {
                IsNegated = true;
                sentence = negation.Sentence;
            }

            Proposition = sentence as Proposition ?? throw new ArgumentException("Sentence must be a literal", nameof(sentence));
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

            if (isNegated)
            {
                Sentence = new Negation(proposition);
            }
            else
            {
                Sentence = proposition;
            }
        }

        /// <summary>
        /// Gets the actual <see cref="Sentence"/> that underlies this representation.
        /// </summary>
        public Sentence Sentence { get; }

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
        public override string ToString() => Sentence.ToString();

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is CNFLiteral literal && literal.Sentence.Equals(Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }
}