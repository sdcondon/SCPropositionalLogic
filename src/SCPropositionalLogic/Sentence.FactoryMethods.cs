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
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly succinct.
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
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly succinct.
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
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly succinct.
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
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Iff(Sentence left, Sentence right) => new Equivalence(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Negation"/> instance.
        /// </summary>
        /// <param name="sentence">The negated sentence.</param>
        /// <returns>A new <see cref="Negation"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCPropositionalLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Not(Sentence sentence) => new Negation(sentence);

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "A".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition A => new Proposition(nameof(A));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "B".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition B => new Proposition(nameof(B));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "C".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition C => new Proposition(nameof(C));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "D".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition D => new Proposition(nameof(D));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "E".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition E => new Proposition(nameof(E));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "F".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition F => new Proposition(nameof(F));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "G".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition G => new Proposition(nameof(G));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "H".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition H => new Proposition(nameof(H));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "I".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition I => new Proposition(nameof(I));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "J".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition J => new Proposition(nameof(J));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "K".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition K => new Proposition(nameof(K));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "L".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition L => new Proposition(nameof(L));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "M".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition M => new Proposition(nameof(M));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "N".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition N => new Proposition(nameof(N));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "O".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition O => new Proposition(nameof(O));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "P".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition P => new Proposition(nameof(P));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "Q".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition Q => new Proposition(nameof(Q));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "R".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition R => new Proposition(nameof(R));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "S".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition S => new Proposition(nameof(S));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "T".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition T => new Proposition(nameof(T));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "U".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition U => new Proposition(nameof(U));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "V".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition V => new Proposition(nameof(V));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "W".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition W => new Proposition(nameof(W));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "X".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition X => new Proposition(nameof(X));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "Y".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition Y => new Proposition(nameof(Y));

        /// <summary>
        /// Gets a new <see cref="Proposition"/> with the symbol "Z".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the tests though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static Proposition Z => new Proposition(nameof(Z));
    }
}
