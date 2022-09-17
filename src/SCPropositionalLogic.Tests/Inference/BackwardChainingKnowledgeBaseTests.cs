using FlUnit;
using FluentAssertions;
using static SCPropositionalLogic.SentenceCreation.SentenceFactory;

namespace SCPropositionalLogic.Inference
{
    public static class BackwardChainingKnowledgeBaseTests
    {
        public static Test BasicBehaviour => TestThat
            .When(() =>
            {
                var kb = new BackwardChainingKnowledgeBase();
                kb.Tell(If(And(Q, R), P));
                kb.Tell(If(S, P));
                kb.Tell(If(T, Q));
                kb.Tell(If(U, Q));
                kb.Tell(U);
                kb.Tell(R);
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(P).Should().BeTrue())
            .And(kb => kb.Ask(Q).Should().BeTrue())
            .And(kb => kb.Ask(R).Should().BeTrue())
            .And(kb => kb.Ask(S).Should().BeFalse())
            .And(kb => kb.Ask(T).Should().BeFalse())
            .And(kb => kb.Ask(U).Should().BeTrue());
    }
}
