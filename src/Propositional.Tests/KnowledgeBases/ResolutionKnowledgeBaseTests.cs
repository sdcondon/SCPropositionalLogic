using Xunit;
using static LinqToKnowledgeBase.Propositional.PLExpression<LinqToKnowledgeBase.Propositional.KnowledgeBases.ResolutionKnowledgeBaseTests.DaysModel>;

namespace LinqToKnowledgeBase.Propositional.KnowledgeBases
{
    public class ResolutionKnowledgeBaseTests
    {
        [Fact]
        public void Test1()
        {
            var kb = new ResolutionKnowledgeBase<DaysModel>();
            kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend));
            kb.Tell(m => m.ItIsSaturday);

            Assert.True(kb.Ask(m => m.ItIsTheWeekend));
            Assert.False(kb.Ask(m => m.ItIsJanuary));
        }

        [Fact]
        public void Test2()
        {
            var kb = new ResolutionKnowledgeBase<DaysModel>();
            kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend));
            kb.Tell(m => !m.ItIsTheWeekend);

            Assert.True(kb.Ask(m => !m.ItIsSaturday));
            Assert.False(kb.Ask(m => m.ItIsJanuary));
        }

        public class DaysModel
        {
            public bool ItIsSaturday { get; set; }
            public bool ItIsTheWeekend { get; set; }
            public bool ItIsJanuary { get; set; }
        }
    }
}
