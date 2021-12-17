using System;

namespace LinqToKB.PropositionalLogic
{
    /// <summary>
    /// Representation of a proposition sentence of propositional logic, In typical PL syntax, this is written as:
    /// <code>{proposition symbol}</code>
    /// </summary>
    public class Proposition : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class.
        /// </summary>
        /// <param name="symbol">An object representing the symbol of the proposition.</param>
        public Proposition(object symbol) => Symbol = symbol;

        /// <summary>
        /// Gets an object representing the symbol of the proposition.
        /// </summary>
        /// <remarks>
        /// Symbol equality should indicate that it is the "same" proposition in the model. ToString of the Symbol should be appropriate for rendering in PL syntax.
        /// </remarks>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Proposition otherProposition && otherProposition.Symbol.Equals(Symbol);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Symbol.GetHashCode());
    }
}
