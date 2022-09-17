using FluentAssertions;
using FlUnit;

namespace SCPropositionalLogic
{
    public static class PropositionTests
    {
        public static Test CloneComparison => TestThat
            .When(() => new
            {
                Representation1 = new Proposition("A"),
                Representation2 = new Proposition("A"),
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
    }
}
