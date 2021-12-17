using FluentAssertions;
using FlUnit;
using System;
using System.Linq.Expressions;
using System.Reflection;
using static SCPropositionalLogic.LanguageIntegration.Operators;

namespace SCPropositionalLogic.LanguageIntegration
{
    public class SentenceFactoryTests
    {
        private interface IModel
        {
            bool Proposition1 { get; }
            bool Proposition2 { get; }
        }

        private static readonly MemberInfo proposition1 = typeof(IModel).GetProperty(nameof(IModel.Proposition1));
        private static readonly MemberInfo proposition2 = typeof(IModel).GetProperty(nameof(IModel.Proposition2));

        private record TestCase(Expression<Predicate<IModel>> Expression, Sentence ExpectedSentence);

        public static Test Creation => TestThat
            .GivenEachOf(() => new[]
            {
                new TestCase(
                    Expression: d => d.Proposition1 && d.Proposition2,
                    ExpectedSentence: new Conjunction(
                        new MemberProposition(proposition1),
                        new MemberProposition(proposition2))),

                new TestCase(
                    Expression: d => d.Proposition1 || d.Proposition2,
                    ExpectedSentence: new Disjunction(
                        new MemberProposition(proposition1),
                        new MemberProposition(proposition2))),

                new TestCase(
                    Expression: d => Iff(d.Proposition1, d.Proposition2),
                    ExpectedSentence: new Equivalence(
                        new MemberProposition(proposition1),
                        new MemberProposition(proposition2))),

                new TestCase(
                    Expression: d => If(d.Proposition1, d.Proposition2),
                    ExpectedSentence: new Implication(
                        new MemberProposition(proposition1),
                        new MemberProposition(proposition2))),

                new TestCase(
                    Expression: d => !d.Proposition1,
                    ExpectedSentence: new Negation(
                        new MemberProposition(proposition1))),
            })
            .When(tc => SentenceFactory.Create<IModel>(tc.Expression))
            .ThenReturns((tc, sentence) =>
            {
                sentence.Should().BeEquivalentTo(tc.ExpectedSentence, o => o.RespectingRuntimeTypes());
            });
    }
}
