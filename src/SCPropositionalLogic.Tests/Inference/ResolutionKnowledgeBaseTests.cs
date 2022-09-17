using FlUnit;
using FluentAssertions;
using static SCPropositionalLogic.SentenceCreation.SentenceFactory;

namespace SCPropositionalLogic.Inference
{
    public static class ResolutionKnowledgeBaseTests
    {
        public static Test TrivialConclusions1 => TestThat
            .When(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(If(P, Q));
                kb.Tell(P);
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(Q).Should().BeTrue()) // Modus Ponens..
            .And(kb => kb.Ask(R).Should().BeFalse()) // Truthiness of R cannot be determined..
            .And(kb => kb.Ask(Not(R)).Should().BeFalse()); // Truthiness of R cannot be determined..

        public static Test TrivialConclusions2 => TestThat
            .When(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(If(P, Q));
                kb.Tell(Not(Q));
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(Not(P)).Should().BeTrue()) // Contrapositive..
            .And(kb => kb.Ask(R).Should().BeFalse()) // Truthiness of R cannot be determined..
            .And(kb => kb.Ask(Not(R)).Should().BeFalse()); // Truthiness of R cannot be determined..

        public static Test NonTrivialConclusion1 => TestThat
            .When(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(If(P, R));
                kb.Tell(If(Q, R));
                kb.Tell(If(Not(P), Q));
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(R).Should().BeTrue());

        public static Test NonTrivialConclusion2 => TestThat
            .When(() =>
            {
                var kb = new ResolutionKnowledgeBase();
                kb.Tell(Or(P, Q));
                kb.Tell(Or(P, Not(Q)));
                kb.Tell(Or(Not(P), Q));
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(And(P, Q)));
    }
}
