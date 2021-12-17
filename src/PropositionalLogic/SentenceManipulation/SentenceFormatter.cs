using System;
using System.Linq;

namespace LinqToKB.PropositionalLogic.SentenceManipulation
{
    /// <summary>
    /// Temporary..
    /// Will do while I figure out what I need (formatprovider, ToString implementations in inidividual classes, ...?).
    /// Will ultimately want something that is more intelligent with brackets (i.e. drops them where not needed), too.
    /// </summary>
    internal static class SentenceFormatter
    {
        public static string Print(this Sentence sentence) => sentence switch
        {
            Conjunction conjunction => Print(conjunction),
            Disjunction disjunction => Print(disjunction),
            Equivalence equivalence => Print(equivalence),
            Implication implication => Print(implication),
            Negation negation => Print(negation),
            Proposition proposition => Print(proposition),
            _ => throw new ArgumentException("Unsupported sentence type")
        };

        private static string Print(Conjunction conjunction) =>
            $"({Print(conjunction.Left)} ∧ {Print(conjunction.Right)})";

        public static string Print(Disjunction disjunction) =>
            $"({Print(disjunction.Left)} ∨ {Print(disjunction.Right)})";

        public static string Print(Equivalence equivalence) =>
            $"({Print(equivalence.Left)} ⇔ {Print(equivalence.Right)})";

        public static string Print(Implication implication) =>
            $"({Print(implication.Antecedent)} ⇒ {Print(implication.Consequent)})";

        public static string Print(Negation negation) =>
            $"¬{Print(negation.Sentence)}";

        public static string Print(Proposition proposition) =>
            $"{proposition.Symbol}";
    }
}
