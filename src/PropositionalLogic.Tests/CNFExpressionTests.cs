using FluentAssertions;
using System.Linq;
using Xunit;

namespace LinqToKB.PropositionalLogic
{
    public class CNFExpressionTests
    {
        [Fact]
        public void ConstructionOfUnitClause()
        {
            var e = new CNFExpression<MyModel>(m => m.P);

            e.ShouldHaveState(clauseCount: 1);

            e.Clauses.Single().ShouldHaveState(
                isDefiniteClause: true,
                isGoalClause: false,
                isHornClause: true,
                isUnitClause: true,
                literalCount: 1);

            e.Clauses.Single().Literals.Single().ShouldHaveState(
                atomicSentenceSymbol: "P",
                isNegated: false);
        }

        [Fact]
        public void ConstructionOfNegatedUnitClause()
        {
            var e = new CNFExpression<MyModel>(m => !m.P);

            e.ShouldHaveState(clauseCount: 1);

            e.Clauses.Single().ShouldHaveState(
                isDefiniteClause: false,
                isGoalClause: true,
                isHornClause: true,
                isUnitClause: true,
                literalCount: 1);

            e.Clauses.Single().Literals.Single().ShouldHaveState(
                atomicSentenceSymbol: "P",
                isNegated: true);
        }

        [Fact]
        public void ConstructionOfNonNormalExpression()
        {
            var e = new CNFExpression<MyModel>(PLExpression<MyModel>.Iff(m => m.P, m => !(!m.Q && !m.R)));

            e.ShouldHaveState(clauseCount: 3);

            e.Clauses.ElementAt(0).ShouldHaveState(
                isDefiniteClause: false,
                isGoalClause: false,
                isHornClause: false,
                isUnitClause: false,
                literalCount: 3);
            e.Clauses.ElementAt(0).Literals.ElementAt(0).ShouldHaveState(
                atomicSentenceSymbol: "P",
                isNegated: true);
            e.Clauses.ElementAt(0).Literals.ElementAt(1).ShouldHaveState(
                atomicSentenceSymbol: "Q",
                isNegated: false);
            e.Clauses.ElementAt(0).Literals.ElementAt(2).ShouldHaveState(
                atomicSentenceSymbol: "R",
                isNegated: false);

            e.Clauses.ElementAt(1).ShouldHaveState(
                isDefiniteClause: true,
                isGoalClause: false,
                isHornClause: true,
                isUnitClause: false,
                literalCount: 2);
            e.Clauses.ElementAt(1).Literals.ElementAt(0).ShouldHaveState(
                atomicSentenceSymbol: "P",
                isNegated: false);
            e.Clauses.ElementAt(1).Literals.ElementAt(1).ShouldHaveState(
                atomicSentenceSymbol: "Q",
                isNegated: true);

            e.Clauses.ElementAt(2).ShouldHaveState(
                isDefiniteClause: true,
                isGoalClause: false,
                isHornClause: true,
                isUnitClause: false,
                literalCount: 2);
            e.Clauses.ElementAt(2).Literals.ElementAt(0).ShouldHaveState(
                atomicSentenceSymbol: "P",
                isNegated: false);
            e.Clauses.ElementAt(2).Literals.ElementAt(1).ShouldHaveState(
                atomicSentenceSymbol: "R",
                isNegated: true);
        }

        [Fact]
        public void ResolutionOfResolvableClauses()
        {
            var resolvents = CNFClause<MyModel>.Resolve(
                new CNFExpression<MyModel>(m => !m.P || m.Q).Clauses.Single(),
                new CNFExpression<MyModel>(m => m.P).Clauses.Single());
            Assert.Single(resolvents, new CNFExpression<MyModel>(m => m.Q).Clauses.Single());
        }

        [Fact]
        public void ResolutionOfResolvableClauses2()
        {
            var resolvents = CNFClause<MyModel>.Resolve(
                new CNFExpression<MyModel>(m => !m.P || m.Q).Clauses.Single(),
                new CNFExpression<MyModel>(m => m.P || m.R).Clauses.Single());
            Assert.Single(resolvents, new CNFExpression<MyModel>(m => m.Q || m.R).Clauses.Single());
        }

        [Fact]
        public void ResolutionOfUnresolvableClauses()
        {
            var resolvents = CNFClause<MyModel>.Resolve(
                new CNFExpression<MyModel>(m => m.P).Clauses.Single(),
                new CNFExpression<MyModel>(m => m.Q).Clauses.Single());
            Assert.Empty(resolvents);
        }

        [Fact]
        public void ResolutionOfComplementaryUnitClauses()
        {
            var resolvents = CNFClause<MyModel>.Resolve(
                new CNFExpression<MyModel>(m => m.P).Clauses.Single(),
                new CNFExpression<MyModel>(m => !m.P).Clauses.Single());
            Assert.Single(resolvents, CNFClause<MyModel>.Empty);
        }

        [Fact]
        public void ResolutionOfMultiplyResolvableClauses()
        {
            var resolvents = CNFClause<MyModel>.Resolve(
                new CNFExpression<MyModel>(m => m.P || m.Q).Clauses.Single(),
                new CNFExpression<MyModel>(m => !m.P || !m.Q).Clauses.Single());
            resolvents.Should().BeEquivalentTo(new[]
            {
                new CNFExpression<MyModel>(m => m.P || !m.P).Clauses.Single(),
                new CNFExpression<MyModel>(m => m.Q || !m.Q).Clauses.Single()
            });
        }
    }

    internal class MyModel
    {
        public bool P { get; set; }
        public bool Q { get; set; }
        public bool R { get; set; }
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
            Assert.Equal(atomicSentenceSymbol, literal.AtomicSentence.Symbol);
            Assert.Equal(isNegated, literal.IsNegated);
        }
    }
}
