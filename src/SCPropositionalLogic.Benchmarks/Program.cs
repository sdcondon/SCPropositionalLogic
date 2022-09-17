using BenchmarkDotNet.Running;
using System.Reflection;

namespace SCPropositionalLogic.Benchmarks
{
    public class Program
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
    }
}