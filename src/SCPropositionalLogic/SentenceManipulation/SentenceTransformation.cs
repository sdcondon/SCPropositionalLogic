using System;

namespace SCPropositionalLogic.SentenceManipulation
{
    /// <summary>
    /// Base class for transformations of <see cref="Sentence"/> instances.
    /// </summary>
    public abstract class SentenceTransformation
    {
        /// <summary>
        /// Applies this transformation to a <see cref="Sentence"/> instance.
        /// The default implementation simply invokes the Apply method appropriate to the actual type of the sentence.
        /// </summary>
        /// <param name="sentence">The sentence to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual Sentence ApplyTo(Sentence sentence)
        {
            // TODO-PERFORMANCE: Using "proper" visitor pattern will be (ever so slightly) faster than this,
            // as well as adhering better to the OCP - decide if its worth the extra complexity.
            return sentence switch
            {
                Conjunction conjunction => ApplyTo(conjunction),
                Disjunction disjunction => ApplyTo(disjunction),
                Equivalence equivalence => ApplyTo(equivalence),
                Implication implication => ApplyTo(implication),
                Negation negation => ApplyTo(negation),
                Proposition proposition => ApplyTo(proposition),
                _ => throw new ArgumentException("Unsupported sentence type")
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Conjunction"/> instance.
        /// The default implementation returns a <see cref="Conjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Conjunction conjunction)
        {
            var left = ApplyTo(conjunction.Left);
            var right = ApplyTo(conjunction.Right);
            if (left != conjunction.Left || right != conjunction.Right)
            {
                return new Conjunction(left, right);
            }

            return conjunction;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Disjunction"/> instance.
        /// The default implementation returns a <see cref="Disjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The <see cref="Disjunction"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Disjunction disjunction)
        {
            var left = ApplyTo(disjunction.Left);
            var right = ApplyTo(disjunction.Right);
            if (left != disjunction.Left || right != disjunction.Right)
            {
                return new Disjunction(left, right);
            }

            return disjunction;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Equivalence"/> instance. 
        /// The default implementation returns an <see cref="Equivalence"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Equivalence equivalence)
        {
            var equivalent1 = ApplyTo(equivalence.Left);
            var equivalent2 = ApplyTo(equivalence.Right);
            if (equivalent1 != equivalence.Left || equivalent2 != equivalence.Right)
            {
                return new Equivalence(equivalent1, equivalent2);
            }

            return equivalence;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Implication"/> instance. 
        /// The default implementation returns an <see cref="Implication"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Implication implication)
        {
            var antecedent = ApplyTo(implication.Antecedent);
            var consequent = ApplyTo(implication.Consequent);

            if (antecedent != implication.Antecedent || consequent != implication.Consequent)
            {
                return new Implication(antecedent, consequent);
            }

            return implication;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Proposition"/> instance. 
        /// The default implementation simply returns the <see cref="Proposition"/> unchanged.
        /// </summary>
        /// <param name="predicate">The <see cref="Proposition"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Proposition proposition)
        {
            return proposition;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Negation"/> instance. 
        /// The default implementation returns a <see cref="Negation"/> of the result of calling <see cref="ApplyTo"/> on the current sub-sentence.
        /// </summary>
        /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Negation negation)
        {
            var sentence = ApplyTo(negation.Sentence);

            if (sentence != negation.Sentence)
            {
                return new Negation(sentence);
            }

            return negation;
        }
    }
}
