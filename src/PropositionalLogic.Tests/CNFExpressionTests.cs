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
            new CNFExpression<MyModel>(m => m.P).Should().BeEquivalentTo(new
            {
                Clauses = new[]
                {
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = true,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = false },
                        }
                    }
                }
            });
        }

        [Fact]
        public void ConstructionOfNegatedUnitClause()
        {
            new CNFExpression<MyModel>(m => !m.P).Should().BeEquivalentTo(new
            {
                Clauses = new[]
                {
                    new
                    {
                        IsDefiniteClause = false,
                        IsGoalClause = true,
                        IsHornClause = true,
                        IsUnitClause = true,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = true },
                        }
                    }
                }
            });
        }

        [Fact]
        public void ConstructionOfNonNormalExpression()
        {
            new CNFExpression<MyModel>(PLExpression<MyModel>.Iff(m => m.P, m => !(!m.Q && !m.R))).Should().BeEquivalentTo(new
            {
                Clauses = new[]
                {
                    new
                    {
                        IsDefiniteClause = false,
                        IsGoalClause = false,
                        IsHornClause = false,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = true },
                            new { AtomicSentence = new { Symbol = "Q" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "R" }, IsNegated = false },
                        }
                    },
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "Q" }, IsNegated = true }
                        }
                    },
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "R" }, IsNegated = true }
                        }
                    }
                }
            });
        }

        [Fact]
        public void ConstructionOfNonNormalExpression2()
        {
            // NB: We don't remove the trivially true clauses (e.g. P ∨ ¬P) - none of the source material
            // references this as being part of the normalisation process. But we probably should..
            new CNFExpression<MyModel>(m => (m.P && m.Q) || (!m.P && !m.Q)).Should().BeEquivalentTo(new
            {
                Clauses = new[]
                {
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = true },
                        }
                    },
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "Q" }, IsNegated = true },
                        }
                    },
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "Q" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "P" }, IsNegated = true }
                        }
                    },
                    new
                    {
                        IsDefiniteClause = true,
                        IsGoalClause = false,
                        IsHornClause = true,
                        IsUnitClause = false,
                        Literals = new[]
                        {
                            new { AtomicSentence = new { Symbol = "Q" }, IsNegated = false },
                            new { AtomicSentence = new { Symbol = "Q" }, IsNegated = true }
                        }
                    }
                }
            });
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

            // Both of these resolvents are trivially true - so largely useless - should the method return no resolvents in this case?
            // Are all cases where more than one resolvent would be returned useless? Should the method return a (potentially null) clause instead of a enumerable?
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
}
