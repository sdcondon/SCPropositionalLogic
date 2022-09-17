using System;

namespace SCPropositionalLogic.SentenceFormatting
{
    /// <summary>
    /// Temporary..
    /// Will do while I figure out what I want/need (formatprovider, ToString implementations in inidividual classes, ...?).
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
            $"({conjunction.Left.Print()} ∧ {conjunction.Right.Print()})";

        public static string Print(Disjunction disjunction) =>
            $"({disjunction.Left.Print()} ∨ {disjunction.Right.Print()})";

        public static string Print(Equivalence equivalence) =>
            $"({equivalence.Left.Print()} ⇔ {equivalence.Right.Print()})";

        public static string Print(Implication implication) =>
            $"({implication.Antecedent.Print()} ⇒ {implication.Consequent.Print()})";

        public static string Print(Negation negation) =>
            $"¬{negation.Sentence.Print()}";

        public static string Print(Proposition proposition) =>
            $"{proposition.Symbol}";
    }
}
