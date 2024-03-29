﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SCPropositionalLogic.LanguageIntegration
{
    /// <summary>
    /// Static factory methods for creating <see cref="Sentence"/> instances from LINQ expressions operating on an object representing the model.
    /// The conventions used can be found in library documentation.
    /// </summary>
    public static class SentenceFactory
    {
        private static readonly MethodInfo IfMethod;
        private static readonly MethodInfo IffMethod;

        static SentenceFactory()
        {
            IfMethod = typeof(Operators).GetMethod(nameof(Operators.If));
            IffMethod = typeof(Operators).GetMethod(nameof(Operators.Iff));
        }

        /// <summary>
        /// Creates and returns the <see cref="Sentence"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible models.
        /// </summary>
        /// <typeparam name="TModel">The type representing the model.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <returns>The created sentence.</returns>
        public static Sentence Create<TModel>(Expression<Predicate<TModel>> lambda)
        {
            if (!TryCreate<TModel>(lambda, out var sentence))
            {
                throw new ArgumentException("Expression is not convertible to a sentence of propositional logic", nameof(sentence));
            }

            return sentence;
        }

        /// <summary>
        /// Tries to create the <see cref="Sentence"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <typeparam name="TModel">The type representing the model.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate<TModel>(Expression<Predicate<TModel>> lambda, out Sentence sentence)
        {
            return TryCreateSentence<TModel>(lambda.Body, out sentence);
        }

        private static bool TryCreateSentence<TModel>(Expression expression, out Sentence sentence)
        {
            // TODO-USABILITY: might be nice to return more info than just "false" - but can't rely on exceptions due to the way this works.
            return
                // Complex sentences:
                TryCreateNegation<TModel>(expression, out sentence)
                || TryCreateConjunction<TModel>(expression, out sentence)
                || TryCreateDisjunction<TModel>(expression, out sentence)
                || TryCreateEquivalence<TModel>(expression, out sentence)
                || TryCreateImplication<TModel>(expression, out sentence)
                // Atomic sentences:
                || TryCreateProposition<TModel>(expression, out sentence);
        }

        /// <summary>
        /// Tries to create a <see cref="Conjunction"/> from an expression acting on the model of the form:
        /// <code>{expression} {&amp;&amp; or &amp;} {expression}</code>
        /// </summary>
        private static bool TryCreateConjunction<TModel>(Expression expression, out Sentence sentence)
        {
            if (expression is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.And)
                && TryCreateSentence<TModel>(binaryExpr.Left, out var left)
                && TryCreateSentence<TModel>(binaryExpr.Right, out var right))
            {
                sentence = new Conjunction(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Disjunction"/> from an expression acting on the model of the form:
        /// <code>{expression} {|| or |} {expression}</code>
        /// </summary>
        private static bool TryCreateDisjunction<TModel>(Expression expression, out Sentence sentence)
        {
            if (expression is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.OrElse || binaryExpr.NodeType == ExpressionType.Or)
                && TryCreateSentence<TModel>(binaryExpr.Left, out var left)
                && TryCreateSentence<TModel>(binaryExpr.Right, out var right))
            {
                sentence = new Disjunction(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Equivalence"/> from an expression acting on the model of the form:
        /// <code>Operators.Iff({expression}, {expression})</code>
        /// (Consumers are encouraged to include <c>using static SCPropositionalLogic.LanguageIntegration.Operators;</c> to make this a little shorter)
        /// </summary>
        private static bool TryCreateEquivalence<TModel>(Expression expression, out Sentence sentence)
        {
            // TODO-FEATURE: Would it be reasonable to also accept {sentence} == {sentence} here?

            if (expression is MethodCallExpression methodCallExpr && MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, IffMethod)
                && TryCreateSentence<TModel>(methodCallExpr.Arguments[0], out var equivalent1)
                && TryCreateSentence<TModel>(methodCallExpr.Arguments[1], out var equivalent2))
            {
                sentence = new Equivalence(equivalent1, equivalent2);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Implication"/> from an expression acting on the model of the form:
        /// <code>Operators.If({expression}, {expression})</code>
        /// (Consumers are encouraged to include <c>using static SCPropositionalLogic.LanguageIntegration.Operators;</c> to make this a little shorter)
        /// </summary>
        private static bool TryCreateImplication<TModel>(Expression expression, out Sentence sentence)
        {
            if (expression is MethodCallExpression methodCallExpr && MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, IfMethod)
                && TryCreateSentence<TModel>(methodCallExpr.Arguments[0], out var antecedent)
                && TryCreateSentence<TModel>(methodCallExpr.Arguments[1], out var consequent))
            {
                sentence = new Implication(antecedent, consequent);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Negation"/> from an expression acting on the model of the form:
        /// <code>!{expression}</code>
        /// We also interpret <c>!=</c> as a negation of an equality.
        /// </summary>
        private static bool TryCreateNegation<TModel>(Expression expression, out Sentence sentence)
        {
            if (expression is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Not
                && TryCreateSentence<TModel>(unaryExpr.Operand, out var operand))
            {
                sentence = new Negation(operand);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="MemberProposition"/> from an expression acting on the model that
        /// is a boolean-valued property or method call on an element object, or a boolean-valued property or method call on a domain object.
        /// </summary>
        private static bool TryCreateProposition<TModel>(Expression expression, out Sentence sentence)
        {
            if (expression.Type != typeof(bool))
            {
                sentence = null;
                return false;
            }

            if (expression is MemberExpression memberExpr && memberExpr.Expression != null) // Non-static field or property access
            {
                if (memberExpr.Expression.Type == typeof(TModel)) // todo: no guarantee that this is the model param of the original lambda... Make me robust! requires passing domain param down through the whole process though..
                {
                    // Boolean-valued property access on the model parameter is interpreted as a ground predicate
                    sentence = new MemberProposition(memberExpr.Member);
                    return true;
                }
            }

            sentence = null;
            return false;
        }
    }
}
