using SCPropositionalLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// Knowledge base that satisfies queries by enumerating all possible models, returning true if and only if the query holds
    /// true in all models in which all of the rules hold true. Obviously incredibly slow for non-trivial models.
    /// </summary>
    public class TruthTableKnowledgeBase : IKnowledgeBase
    {
        private readonly HashSet<Proposition> propositions;
        private readonly PropositionFinder propositionFinder;
        private readonly List<Sentence> sentences = new List<Sentence>();

        public TruthTableKnowledgeBase()
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
            /*
             * Pseudocode from 'Artificial Intelligence: A Modern Approach, 3rd Ed' section 7.4, figure 7.10:
             * 
            function TT-ENTAILS?(KB, α) returns true or false
                inputs: 
                  KB, the knowledge base, a sentence in propositional logic
                  α, the query, a sentence in propositional logic  
                symbols ← a list of the proposition symbols in KB and α
                return TT-CHECK-ALL(KB,α, symbols, {})

            function TT-CHECK-ALL(KB, α, symbols, model) returns true or false
                if EMPTY?(symbols) then
                    if PL-TRUE?(KB, model) then return PL-TRUE?(α, model)
                    else return true// when KB is false, always return true
                else do
                    P ← FIRST(symbols)
                    rest ← REST(symbols) 
                    return (TT-CHECK-ALL(KB,α, rest, model ∪ {P = true})  and  TT-CHECK-ALL(KB, α, rest, model ∪ {P = false}))

            Our implementation deviates from this a little because it uses a single dictionary instance.
            */

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

            bool IsSatisfied(Sentence sentence, Dictionary<Proposition, bool> model)
            {
                // This is obviously slow - a place where tighter language integration would be better.
                // See LinqTruthTableKnowledgeBase<TModel> in the benchmarks project for an example of an alternate (language-integrated) approach.
                // Of course, an approach where we (lazily..) compile sentences into Func<TWhatever, bool> is possible (either via Linq expressions
                // or the Emit API directly - just need to know how to extract proposition values from TWhatever), but haven't bothered to do that yet..
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
