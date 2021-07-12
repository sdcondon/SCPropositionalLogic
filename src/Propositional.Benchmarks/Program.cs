using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LinqToKnowledgeBase.PropositionalLogic.KnowledgeBases;
using System.Reflection;
using static LinqToKnowledgeBase.PropositionalLogic.PLExpression<LinqToKnowledgeBase.PropositionalLogic.Benchmarks.KnowledgeBaseBenchmarks.MyModel>;
using AiAModernApproachResolutionKB = LinqToKnowledgeBase.PropositionalLogic.Benchmarks.Alternatives.FromAiAModernApproach.ResolutionKnowledgeBase<LinqToKnowledgeBase.PropositionalLogic.Benchmarks.KnowledgeBaseBenchmarks.MyModel>;

namespace LinqToKnowledgeBase.PropositionalLogic.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class KnowledgeBaseBenchmarks
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            // See https://benchmarkdotnet.org/articles/guides/console-args.html (or run app with --help)
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
        }

        [Benchmark]
        public bool TruthTableForwardChain() => ForwardChain(new TruthTableKnowledgeBase<MyModel>());

        [Benchmark]
        public bool ResolutionForwardChain() => ForwardChain(new ResolutionKnowledgeBase<MyModel>());

        [Benchmark]
        public bool AiAModernApproachResolutionForwardChain() => ForwardChain(new AiAModernApproachResolutionKB());

        private static bool ForwardChain(IKnowledgeBase<MyModel> kb)
        {
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

            bool result = kb.Ask(m => m.Fact10);
            if (!result)
            {
                throw new System.Exception("Got the wrong result!");
            }

            return result;
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