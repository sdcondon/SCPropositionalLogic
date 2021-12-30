namespace SCPropositionalLogic
{
    /// <summary>
    /// Representation of a sentence of propositional logic.
    /// </summary>
    public abstract partial class Sentence
    {
        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="Conjunction"/>(s) of two (or more) operands.
        /// </summary>
        /// <param name="operand1">The first operand of the conjunction.</param>
        /// <param name="operand2">The second operand of the conjunction.</param>
        /// <param name="otherOperands">Any additional operands.</param>
        /// <returns>A new <see cref="Conjunction"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly non-verbose.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence And(Sentence operand1, Sentence operand2, params Sentence[] otherOperands)
        {
            var conjunction = new Conjunction(operand1, operand2);

            foreach (var operand in otherOperands)
            {
                conjunction = new Conjunction(conjunction, operand);
            }

            return conjunction;
        }

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="Disjunction"/>(s) of two (or more) operands.
        /// </summary>
        /// <param name="operand1">The first operand of the disjunction.</param>
        /// <param name="operand2">The second operand of the disjunction.</param>
        /// <param name="otherOperands">Any additional operands.</param>
        /// <returns>A new <see cref="Disjunction"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly non-verbose.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Or(Sentence operand1, Sentence operand2, params Sentence[] otherOperands)
        {
            var disjunction = new Disjunction(operand1, operand2);

            foreach (var operand in otherOperands)
            {
                disjunction = new Disjunction(disjunction, operand);
            }

            return disjunction;
        }

        /// <summary>
        /// Shorthand factory method for a new <see cref="Implication"/> instance.
        /// </summary>
        /// <param name="left">The antecedent sentence of the implication.</param>
        /// <param name="right">The consequent sentence of the implication.</param>
        /// <returns>A new <see cref="Implication"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly non-verbose.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence If(Sentence antecedent, Sentence consequent) => new Implication(antecedent, consequent);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Equivalence"/> instance.
        /// </summary>
        /// <param name="left">The left-hand operand of the equivalence.</param>
        /// <param name="right">The right-hand operand of the equivalence.</param>
        /// <returns>A new <see cref="Equivalence"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly non-verbose.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Iff(Sentence left, Sentence right) => new Equivalence(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Negation"/> instance.
        /// </summary>
        /// <param name="sentence">The negated sentence.</param>
        /// <returns>A new <see cref="Negation"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly non-verbose.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Not(Sentence sentence) => new Negation(sentence);
    }
}
