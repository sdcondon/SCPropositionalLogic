using Xunit;
using static LinqToKnowledgeBase.PropositionalLogic.PLExpression<LinqToKnowledgeBase.PropositionalLogic.KnowledgeBases.ResolutionKnowledgeBaseTests.DaysModel>;

namespace LinqToKnowledgeBase.PropositionalLogic.KnowledgeBases
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

        [Fact]
        public void MakeResolutionKnowledgeBase()
        {
            var kb = new ResolutionKnowledgeBase<ForwardChainModel>();
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact1, m => m.Fact2));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact2, m => m.Fact3));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact3, m => m.Fact4));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact4, m => m.Fact5));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact5, m => m.Fact6));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact6, m => m.Fact7));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact7, m => m.Fact8));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact8, m => m.Fact9));
            kb.Tell(PLExpression<ForwardChainModel>.Implies(m => m.Fact9, m => m.Fact10));
            kb.Tell(m => m.Fact1);
            Assert.True(kb.Ask(m => m.Fact10));
        }

        public class ForwardChainModel
        {
            public bool Fact1 { get; set; }
            public bool Fact2 { get; set; }
            public bool Fact3 { get; set; }
            public bool Fact4 { get; set; }
            public bool Fact5 { get; set; }
            public bool Fact6 { get; set; }
            public bool Fact7 { get; set; }
            public bool Fact8 { get; set; }
            public bool Fact9 { get; set; }
            public bool Fact10 { get; set; }

        }
    }
}
