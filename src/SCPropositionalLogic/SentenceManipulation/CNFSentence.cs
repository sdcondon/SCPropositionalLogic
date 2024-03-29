﻿using System.Collections.Generic;

namespace SCPropositionalLogic.SentenceManipulation
{
    /// <summary>
    /// Representation of a <see cref="Sentence"/> in conjunctive normal form (CNF).
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of this expression refer to.</typeparam>
    public class CNFSentence
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFSentence"/> class, implicitly converting the provided sentence to CNF in the process.
        /// </summary>
        /// <param name="sentence">The sentence to (convert and) represent.</param>
        public CNFSentence(Sentence sentence)
        {
            Sentence = new CNFConversion().ApplyTo(sentence);
            var clauses = new List<CNFClause>();
            new SentenceConstructor(clauses).ApplyTo(Sentence);
            Clauses = clauses.AsReadOnly();
        }

        /// <summary>
        /// Gets the actual <see cref="Sentence"/> that underlies this representation.
        /// </summary>
        public Sentence Sentence { get; }

        /// <summary>
        /// Gets the collection of clauses that comprise this CNF sentence.
        /// </summary>
        public IReadOnlyCollection<CNFClause> Clauses { get; }

        /// <summary>
        /// Defines the (implicit) conversion of a <see cref="Sentence"/> instance to a <see cref="CNFSentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to convert.</param>
        /// <remarks>
        /// I'm still not 100% happy with exactly how the CNFSentence / Sentence dichotomy is handled. Almost all of the time
        /// we'll be wanting to deal with CNF - but the "raw" sentence tree structure still has value. This conversion
        /// operator helps, but there's almost certainly more that could be done.
        /// </remarks>
        public static implicit operator CNFSentence(Sentence sentence) => new CNFSentence(sentence);

        /// <summary>
        /// Sentence "Transformation" that constructs a set of <see cref="CNFClause"/> objects from a <see cref="Sentence"/> in CNF.
        /// </summary>
        private class SentenceConstructor : SentenceTransformation
        {
            private readonly List<CNFClause> clauses;

            public SentenceConstructor(List<CNFClause> clauses) => this.clauses = clauses;

            /// <inheritdoc />
            public override Sentence ApplyTo(Sentence sentence)
            {
                if (sentence is Conjunction)
                {
                    // The expression is already in CNF - so the root down until the individual clauses will all be Conjunctions - we just skip past those.
                    return base.ApplyTo(sentence);
                }
                else
                {
                    // We've (assumedly) hit a clause (will throw if its not actually a clause).
                    clauses.Add(new CNFClause(sentence));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor, above,
                    // does so to figure out the details of the clause). So we can just return sentence rather than invoking base.ApplyTo. 
                    return sentence;
                }
            }
        }
    }
}
