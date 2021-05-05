////using System;
////using System.Linq.Expressions;
using System;
using Xunit;
using static LinqToKB.Propositional.PLExpression<LinqToKB.Propositional.Smoke.MyDomain>;

namespace LinqToKB.Propositional
{
    public class Smoke
    {
        [Fact]
        public void Test1()
        {
            var kb = new KnowledgeBase<MyDomain>();

            // Equivalent but less easy to read..
            ////kb.Tell(m => !m.TodayIsSaturday || m.ItIsTheWeekend); 
            // Would be nice if we could do this with Implies as an extension method, but can't apply member access operator (.) direct to lambdas - need to cast first..:
            ////kb.Tell((m => m.TodayIsSaturday).Implies(m => m.ItIsTheWeekend));
            // ..but casting looks like this - horrendous (though slightly nicer if you use an alias for Expression<Predicate<TDomain>>)..
            ////kb.Tell(((Expression<Predicate<MyDomain>>)(m => m.TodayIsSaturday)).Implies(m => m.ItIsTheWeekend));
            kb.Tell(Implies(m => m.TodayIsSaturday, m => m.ItIsTheWeekend)); 

            kb.Tell(m => m.TodayIsSaturday);

            Assert.True(kb.Ask(m => m.ItIsTheWeekend));
            Assert.False(kb.Ask(m => m.ItIsSunny));
        }

        public class MyDomain
        {
            public bool TodayIsSaturday { get; set; }
            public bool ItIsTheWeekend { get; set; }
            public bool ItIsSunny { get; set; }
        }
    }
}
