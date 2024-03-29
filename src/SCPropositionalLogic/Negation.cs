﻿using System;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of an negation sentence of propositional logic. In typical PL syntax, this is written as:
    /// <code>¬{sentence}</code>
    /// </summary>
    public class Negation : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Negation"/> class.
        /// </summary>
        /// <param name="sentence">The sentence that is negated.</param>
        public Negation(Sentence sentence) => Sentence = sentence;

        /// <summary>
        /// Gets the sentence that is negated.
        /// </summary>
        public Sentence Sentence { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Negation otherNegation && Sentence.Equals(otherNegation.Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }
}
