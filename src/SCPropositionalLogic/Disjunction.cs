using System;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a disjunction sentence of propositional logic. In typical PL syntax, this is written as:
    /// <code>{sentence} ∨ {sentence}</code>
    /// </summary>
    public class Disjunction : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Disjunction"/> class.
        /// </summary>
        /// <param name="left">The left side of the disjunction.</param>
        /// <param name="right">The right side of the disjunction.</param>
        public Disjunction(Sentence left, Sentence right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the disjunction.
        /// </summary>
        public Sentence Left { get; }

        /// <summary>
        /// Gets the right side of the disjunction.
        /// </summary>
        public Sentence Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Disjunction otherDisjunction))
            {
                return false;
            }

            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);
            (var otherLow, var otherHigh) = otherDisjunction.Left.GetHashCode() < otherDisjunction.Right.GetHashCode() ? (otherDisjunction.Left, otherDisjunction.Right) : (otherDisjunction.Right, otherDisjunction.Left);

            return low.Equals(otherLow) && high.Equals(otherHigh);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            (var low, var high) = Left.GetHashCode() < Right.GetHashCode() ? (Left, Right) : (Right, Left);

            return HashCode.Combine(low, high);
        }
    }
}
