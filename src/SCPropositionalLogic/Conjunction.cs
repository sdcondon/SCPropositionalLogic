﻿using System;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a conjunction sentence of propositional logic. In typical PL syntax, this is written as:
    /// <code>{sentence} ∧ {sentence}</code>
    /// </summary>
    public class Conjunction : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Conjunction"/> class.
        /// </summary>
        /// <param name="left">The left side of the conjunction.</param>
        /// <param name="right">The right side of the conjunction.</param>
        public Conjunction(Sentence left, Sentence right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the conjunction.
        /// </summary>
        public Sentence Left { get; }

        /// <summary>
        /// Gets the right side of the conjunction.
        /// </summary>
        public Sentence Right { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Conjunction otherConjunction && Left.Equals(otherConjunction.Left) && Right.Equals(otherConjunction.Right);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Left, Right);
    }
}
