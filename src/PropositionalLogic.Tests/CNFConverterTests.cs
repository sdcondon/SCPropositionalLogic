using Xunit;

namespace LinqToKB.PropositionalLogic
{
    public class CNFConverterTests
    {
        [Fact]
        public void Lambda()
        {
            var original = PLExpression<MyModel>.Iff(m => m.L, m => !(!m.R1 && !m.R2));
            var converted = new CNFConverter().VisitAndConvert(original, null);
            Assert.Equal(
                "m => ((Not(m.L) OrElse (m.R1 OrElse m.R2)) AndAlso ((Not(m.R1) OrElse m.L) AndAlso (Not(m.R2) OrElse m.L)))",
                converted.ToString());
        }

        [Fact]
        public void LambdaBody()
        {
            var original = PLExpression<MyModel>.Iff(m => m.L, m => !(!m.R1 && !m.R2));
            var converted = new CNFConverter().VisitAndConvert(original.Body, null);
            Assert.Equal(
                "((Not(m.L) OrElse (m.R1 OrElse m.R2)) AndAlso ((Not(m.R1) OrElse m.L) AndAlso (Not(m.R2) OrElse m.L)))",
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
