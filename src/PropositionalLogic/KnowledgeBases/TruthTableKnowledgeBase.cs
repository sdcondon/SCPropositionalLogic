using SCPropositionalLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SCPropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// Knowledge base that satisfies queries by enumerating all possible models, returning true if and only if the query holds
    /// true in all models in which all of the rules hold true. Obviously, this is incredibly slow for non-trivial models.
    /// </summary>
    public class TruthTableKnowledgeBase : IKnowledgeBase
    {
        private readonly HashSet<Proposition> propositions;
        private readonly PropositionFinder propositionFinder;
        private readonly List<Func<Dictionary<Proposition, bool>, bool>> sentenceSatisfactionChecks = new List<Func<Dictionary<Proposition, bool>, bool>>();

        public TruthTableKnowledgeBase()
        {
            propositions = new HashSet<Proposition>();
            propositionFinder = new PropositionFinder(propositions);
        }

        /// <inheritdoc />
        public void Tell(Sentence sentence)
        {
            propositionFinder.ApplyTo(sentence);
            sentenceSatisfactionChecks.Add(MakeSatisfactionCheck(sentence));
        }

        /// <inheritdoc />
        public bool Ask(Sentence query)
        {
            propositionFinder.ApplyTo(query); // always going to be false if we ask about an unknown proposition, but don't bother shortcutting..
            var querySatisfactionCheck = MakeSatisfactionCheck(query);

            bool CheckAll(Dictionary<Proposition, bool> model, IEnumerable<Proposition> propositions)
            {
                var proposition = propositions.FirstOrDefault();

                // If all symbols are assigned..
                if (proposition == null)
                {
                    // ..doesn't disprove the query iff either the knowledge base isn't satisfied by this model, or the query is
                    return !sentenceSatisfactionChecks.All(check => check(model)) || querySatisfactionCheck(model);
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

            return CheckAll(new Dictionary<Proposition, bool>(), propositions);
        }

        /// <summary>
        /// Converts a <see cref="Sentence"/> into a delegate that accepts the model (a dictionary of proposition values) and returns a value indicating if the provided sentence is satisfied by that model.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <returns>A delegate.</returns>
        private static Func<Dictionary<Proposition, bool>, bool> MakeSatisfactionCheck(Sentence sentence)
        {
            var modelParameter = Expression.Parameter(typeof(Dictionary<Proposition, bool>), "model");

            Expression GetExpressionFor(Sentence sentence)
            {
                // TODO-PERFORMANCE: Using "proper" visitor pattern will be (ever so slightly) faster than this,
                // as well as adhering better to the OCP - decide if its worth the extra complexity.
                return sentence switch
                {
                    Conjunction conjunction => Expression.AndAlso(
                        GetExpressionFor(conjunction.Left),
                        GetExpressionFor(conjunction.Right)),

                    Disjunction disjunction => Expression.OrElse(
                        GetExpressionFor(disjunction.Left),
                        GetExpressionFor(disjunction.Right)),

                    Negation negation => Expression.Not(
                        GetExpressionFor(negation.Sentence)),

                    Implication implication => Expression.OrElse(
                        Expression.Not(GetExpressionFor(implication.Antecedent)),
                        GetExpressionFor(implication.Consequent)),

                    Equivalence equivalence => Expression.Equal(
                        GetExpressionFor(equivalence.Left),
                        GetExpressionFor(equivalence.Right)),

                    // NB: dictionary is guaranteed to contain this key when this is invoked, so there's no need to get clever:
                    Proposition proposition => Expression.MakeIndex(
                        modelParameter,
                        typeof(Dictionary<Proposition, bool>).GetProperty("Item"),
                        new[] { Expression.Constant(proposition) }),
                };
            }

            return Expression.Lambda<Func<Dictionary<Proposition, bool>, bool>>(GetExpressionFor(sentence), modelParameter).Compile();
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
