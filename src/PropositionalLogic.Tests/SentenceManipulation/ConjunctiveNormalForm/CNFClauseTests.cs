using FluentAssertions;
using FlUnit;

namespace SCPropositionalLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public class CNFClauseTests
    {
        private static Proposition P => new Proposition("P");
        private static Proposition Q => new Proposition("Q");
        private static Proposition R => new Proposition("R");

        public static Test ResolutionOfResolvableClauses => TestThat
            .When(() =>
            {
                return CNFClause.Resolve(
                    new CNFClause(new CNFLiteral(P, true), new CNFLiteral(Q, false)),
                    new CNFClause(new CNFLiteral(P, false)));
            })
            .ThenReturns()
            .And(resolvents => resolvents.Should().BeEquivalentTo(new[]
            {
                new CNFClause(new CNFLiteral(Q, false))
            }));

        public static Test ResolutionOfResolvableClauses2 => TestThat
            .When(() =>
            {
                return CNFClause.Resolve(
                    new CNFClause(new CNFLiteral(P, true), new CNFLiteral(Q, false)),
                    new CNFClause(new CNFLiteral(P, false), new CNFLiteral(R, false)));
            })
            .ThenReturns()
            .And(resolvents => resolvents.Should().BeEquivalentTo(new[]
            {
                new CNFClause(new CNFLiteral(Q, false), new CNFLiteral(R, false))
            }));

        public static Test ResolutionOfUnresolvableClauses => TestThat
            .When(() =>
            {
                return CNFClause.Resolve(
                    new CNFClause(new CNFLiteral(P, false)),
                    new CNFClause(new CNFLiteral(Q, false)));

            })
            .ThenReturns()
            .And(resolvents => resolvents.Should().BeEmpty());


        public static Test ResolutionOfComplementaryUnitClauses => TestThat
            .When(() =>
            {
                return CNFClause.Resolve(
                    new CNFClause(new CNFLiteral(P, false)),
                    new CNFClause(new CNFLiteral(P, true)));
            })
            .ThenReturns()
            .And(resolvents => resolvents.Should().BeEquivalentTo(new[]
            {
                CNFClause.Empty
            }));

        public static Test ResolutionOfMultiplyResolvableClauses => TestThat
            .When(() =>
            {
                return CNFClause.Resolve(
                    new CNFClause(new CNFLiteral(P, false), new CNFLiteral(Q, false)),
                    new CNFClause(new CNFLiteral(P, true), new CNFLiteral(Q, true)));
            })
            .ThenReturns()
            .And(resolvents => resolvents.Should().BeEquivalentTo(new[]
            {
                // Both of these resolvents are trivially true - so largely useless - should the method return no resolvents in this case?
                // Are all cases where more than one resolvent would be returned useless? Should the method return a (potentially null) clause instead of a enumerable?
                new CNFClause(new CNFLiteral(P, false), new CNFLiteral(P, true)),
                new CNFClause(new CNFLiteral(Q, false), new CNFLiteral(Q, true))
            }));
    }
}
