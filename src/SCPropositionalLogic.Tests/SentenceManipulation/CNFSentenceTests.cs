using FluentAssertions;
using FlUnit;
using static SCPropositionalLogic.SentenceCreation.SentenceFactory;

namespace SCPropositionalLogic.SentenceManipulation
{
    public static class CNFSentenceTests
    {
        private static Proposition P => new Proposition("P");
        private static Proposition Q => new Proposition("Q");
        private static Proposition R => new Proposition("R");

        public static Test ConstructionOfUnitClause => TestThat
            .When(() => new CNFSentence(P))
            .ThenReturns(sentence => sentence.Should().BeEquivalentTo(new
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
                            new { Proposition = P, IsNegated = false },
                        }
                    }
                }
            }));

        public static Test ConstructionOfNegatedUnitClause => TestThat
            .When(() => new CNFSentence(Not(P)))
            .ThenReturns(sentence => sentence.Should().BeEquivalentTo(new
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
                            new { Proposition = P, IsNegated = true },
                        }
                    }
                }
            }));

        public static Test ConstructionOfNonNormalExpression => TestThat
            .When(() => new CNFSentence(Iff(P, Not(And(Not(Q), Not(R))))))
            .ThenReturns(sentence => sentence.Should().BeEquivalentTo(new
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
                            new { Proposition = P, IsNegated = true },
                            new { Proposition = Q, IsNegated = false },
                            new { Proposition = R, IsNegated = false },
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
                            new { Proposition = P, IsNegated = false },
                            new { Proposition = Q, IsNegated = true }
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
                            new { Proposition = P, IsNegated = false },
                            new { Proposition = R, IsNegated = true }
                        }
                    }
                }
            }));

        public static Test ConstructionOfNonNormalExpression2 => TestThat
            .When(() => new CNFSentence(Or(And(P, Q), And(Not(P), Not(Q)))))
            .ThenReturns(sentence => sentence.Should().BeEquivalentTo(new
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
                            new { Proposition = P, IsNegated = false },
                            new { Proposition = P, IsNegated = true },
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
                            new { Proposition = P, IsNegated = false },
                            new { Proposition = Q, IsNegated = true },
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
                            new { Proposition = Q, IsNegated = false },
                            new { Proposition = P, IsNegated = true }
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
                            new { Proposition = Q, IsNegated = false },
                            new { Proposition = Q, IsNegated = true }
                        }
                    }
                }
            }));
    }
}
