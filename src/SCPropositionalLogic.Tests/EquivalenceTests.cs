﻿using FluentAssertions;
using FlUnit;
using static SCPropositionalLogic.SentenceCreation.SentenceFactory;

namespace SCPropositionalLogic
{
    public static class EquivalenceTests
    {
        public static Test CloneComparison => TestThat
            .When(() => new
            {
                Representation1 = new Equivalence(A, B),
                Representation2 = new Equivalence(A, B)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());

        public static Test CommutationComparison => TestThat
            .When(() => new
            {
                Representation1 = new Equivalence(A, B),
                Representation2 = new Equivalence(B, A)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().NotBe(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeFalse())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeFalse());
    }
}
