using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LinqToKB.Propositional.InferenceStrategies;
using static LinqToKB.Propositional.PLExpression<LinqToKB.Propositional.Benchmarks.InferenceStrategyBenchmarks.MyModel>;

namespace LinqToKB.Propositional.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class InferenceStrategyBenchmarks
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        public static void Main()
        {
            BenchmarkRunner.Run<InferenceStrategyBenchmarks>();
        }

        [Benchmark]
        public bool TruthTable()
        {
            var kb = MakeKnowledgeBase();

            kb.InferenceStrategy = new TruthTableInferenceStrategy<MyModel>();
            return kb.Ask(m => m.Fact10);
        }

        private static KnowledgeBase<MyModel> MakeKnowledgeBase()
        {
            var kb = new KnowledgeBase<MyModel>();
            kb.Tell(Implies(m => m.Fact1, m => m.Fact2));
            kb.Tell(Implies(m => m.Fact2, m => m.Fact3));
            kb.Tell(Implies(m => m.Fact3, m => m.Fact4));
            kb.Tell(Implies(m => m.Fact4, m => m.Fact5));
            kb.Tell(Implies(m => m.Fact5, m => m.Fact6));
            kb.Tell(Implies(m => m.Fact6, m => m.Fact7));
            kb.Tell(Implies(m => m.Fact7, m => m.Fact8));
            kb.Tell(Implies(m => m.Fact8, m => m.Fact9));
            kb.Tell(Implies(m => m.Fact9, m => m.Fact10));
            kb.Tell(m => m.Fact1);
            return kb;
        }

        public class MyModel
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