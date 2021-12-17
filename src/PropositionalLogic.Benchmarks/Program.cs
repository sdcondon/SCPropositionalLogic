using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LinqToKB.PropositionalLogic.Benchmarks.Alternatives;
using LinqToKB.PropositionalLogic.KnowledgeBases;
using LinqToKB.PropositionalLogic.LanguageIntegration;
using System.Reflection;
using static LinqToKB.PropositionalLogic.LanguageIntegration.Operators;
using AiAModernApproachResolutionKB = LinqToKB.PropositionalLogic.Benchmarks.Alternatives.FromAiAModernApproach.ResolutionKnowledgeBase;

namespace LinqToKB.PropositionalLogic.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class KnowledgeBaseBenchmarks
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <remarks>
        /// See https://benchmarkdotnet.org/articles/guides/console-args.html (or run app with --help) for parameter documentation.
        /// </remarks>
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
        }

        [Benchmark]
        public bool TruthTableForwardChain() => ForwardChain(new LinqKnowledgeBase<MyModel>(new TruthTableKnowledgeBase()));

        [Benchmark]
        public bool LinqTruthTableForwardChain() => ForwardChain(new LinqTruthTableKnowledgeBase<MyModel>());

        [Benchmark]
        public bool ResolutionForwardChain() => ForwardChain(new LinqKnowledgeBase<MyModel>(new ResolutionKnowledgeBase()));

        [Benchmark]
        public bool AiAModernApproachResolutionForwardChain() => ForwardChain(new LinqKnowledgeBase<MyModel>(new AiAModernApproachResolutionKB()));

        private static bool ForwardChain(ILinqKnowledgeBase<MyModel> kb)
        {
            kb.Tell(m => If(m.Fact1, m.Fact2));
            kb.Tell(m => If(m.Fact2, m.Fact3));
            kb.Tell(m => If(m.Fact3, m.Fact4));
            kb.Tell(m => If(m.Fact4, m.Fact5));
            kb.Tell(m => If(m.Fact5, m.Fact6));
            kb.Tell(m => If(m.Fact6, m.Fact7));
            kb.Tell(m => If(m.Fact7, m.Fact8));
            kb.Tell(m => If(m.Fact8, m.Fact9));
            kb.Tell(m => If(m.Fact9, m.Fact10));
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