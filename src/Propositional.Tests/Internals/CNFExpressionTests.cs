using System.Linq;
using Xunit;

namespace LinqToKB.Propositional.Internals
{
    public class CNFExpressionTests
    {
        [Fact]
        public void UnitClause()
        {
            var e = new CNFExpression<MyModel>(m => m.L);

            e.ShouldHaveState(clauseCount: 1);

            e.Clauses.Single().ShouldHaveState(
                isDefiniteClause: true,
                isGoalClause: false,
                isHornClause: true,
                isUnitClause: true,
                literalCount: 1);

            e.Clauses.Single().Literals.Single().ShouldHaveState(
                atomicSentenceSymbol: "m.L",
                isNegated: false);
        }

        [Fact]
        public void NeedsNormalisation()
        {
            var e = new CNFExpression<MyModel>(PLExpression<MyModel>.Iff(m => m.L, m => m.R1 || m.R2));

            e.ShouldHaveState(clauseCount: 3);

            e.Clauses.ElementAt(0).ShouldHaveState(
                isDefiniteClause: false,
                isGoalClause: false,
                isHornClause: false,
                isUnitClause: false,
                literalCount: 3);
            e.Clauses.ElementAt(0).Literals.ElementAt(0).ShouldHaveState(
                atomicSentenceSymbol: "m.L",
                isNegated: true);
            e.Clauses.ElementAt(0).Literals.ElementAt(1).ShouldHaveState(
                atomicSentenceSymbol: "m.R1",
                isNegated: false);
            e.Clauses.ElementAt(0).Literals.ElementAt(2).ShouldHaveState(
                atomicSentenceSymbol: "m.R2",
                isNegated: false);

            e.Clauses.ElementAt(1).ShouldHaveState(
                isDefiniteClause: true,
                isGoalClause: false,
                isHornClause: true,
                isUnitClause: false,
                literalCount: 2);
            e.Clauses.ElementAt(1).Literals.ElementAt(0).ShouldHaveState(
                atomicSentenceSymbol: "m.R1",
                isNegated: true);
            e.Clauses.ElementAt(1).Literals.ElementAt(1).ShouldHaveState(
                atomicSentenceSymbol: "m.L",
                isNegated: false);

            e.Clauses.ElementAt(2).ShouldHaveState(
                isDefiniteClause: true,
                isGoalClause: false,
                isHornClause: true,
                isUnitClause: false,
                literalCount: 2);
            e.Clauses.ElementAt(2).Literals.ElementAt(0).ShouldHaveState(
                atomicSentenceSymbol: "m.R2",
                isNegated: true);
            e.Clauses.ElementAt(2).Literals.ElementAt(1).ShouldHaveState(
                atomicSentenceSymbol: "m.L",
                isNegated: false);
        }
    }

    internal class MyModel
    {
        public bool L { get; set; }
        public bool R1 { get; set; }
        public bool R2 { get; set; }
    }

    internal static class CNFAssertionExtensions
    {
        public static void ShouldHaveState(
            this CNFExpression<MyModel> expression,
            int clauseCount)
        {
            Assert.Equal(clauseCount, expression.Clauses.Count);
        }

        public static void ShouldHaveState(
            this CNFClause<MyModel> clause,
            bool isDefiniteClause,
            bool isGoalClause,
            bool isHornClause,
            bool isUnitClause,
            int literalCount)
        {
            Assert.Equal(isDefiniteClause, clause.IsDefiniteClause);
            Assert.Equal(isGoalClause, clause.IsGoalClause);
            Assert.Equal(isHornClause, clause.IsHornClause);
            Assert.Equal(isUnitClause, clause.IsUnitClause);
            Assert.Equal(literalCount, clause.Literals.Count);
        }

        public static void ShouldHaveState(
            this PLLiteral<MyModel> literal,
            string atomicSentenceSymbol,
            bool isNegated)
        {
            Assert.Equal(atomicSentenceSymbol, literal.AtomicSentenceSymbol);
            Assert.Equal(isNegated, literal.IsNegated);
        }
    }
}
