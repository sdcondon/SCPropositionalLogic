﻿using System;

namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a material implication sentence of propositional logic. In typical PL syntax, this is written as:
    /// <code>{sentence} ⇒ {sentence}</code>
    /// </summary>
    public class Implication : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Implication"/> class.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence.</param>
        /// <param name="consequent">The consequent sentence.</param>
        public Implication(Sentence antecedent, Sentence consequent) => (Antecedent, Consequent) = (antecedent, consequent);

        /// <summary>
        /// Gets the antecedent sentence.
        /// </summary>
        public Sentence Antecedent { get; }

        /// <summary>
        /// Gets the consequent sentence.
        /// </summary>
        public Sentence Consequent { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Implication otherImplication
            && Antecedent.Equals(otherImplication.Antecedent)
            && Consequent.Equals(otherImplication.Consequent);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
    }
}
