using SCPropositionalLogic.SentenceManipulation.ConjunctiveNormalForm;
using System.Collections.Generic;
using System.Linq;

namespace SCPropositionalLogic.KnowledgeBases
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// </summary>
    public class ResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly List<CNFSentence> sentences = new List<CNFSentence>();

        /// <inheritdoc />
        public void Tell(Sentence sentence)
        {
            sentences.Add(new CNFSentence(sentence));
        }

        /// <inheritdoc />
        public bool Ask(Sentence sentence)
        {
            var negationOfQuery = new Negation(sentence);
            var negationOfQueryAsCnf = new CNFSentence(negationOfQuery);
            var clauses = sentences.Append(negationOfQueryAsCnf).SelectMany(s => s.Clauses).ToHashSet();
            var queue = new Queue<(CNFClause, CNFClause)>();

            foreach (var ci in clauses)
            {
                foreach (var cj in clauses)
                {
                    queue.Enqueue((ci, cj));
                }
            }

            while (queue.Count > 0)
            {
                var (ci, cj) = queue.Dequeue();
                var resolvents = CNFClause.Resolve(ci, cj);

                foreach (var resolvent in resolvents)
                {
                    if (resolvent.Equals(CNFClause.Empty))
                    {
                        return true;
                    }

                    if (!clauses.Contains(resolvent))
                    {
                        foreach (var clause in clauses)
                        {
                            queue.Enqueue((clause, resolvent));
                        }

                        clauses.Add(resolvent);
                    }
                }
            }

            return false;
        }
    }
}
