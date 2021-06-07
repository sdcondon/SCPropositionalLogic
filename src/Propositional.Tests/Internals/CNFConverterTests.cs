using Xunit;

namespace LinqToKB.Propositional.Internals
{
    public class CNFConverterTests
    {
        [Fact]
        public void Smoke()
        {
            var original = PLExpression<MyModel>.Iff(m => m.L, m => m.R1 || m.R2);
            var converted = new CNFConverter().VisitAndConvert(original.Body, null);
            Assert.Equal(
                "((IsFalse(m.L) OrElse (m.R1 OrElse m.R2)) AndAlso ((IsFalse(m.R1) OrElse m.L) AndAlso (IsFalse(m.R2) OrElse m.L)))",
                converted.ToString());
        }

        private class MyModel
        {
            public bool L { get; set; }
            public bool R1 { get; set; }
            public bool R2 { get; set; }
        }
    }
}
