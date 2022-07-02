namespace SCPropositionalLogic.SentenceManipulation
{
    /// <summary>
    /// Implementation of <see cref="SentenceTransformation"/> that converts sentences to conjunctive normal form.
    /// </summary>
    public class CNFConversion : SentenceTransformation
    {
        private static readonly ImplicationElimination implicationElimination = new ImplicationElimination();
        private static readonly NNFConversion nnfConversion = new NNFConversion();
        private static readonly DisjunctionDistribution disjunctionDistribution = new DisjunctionDistribution();

        /// <summary>
        /// Gets a singleton instance of the <see cref="CNFConversion"/> class.
        /// </summary>
        public static CNFConversion Instance => new CNFConversion();

        /// <inheritdoc />
        public override Sentence ApplyTo(Sentence sentence)
        {
            // Might be possible to do some of these conversions at the same time, but for now
            // at least, do them sequentially - favour maintainability over performance for the mo.
            sentence = implicationElimination.ApplyTo(sentence);
            sentence = nnfConversion.ApplyTo(sentence);
            sentence = disjunctionDistribution.ApplyTo(sentence);

            // TODO-USABILITY: Strictly speaking its not needed for the normalisation process, but I wonder if we should also
            // ensure left- (or right-) first ordering of conjunctions and disjunctions so that the the output Sentences are equal for
            // two inputs if they would ultimately create two CNFSentences that are considered equal. Then again, its extra work if we
            // don't need this. Perhaps only when we're actually interested in the output sentence (as opposed to situations where 
            // we're immediately going to turn it into a CNFSentence). There's probably a useful refactoring here.. TODO!

            return sentence;
        }

        /// <summary>
        /// Transformation that eliminates implications by replacing P ⇒ Q with ¬P ∨ Q and P ⇔ Q with (¬P ∨ Q) ∧ (P ∨ ¬Q)
        /// </summary>
        private class ImplicationElimination : SentenceTransformation
        {
            /// <inheritdoc />
            protected override Sentence ApplyTo(Implication implication)
            {
                return ApplyTo(new Disjunction(
                    new Negation(implication.Antecedent),
                    implication.Consequent));
            }

            /// <inheritdoc />
            protected override Sentence ApplyTo(Equivalence equivalence)
            {
                return ApplyTo(new Conjunction(
                    new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                    new Disjunction(equivalence.Left, new Negation(equivalence.Right))));
            }
        }

        /// <summary>
        /// Transformation that converts to Negation Normal Form by moving negations as far down as possible in the sentence tree.
        /// </summary>
        private class NNFConversion : SentenceTransformation
        {
            /// <inheritdoc />
            protected override Sentence ApplyTo(Negation negation)
            {
                Sentence sentence;

                if (negation.Sentence is Negation n)
                {
                    // Eliminate double negative: ¬(¬P) ≡ P
                    sentence = n.Sentence;
                }
                else if (negation.Sentence is Conjunction c)
                {
                    // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                    sentence = new Disjunction(
                        new Negation(c.Left),
                        new Negation(c.Right));
                }
                else if (negation.Sentence is Disjunction d)
                {
                    // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                    sentence = new Conjunction(
                        new Negation(d.Left),
                        new Negation(d.Right));
                }
                else
                {
                    return base.ApplyTo(negation);
                }

                return ApplyTo(sentence);
            }
        }

        /// <summary>
        /// Transformation that recursively distributes disjunctions over conjunctions.
        /// </summary>
        private class DisjunctionDistribution : SentenceTransformation
        {
            protected override Sentence ApplyTo(Disjunction disjunction)
            {
                Sentence sentence;

                if (disjunction.Right is Conjunction cRight)
                {
                    // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                    // NB the "else if" below is fine (i.e. we don't need a seperate case for if they are both &&s)
                    // since if b.Left is also an &&, well end up distributing over it once we recurse down as far
                    // as the Expression.OrElses we create here.
                    sentence = new Conjunction(
                        new Disjunction(disjunction.Left, cRight.Left),
                        new Disjunction(disjunction.Left, cRight.Right));
                }
                else if (disjunction.Left is Conjunction cLeft)
                {
                    // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
                    sentence = new Conjunction(
                        new Disjunction(cLeft.Left, disjunction.Right),
                        new Disjunction(cLeft.Right, disjunction.Right));
                }
                else
                {
                    return base.ApplyTo(disjunction);
                }

                return ApplyTo(sentence);
            }
        }
    }
}
