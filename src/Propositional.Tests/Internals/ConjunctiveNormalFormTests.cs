using Xunit;

namespace LinqToKB.Propositional.Internals
{
    public class ConjunctiveNormalFormTests
    {
        [Fact]
        public void Smoke()
        {
            var original = PLExpression<MyModel>.Iff(m => m.L, m => m.R1 || m.R2);
            var converted = new ConjunctiveNormalFormConverter().VisitAndConvert(original, null);
            Assert.Equal(
                "((IsFalse(Param_0.L) OrElse (Param_0.R1 OrElse Param_0.R2)) AndAlso ((IsFalse(Param_0.R1) OrElse Param_0.L) AndAlso (IsFalse(Param_0.R2) OrElse Param_0.L)))",
                converted.Body.ToString());
        }

        private class MyModel
        {
            public bool L { get; set; }
            public bool R1 { get; set; }
            public bool R2 { get; set; }
        }
    }
}
