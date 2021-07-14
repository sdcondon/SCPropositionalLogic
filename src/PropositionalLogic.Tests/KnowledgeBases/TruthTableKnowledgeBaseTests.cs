using Xunit;
using static LinqToKB.PropositionalLogic.PLExpression<LinqToKB.PropositionalLogic.KnowledgeBases.TruthTableKnowledgeBaseTests.Model>;

namespace LinqToKB.PropositionalLogic.KnowledgeBases
{
    public class TruthTableKnowledgeBaseTests
    {
        [Fact]
        public void TrivialConclusions1()
        {
            var kb = new TruthTableKnowledgeBase<Model>();
            kb.Tell(Implies(m => m.P, m => m.Q));
            kb.Tell(m => m.P);

            Assert.True(kb.Ask(m => m.Q));
            Assert.False(kb.Ask(m => m.R));
            Assert.False(kb.Ask(m => !m.R));
        }

        [Fact]
        public void TrivialConclusions2()
        {
            var kb = new TruthTableKnowledgeBase<Model>();
            kb.Tell(Implies(m => m.P, m => m.Q));
            kb.Tell(m => !m.Q);

            Assert.True(kb.Ask(m => !m.P));
            Assert.False(kb.Ask(m => m.R));
            Assert.False(kb.Ask(m => !m.R));
        }

        [Fact]
        public void NonTrivialConclusion1()
        {
            var kb = new TruthTableKnowledgeBase<Model>();
            kb.Tell(Implies(m => m.P, m => m.R));
            kb.Tell(Implies(m => m.Q, m => m.R));
            kb.Tell(Implies(m => !m.P, m => m.Q));

            Assert.True(kb.Ask(m => m.R));
        }

        [Fact]
        public void NonTrivialConclusion2()
        {
            var kb = new TruthTableKnowledgeBase<Model>();
            kb.Tell(m => m.P || m.Q);
            kb.Tell(m => m.P || !m.Q);
            kb.Tell(m => !m.P || m.Q);

            Assert.True(kb.Ask(m => m.P && m.Q));
        }

        public class Model
        {
            public bool P { get; set; }
            public bool Q { get; set; }
            public bool R { get; set; }
        }
    }
}
