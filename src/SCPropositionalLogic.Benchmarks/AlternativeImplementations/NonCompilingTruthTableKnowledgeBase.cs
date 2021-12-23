using SCPropositionalLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// Truth table knowledge base that just visits the sentence to check whether its satisfied for a particular model,
    /// rather than compiling the check into IL.
    /// </summary>
    public class NonCompilingTruthTableKnowledgeBase : IKnowledgeBase
    {
        private readonly HashSet<Proposition> propositions;
        private readonly PropositionFinder propositionFinder;
        private readonly List<Sentence> sentences = new List<Sentence>();

        public NonCompilingTruthTableKnowledgeBase()
        {
            propositions = new HashSet<Proposition>();
            propositionFinder = new PropositionFinder(propositions);
        }

        /// <inheritdoc />
        public void Tell(Sentence sentence)
        {
            propositionFinder.ApplyTo(sentence);
            sentences.Add(sentence);
        }

        /// <inheritdoc />
        public bool Ask(Sentence query)
        {
            bool CheckAll(Dictionary<Proposition, bool> model, IEnumerable<Proposition> propositions)
            {
                var proposition = propositions.FirstOrDefault();

                // If all symbols are assigned..
                if (proposition == null)
                {
                    // ..doesn't disprove the query iff either the knowledge base isn't satisfied by this model, or the query is
                    return !sentences.All(s => IsSatisfied(s, model)) || IsSatisfied(query, model);
                }

                // ..else recurse until all symbols are assigned
                var rest = propositions.Skip(1); // TODO-PERFORMANCE: inefficient nested enums

                model[proposition] = true;
                if (!CheckAll(model, rest))
                {
                    return false;
                }

                model[proposition] = false;
                return CheckAll(model, rest);
            }

            // This is obviously the difference between this class and the implementation in the library itself.
            // Here we visit the sentence tree every time rather than compiling the satisfaction check into IL.
            bool IsSatisfied(Sentence sentence, Dictionary<Proposition, bool> model)
            {
                return sentence switch
                {
                    Conjunction conjunction => IsSatisfied(conjunction.Left, model) && IsSatisfied(conjunction.Right, model),
                    Disjunction disjunction => IsSatisfied(disjunction.Left, model) || IsSatisfied(disjunction.Right, model),
                    Negation negation => !IsSatisfied(negation.Sentence, model),
                    Implication implication => !IsSatisfied(implication.Antecedent, model) || IsSatisfied(implication.Consequent, model),
                    Equivalence equivalence => IsSatisfied(equivalence.Left, model) == IsSatisfied(equivalence.Right, model),
                    Proposition proposition => model[proposition],
                    _ => throw new ArgumentException($"Unsupported sentence type {sentence.GetType()}")
                };
            }

            propositionFinder.ApplyTo(query); // always going to be false if we ask about an unknown proposition, but don't bother shortcutting..
            return CheckAll(new Dictionary<Proposition, bool>(), propositions);
        }

        private class PropositionFinder : SentenceTransformation
        {
            private readonly HashSet<Proposition> propositions;

            public PropositionFinder(HashSet<Proposition> propositions) => this.propositions = propositions;

            protected override Sentence ApplyTo(Proposition proposition)
            {
                propositions.Add(proposition);
                return proposition;
            }
        }
    }
}
