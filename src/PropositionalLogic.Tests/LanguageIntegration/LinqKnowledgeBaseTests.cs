using FlUnit;
using FluentAssertions;
using static LinqToKB.PropositionalLogic.LanguageIntegration.Operators;
using LinqToKB.PropositionalLogic.KnowledgeBases;

namespace LinqToKB.PropositionalLogic.LanguageIntegration
{
    public static class LinqKnowledgeBaseTests
    {
        private interface IModel
        {
            public bool P { get; }
            public bool Q { get; }
            public bool R { get; }
        }

        public static Test TrivialConclusions1 => TestThat
            .When(() =>
            {
                var kb = new LinqKnowledgeBase<IModel>(new ResolutionKnowledgeBase());
                kb.Tell(m => If(m.P, m.Q));
                kb.Tell(m => m.P);
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(m => m.Q).Should().BeTrue())
            .And(kb => kb.Ask(m => m.R).Should().BeFalse()) // Truthiness of R cannot be determined..
            .And(kb => kb.Ask(m => !m.R).Should().BeFalse()); // Truthiness of R cannot be determined..

        public static Test TrivialConclusions2 => TestThat
            .When(() =>
            {
                var kb = new LinqKnowledgeBase<IModel>(new ResolutionKnowledgeBase());
                kb.Tell(m => If(m.P, m.Q));
                kb.Tell(m => !m.Q);
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(m => !m.P).Should().BeTrue())
            .And(kb => kb.Ask(m => m.R).Should().BeFalse())
            .And(kb => kb.Ask(m => !m.R).Should().BeFalse());

        public static Test NonTrivialConclusion1 => TestThat
            .When(() =>
            {
                var kb = new LinqKnowledgeBase<IModel>(new ResolutionKnowledgeBase());
                kb.Tell(m => If(m.P, m.R));
                kb.Tell(m => If(m.Q, m.R));
                kb.Tell(m => If(!m.P, m.Q));
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(m => m.R).Should().BeTrue());

        public static Test NonTrivialConclusion2 => TestThat
            .When(() =>
            {
                var kb = new LinqKnowledgeBase<IModel>(new ResolutionKnowledgeBase());
                kb.Tell(m => m.P || m.Q);
                kb.Tell(m => m.P || !m.Q);
                kb.Tell(m => !m.P || m.Q);
                return kb;
            })
            .ThenReturns()
            .And(kb => kb.Ask(m => m.P && m.Q));
    }
}
