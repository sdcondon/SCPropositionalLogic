using Xunit;
using static LinqToKnowledgeBase.PropositionalLogic.PLExpression<LinqToKnowledgeBase.PropositionalLogic.KnowledgeBases.TruthTableKnowledgeBaseTests.Model>;

namespace LinqToKnowledgeBase.PropositionalLogic.KnowledgeBases
{
    public class TruthTableKnowledgeBaseTests
    {
        [Fact]
        public void Test1()
        {
            var kb = new TruthTableKnowledgeBase<Model>();
            kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend)); 
            kb.Tell(m => m.ItIsSaturday);

            Assert.True(kb.Ask(m => m.ItIsTheWeekend));
            Assert.False(kb.Ask(m => m.ItIsSunny));
        }

        public class Model
        {
            public bool ItIsSaturday { get; set; }
            public bool ItIsTheWeekend { get; set; }
            public bool ItIsSunny { get; set; }
        }
    }
}
