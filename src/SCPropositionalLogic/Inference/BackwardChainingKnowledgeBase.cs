using SCPropositionalLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPropositionalLogic.Inference
{
    /// <summary>
    /// A very basic implementation of backward chaining.
    /// </summary>
    public class BackwardChainingKnowledgeBase : IKnowledgeBase
    {
        private readonly Dictionary<Proposition, List<CNFClause>> clausesByConsequent = new Dictionary<Proposition, List<CNFClause>>();

        /// <inheritdoc />
        public void Tell(Sentence sentence)
        {
            var cnfSentence = new CNFSentence(sentence);

            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses");
            }

            foreach (var clause in cnfSentence.Clauses)
            {
                var consequent = clause.Literals.Single(l => l.IsPositive).Proposition;

                if (!clausesByConsequent.TryGetValue(consequent, out var clausesWithThisConsequent))
                {
                    clausesWithThisConsequent = clausesByConsequent[consequent] = new List<CNFClause>();
                }

                clausesWithThisConsequent.Add(clause);
            }
        }

        /// <inheritdoc />
        public bool Ask(Sentence query)
        {
            if (!(query is Proposition p))
            {
                throw new ArgumentException("This knowledge base supports only queries that are propositions");
            }

            return VisitProposition(p, Path.Empty).Succeeded;
        }

        private Outcome VisitProposition(Proposition proposition, Path path)
        {
            if (!clausesByConsequent.TryGetValue(proposition, out var clausesForProposition))
            {
                return new Outcome(false);
            }

            if (clausesForProposition.Any(c => c.IsUnitClause))
            {
                return new Outcome(true);
            }

            if (path.Contains(proposition))
            {
                return new Outcome(false);
            }

            path = path.Prepend(proposition);

            foreach (var clause in clausesForProposition)
            {
                var subTrees = VisitClause(clause, path);
                if (subTrees != null)
                {
                    return new Outcome(new Tree(clause, subTrees));
                }
            }

            return new Outcome(false);
        }

        private IReadOnlyDictionary<Proposition, Tree> VisitClause(CNFClause clause, Path path)
        {
            var subTrees = new Dictionary<Proposition, Tree>();

            foreach (var antecedent in clause.Literals.Where(l => l.IsNegated))
            {
                var outcome = VisitProposition(antecedent.Proposition, path);

                if (!outcome.Succeeded)
                {
                    return null;
                }

                subTrees[antecedent.Proposition] = outcome.Result;
            }

            return subTrees;
        }

        /// <summary>
        /// Container for the outcome of a <see cref="AndOrDFS{TNode,TEdge}"/> search. Just a friendly struct wrapped around an optional <see cref="Tree"/>.
        /// </summary>
        private struct Outcome
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Outcome"/> struct that either indicates failure, or success with an empty tree (because a target node has been reached).
            /// </summary>
            /// <param name="succeeded">A value indicating whether the outcome is a success.</param>
            public Outcome(bool succeeded) => Result = succeeded ? Tree.Empty : null;

            /// <summary>
            /// Initializes a new instance of the <see cref="Outcome"/> struct that indicates success.
            /// </summary>
            /// <param name="result">The search tree - the leaves of which include only target nodes.</param>
            public Outcome(Tree result) => Result = result ?? throw new ArgumentNullException(nameof(result));

            /// <summary>
            /// Gets a value indicating whether the search succeeded in creating a tree.
            /// </summary>
            public bool Succeeded => Result != null;

            /// <summary>
            /// Gets the search tree - the leaves of which include only target nodes.
            /// </summary>
            public Tree Result { get; }
        }

        /// <summary>
        /// Container for a tree (or sub-tree) created by a <see cref="BackwardChainingKnowledgeBase"/> query. Not really used for the mo. Keeping in case I change that
        /// so that queries (exist as objects and) are made explainable - as in SCFirstOrderLogic.
        /// </summary>
        public class Tree
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Tree"/> class.
            /// </summary>
            /// <param name="root">The root edge of the tree.</param>
            /// <param name="subTreesByRootNode">The sub-trees that follow the root edge, keyed by node that they connect from. There will be more than one if the root edge actually represents a set of more than one coinjoined ("and") edges.</param>
            internal Tree(CNFClause root, IReadOnlyDictionary<Proposition, Tree> subTreesByRootNode)
            {
                if (root == null)
                {
                    throw new ArgumentNullException(nameof(root));
                }

                Root = root;
                SubTrees = subTreesByRootNode ?? throw new ArgumentNullException(nameof(subTreesByRootNode));
            }

            private Tree() => (Root, SubTrees) = (default, null);

            /// <summary>
            /// Gets an empty tree - that indicates that a target node has been reached.
            /// </summary>
            public static Tree Empty { get; } = new Tree();

            /// <summary>
            /// Gets the root edge of the tree.
            /// </summary>
            public CNFClause Root { get; }

            /// <summary>
            /// Gets the sub-trees that follow the root edge, keyed by node that they connect from. There will be more than one if the root edge actually represents a set of more than one coinjoined ("and") edges.
            /// </summary>
            public IReadOnlyDictionary<Proposition, Tree> SubTrees { get; }

            /// <summary>
            /// Flattens the tree out into a single mapping from the current node to the edge that should be followed to ultimately reach only target nodes.
            /// Intended to make trees easier to work with in certain situations (e.g. assertions in tests).
            /// <para/>
            /// Each node will occur at most once in the entire tree, so we can always safely do this.
            /// </summary>
            /// <returns>A mapping from the current node to the edge that should be followed to ultimately reach only target nodes.</returns>
            public IReadOnlyDictionary<Proposition, CNFClause> Flatten()
            {
                var flattened = new Dictionary<Proposition, CNFClause>();

                void Visit(Tree tree)
                {
                    if (!tree.Equals(Tree.Empty))
                    {
                        flattened[tree.Root.Literals.Single(l => l.IsPositive).Proposition] = tree.Root;
                        foreach (var subPlan in tree.SubTrees.Values)
                        {
                            Visit(subPlan);
                        }
                    }
                }

                Visit(this);

                return flattened;
            }
        }

        private class Path
        {
            private Path(Proposition first, Path rest) => (First, Rest) = (first, rest);

            public static Path Empty { get; } = new Path(default, null);

            public Proposition First { get; }

            public Path Rest { get; }

            public Path Prepend(Proposition proposition) => new Path(proposition, this);

            public bool Contains(Proposition proposition) => (First?.Equals(proposition) ?? false) || (Rest?.Contains(proposition) ?? false);
        }
    }
}
