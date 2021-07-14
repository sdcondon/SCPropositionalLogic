using Xunit;
using static LinqToKB.PropositionalLogic.PLExpression<LinqToKB.PropositionalLogic.KnowledgeBases.ResolutionKnowledgeBaseTests.I3Facts>;

namespace LinqToKB.PropositionalLogic.KnowledgeBases
{
    public class ResolutionKnowledgeBaseTests
    {
        [Fact]
        public void TrivialConclusions1()
        {
            var kb = new ResolutionKnowledgeBase<I3Facts>();
            kb.Tell(Implies(m => m.P, m => m.Q));
            kb.Tell(m => m.P);

            Assert.True(kb.Ask(m => m.Q));
            Assert.False(kb.Ask(m => m.R));
            Assert.False(kb.Ask(m => !m.R));
        }

        [Fact]
        public void TrivialConclusions2()
        {
            var kb = new ResolutionKnowledgeBase<I3Facts>();
            kb.Tell(Implies(m => m.P, m => m.Q));
            kb.Tell(m => !m.Q);

            Assert.True(kb.Ask(m => !m.P));
            Assert.False(kb.Ask(m => m.R));
            Assert.False(kb.Ask(m => !m.R));
        }

        [Fact]
        public void NonTrivialConclusion1()
        {
            var kb = new ResolutionKnowledgeBase<I3Facts>();
            kb.Tell(Implies(m => m.P, m => m.R));
            kb.Tell(Implies(m => m.Q, m => m.R));
            kb.Tell(Implies(m => !m.P, m => m.Q));

            Assert.True(kb.Ask(m => m.R));
        }

        [Fact]
        public void NonTrivialConclusion2()
        {
            var kb = new ResolutionKnowledgeBase<I3Facts>();
            kb.Tell(m => m.P || m.Q);
            kb.Tell(m => m.P || !m.Q);
            kb.Tell(m => !m.P || m.Q);

            Assert.True(kb.Ask(m => m.P && m.Q));
        }

        public interface I3Facts
        {
            bool P { get; }
            bool Q { get; }
            bool R { get; }
        }
    }
}
