using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SCPropositionalLogic.Benchmarks.Alternatives;
using SCPropositionalLogic.Inference;
using SCPropositionalLogic.LanguageIntegration;
using System.Reflection;
using static SCPropositionalLogic.LanguageIntegration.Operators;
using AiAModernApproachResolutionKB = SCPropositionalLogic.Benchmarks.Alternatives.FromAiAModernApproach.ResolutionKnowledgeBase;

namespace SCPropositionalLogic.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class KnowledgeBaseBenchmarks
    {
        //// NB: All the adapter-y stuff here (simply because I'm lazy..) probably makes these slightly unfair tests (gives LinqKnowledgeBase a slight advantage).
        private readonly ILinqKnowledgeBase<MyModel> truthTable = new LinqKnowledgeBase<MyModel>(new TruthTableKnowledgeBase());
        private readonly ILinqKnowledgeBase<MyModel> linqTruthTable = new LinqTruthTableKnowledgeBase<MyModel>();
        private readonly ILinqKnowledgeBase<MyModel> nonCompilingTruthTable = new LinqKnowledgeBase<MyModel>(new NonCompilingTruthTableKnowledgeBase());
        private readonly ILinqKnowledgeBase<MyModel> resolution = new LinqKnowledgeBase<MyModel>(new ResolutionKnowledgeBase());
        private readonly ILinqKnowledgeBase<MyModel> altResolution = new LinqKnowledgeBase<MyModel>(new AiAModernApproachResolutionKB());

        public KnowledgeBaseBenchmarks()
        {
            TellForwardChain(truthTable);
            TellForwardChain(linqTruthTable);
            TellForwardChain(nonCompilingTruthTable);
            TellForwardChain(resolution);
            TellForwardChain(altResolution);
        }

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
        public bool TruthTableForwardChain() => Ask(truthTable);
        
        // why is this so much faster than regular TT - dictionary setting & lookups can't be adding THAT much, surely?
        // Or maybe it can - more gen 0 collections.. Will try something more direct than a dictionary at some point.
        [Benchmark]
        public bool LinqTruthTableForwardChain() => Ask(linqTruthTable);

        [Benchmark]
        public bool NonCompilingTableForwardChain() => Ask(nonCompilingTruthTable);

        [Benchmark]
        public bool ResolutionForwardChain() => Ask(resolution);

        [Benchmark]
        public bool AiAModernApproachResolutionForwardChain() => Ask(altResolution);

        private static void TellForwardChain(ILinqKnowledgeBase<MyModel> kb)
        {
            kb.Tell(m => m.Fact1);
            kb.Tell(m => If(m.Fact1, m.Fact2));
            kb.Tell(m => If(m.Fact2, m.Fact3));
            kb.Tell(m => If(m.Fact3, m.Fact4));
            kb.Tell(m => If(m.Fact4, m.Fact5));
            kb.Tell(m => If(m.Fact5, m.Fact6));
            kb.Tell(m => If(m.Fact6, m.Fact7));
            kb.Tell(m => If(m.Fact7, m.Fact8));
            kb.Tell(m => If(m.Fact8, m.Fact9));
            kb.Tell(m => If(m.Fact9, m.Fact10));
            kb.Tell(m => If(m.Fact10, m.Fact11));
            kb.Tell(m => If(m.Fact11, m.Fact12));
            kb.Tell(m => If(m.Fact12, m.Fact13));
            kb.Tell(m => If(m.Fact13, m.Fact14));
            kb.Tell(m => If(m.Fact14, m.Fact15));
            kb.Tell(m => If(m.Fact15, m.Fact16));
            kb.Tell(m => If(m.Fact16, m.Fact17));
            kb.Tell(m => If(m.Fact17, m.Fact18));
            kb.Tell(m => If(m.Fact18, m.Fact19));
            kb.Tell(m => If(m.Fact19, m.Fact20));
        }

        private static bool Ask(ILinqKnowledgeBase<MyModel> kb)
        {
            bool result = kb.Ask(m => m.Fact20);
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
            public bool Fact11 { get; set; }
            public bool Fact12 { get; set; }
            public bool Fact13 { get; set; }
            public bool Fact14 { get; set; }
            public bool Fact15 { get; set; }
            public bool Fact16 { get; set; }
            public bool Fact17 { get; set; }
            public bool Fact18 { get; set; }
            public bool Fact19 { get; set; }
            public bool Fact20 { get; set; }
        }
    }
}