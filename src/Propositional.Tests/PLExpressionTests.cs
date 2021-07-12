////using System;
////using System.Linq.Expressions;
using Xunit;

namespace LinqToKnowledgeBase.PropositionalLogic
{
    public class PLExpressionTests
    {
        [Fact]
        public void Implies()
        {
            // Equivalent but less easy to read..
            //// m => !m.TodayIsSaturday || m.ItIsTheWeekend; 
            // Would be nice if we could do this with Implies as an extension method, but can't apply member access operator (.) direct to lambdas - need to cast first..:
            //// (m => m.TodayIsSaturday).Implies(m => m.ItIsTheWeekend);
            // ..but casting looks like this - horrendous (though slightly nicer if you use an alias for Expression<Predicate<TModel>>)..
            //// ((Expression<Predicate<MyModel>>)(m => m.TodayIsSaturday)).Implies(m => m.ItIsTheWeekend);
            var predicate = PLExpression<Model>.Implies(m => m.P, m => m.Q).Compile();

            // Verify the truth table:
            Assert.True(predicate(new Model { P = false, Q = false }));
            Assert.True(predicate(new Model { P = false, Q = true }));
            Assert.False(predicate(new Model { P = true, Q = false }));
            Assert.True(predicate(new Model { P = true, Q = true }));
        }

        [Fact]
        public void Iff()
        {
            var predicate = PLExpression<Model>.Iff(m => m.P, m => m.Q).Compile();

            // Verify the truth table:
            Assert.True(predicate(new Model { P = false, Q = false }));
            Assert.False(predicate(new Model { P = false, Q = true }));
            Assert.False(predicate(new Model { P = true, Q = false }));
            Assert.True(predicate(new Model { P = true, Q = true }));
        }

        private class Model
        {
            public bool P { get; set; }
            public bool Q { get; set; }
        }
    }
}
