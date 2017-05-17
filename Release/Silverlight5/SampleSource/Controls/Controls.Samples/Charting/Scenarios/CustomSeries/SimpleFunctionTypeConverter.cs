// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Sle = System.Linq.Expressions;
using System.Text;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// TypeConverter used for creating single variable functions from simple
    /// arithmetic expressions.
    /// </summary>
    public class SimpleFunctionTypeConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the SimpleFunctionTypeConverter class.
        /// </summary>
        public SimpleFunctionTypeConverter()
        {
        }

        /// <summary>
        /// Determine whether the sourceType can be converted to a single
        /// variable function.
        /// </summary>
        /// <param name="context">Conversion context.</param>
        /// <param name="sourceType">The type to convert from.</param>
        /// <returns>
        /// A value indicating whether the type can be converted.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Convert the value into a single variable function.
        /// </summary>
        /// <param name="context">Conversion context.</param>
        /// <param name="culture">Conversion culture.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>A single variable function.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Func<double, double> function = null;

            string text = value as string;
            if (!string.IsNullOrEmpty(text))
            {
                function = new Parser(text).Function;
            }

            return function;
        }

        /// <summary>
        /// Defines the type of a token.
        /// </summary>
        private enum TokenType
        {
            /// <summary>
            /// A numeric value..
            /// </summary>
            Number,

            /// <summary>
            /// An identifier.
            /// </summary>
            Identifier,

            /// <summary>
            /// The addition operator.
            /// </summary>
            Addition,

            /// <summary>
            /// The substraction operator.
            /// </summary>
            Subtraction,

            /// <summary>
            /// The exponentiation operator.
            /// </summary>
            Multiplication,

            /// <summary>
            /// The division operator.
            /// </summary>
            Division,

            /// <summary>
            /// The exponentiation operator.
            /// </summary>
            Exponentiation,

            /// <summary>
            /// A left parenthesis.
            /// </summary>
            LeftParenthesis,

            /// <summary>
            /// A right parenthesis.
            /// </summary>
            RightParenthesis
        }

        /// <summary>
        /// Represents a lexical token.
        /// </summary>
        private class Token
        {
            /// <summary>
            /// Gets or sets the type of the token.
            /// </summary>
            public TokenType TokenType { get; set; }

            /// <summary>
            /// Gets or sets the value of a token (for numbers and identifiers).
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// Initializes a new instance of the Token class.
            /// </summary>
            public Token()
            {
            }
        }

        /// <summary>
        /// Perform lexical analysis of simple expressions.
        /// </summary>
        private class Lexer
        {
            /// <summary>
            /// Gets or sets the input string to scan.
            /// </summary>
            internal string Input { get; set; }

            /// <summary>
            /// Gets or sets the current position of the lexer.
            /// </summary>
            internal int Position { get; set; }

            /// <summary>
            /// Gets a value indicating whether the lexer has room to advance
            /// through the input.
            /// </summary>
            internal bool CanAdvance
            {
                get { return Position < Input.Length; }
            }

            /// <summary>
            /// Gets the character at the current input position.
            /// </summary>
            private char Current
            {
                get { return Input[Position]; }
            }

            /// <summary>
            /// Gets or sets the lookahead token.
            /// </summary>
            private Token Lookahead { get; set; }

            /// <summary>
            /// Initializes a new instance of the Lexer class.
            /// </summary>
            /// <param name="input">The input to analyze.</param>
            public Lexer(string input)
            {
                Debug.Assert(!string.IsNullOrEmpty(input), "input shuould not be null or empty!");
                Input = input;
            }

            /// <summary>
            /// Advance the token to the next input.
            /// </summary>
            /// <returns>The token that was read.</returns>
            private Token ReadNext()
            {
                // Eat as much whitespace as possible
                while (CanAdvance && char.IsWhiteSpace(Current))
                {
                    Position++;
                }

                // Match a literal token
                Token token =
                    MatchLiteral('(', TokenType.LeftParenthesis) ??
                    MatchLiteral(')', TokenType.RightParenthesis) ??
                    MatchLiteral('+', TokenType.Addition) ??
                    MatchLiteral('-', TokenType.Subtraction) ??
                    MatchLiteral('*', TokenType.Multiplication) ??
                    MatchLiteral('/', TokenType.Division) ??
                    MatchLiteral('^', TokenType.Exponentiation);

                // Match identifier or number tokens
                if (token == null)
                {
                    int start = Position;
                    
                    // Try and match identifiers
                    while (CanAdvance && char.IsLetter(Current))
                    {
                        Position++;
                    }
                    if (start != Position)
                    {
                        token = new Token { TokenType = TokenType.Identifier };
                        token.Value = Input.Substring(start, Position - start);
                    }
                    else
                    {
                        // Try and match numbers
                        while (CanAdvance && char.IsDigit(Current))
                        {
                            Position++;
                        }
                        if (CanAdvance && Current == '.')
                        {
                            Position++;
                        }
                        while (CanAdvance && char.IsDigit(Current))
                        {
                            Position++;
                        }

                        if (start != Position)
                        {
                            token = new Token { TokenType = TokenType.Number };
                            token.Value = double.Parse(Input.Substring(start, Position - start), CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (token != null)
                {
                    return token;
                }
                else if (CanAdvance)
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Unknown token at position {0}!", Position));
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Match a literal token.
            /// </summary>
            /// <param name="tokenChar">Character of the token.</param>
            /// <param name="tokenType">The type of the token.</param>
            /// <returns>The literal token, if matched.</returns>
            private Token MatchLiteral(char tokenChar, TokenType tokenType)
            {
                if (CanAdvance && Current == tokenChar)
                {
                    Position++;
                    return new Token { TokenType = tokenType };
                }

                return null;
            }

            /// <summary>
            /// Get the next input token.
            /// </summary>
            /// <returns>The next input token.</returns>
            public Token Get()
            {
                Token token = null;
                if (Lookahead != null)
                {
                    token = Lookahead;
                    Lookahead = null;
                }
                else
                {
                    token = ReadNext();
                }
                return token;
            }

            /// <summary>
            /// Peek at the lookahead token.
            /// </summary>
            /// <returns>The lookahead token.</returns>
            public Token Peek()
            {
                if (Lookahead == null)
                {
                    Lookahead = ReadNext();
                }
                return Lookahead;
            }
        }

        /// <summary>
        /// Perform syntactic analysis of simple expressions.
        /// </summary>
        /// <remarks>
        /// The parser uses the following grammar:
        ///    {Expression}
        ///         := {Term} '+' {Expression}
        ///         |  {Term} '-' {Expression}
        ///         |  {Term}
        ///    {Term}
        ///         := {Exponent} '*' {Term}
        ///         |  {Exponent} '/' {Term}
        ///         |  {Exponent}
        ///    {Exponent}
        ///         := {Factor} '^' {Exponent}
        ///         |  {Factor}
        ///    {Factor}
        ///         := {Number}
        ///         |  {Identifier}
        ///         | '(' {Expression} ')'
        /// </remarks>
        private class Parser
        {
            /// <summary>
            /// Gets or sets the lexer used for lexical analysis.
            /// </summary>
            private Lexer Lexer { get; set; }

            /// <summary>
            /// Gets or sets the single variable of the function.
            /// </summary>
            private Sle.ParameterExpression Parameter { get; set; }

            /// <summary>
            /// Gets the function created from the input.
            /// </summary>
            public Func<double, double> Function { get; private set; }

            /// <summary>
            /// Initializes a new instance of the Parser class.
            /// </summary>
            /// <param name="input">The input to analyze.</param>
            public Parser(string input)
            {
                Lexer = new Lexer(input);
                Parse();
            }

            /// <summary>
            /// Parse the input and create a function.
            /// </summary>
            private void Parse()
            {
                // Build the expression
                Sle.Expression expression = GetExpression();

                // Ensure we exhausted the input
                int finalPosition = Lexer.Position;
                Token finalToken = Lexer.Get();
                if (finalToken != null)
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Unexpected token {0} at position {1}!", finalToken.TokenType, finalPosition));
                }

                // Wrap the expression in a function
                Sle.Expression<Func<double, double>> functionExpression =
                    Sle.Expression.Lambda<Func<double, double>>(
                        expression,
                        Parameter ?? Sle.Expression.Parameter(typeof(double), "x"));

                // Compile the expression into a function
                Function = functionExpression.Compile();
            }

            /// <summary>
            /// Read an expression.
            /// </summary>
            /// <returns>The parsed expression.</returns>
            private Sle.Expression GetExpression()
            {
                Sle.Expression term = GetTerm();
                if (TryMatch(TokenType.Addition))
                {
                    Sle.Expression expr = GetExpression();
                    return Sle.Expression.Add(term, expr);
                }
                else if (TryMatch(TokenType.Subtraction))
                {
                    Sle.Expression expr = GetExpression();
                    return Sle.Expression.Subtract(term, expr);
                }
                return term;
            }

            /// <summary>
            /// Read a term.
            /// </summary>
            /// <returns>The parsed term.</returns>
            private Sle.Expression GetTerm()
            {
                Sle.Expression exponent = GetExponent();
                if (TryMatch(TokenType.Multiplication))
                {
                    Sle.Expression term = GetTerm();
                    return Sle.Expression.Multiply(exponent, term);
                }
                else if (TryMatch(TokenType.Division))
                {
                    Sle.Expression term = GetTerm();
                    return Sle.Expression.Divide(exponent, term);
                }
                return exponent;
            }

            /// <summary>
            /// Read an exponent.
            /// </summary>
            /// <returns>The parsed exponent.</returns>
            private Sle.Expression GetExponent()
            {
                Sle.Expression factor = GetFactor();
                if (TryMatch(TokenType.Exponentiation))
                {
                    Sle.Expression power = GetExponent();
                    return Sle.Expression.Power(factor, power);
                }
                else
                {
                    return factor;
                }
            }

            /// <summary>
            /// Read a factor.
            /// </summary>
            /// <returns>The parsed factor.</returns>
            private Sle.Expression GetFactor()
            {
                Token token = Lexer.Get();
                if (token != null)
                {
                    if (token.TokenType == TokenType.Number)
                    {
                        return Sle.Expression.Constant(token.Value, typeof(double));
                    }
                    else if (token.TokenType == TokenType.Identifier)
                    {
                        string name = token.Value as string;

                        // Linq expressions use referential equality on
                        // parameters, so we need to use the same instance
                        if (Parameter != null && Parameter.Name != name)
                        {
                            throw new FormatException("Only single variable functions are supported!");
                        }
                        else if (Parameter == null)
                        {
                            Parameter = Sle.Expression.Parameter(typeof(double), name);
                        }
                        return Parameter;
                    }
                    else if (token.TokenType == TokenType.LeftParenthesis)
                    {
                        Sle.Expression nested = GetExpression();
                        if (TryMatch(TokenType.RightParenthesis))
                        {
                            return nested;
                        }
                    }
                }

                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Unexpected token type at position {0}!", Lexer.Position));
            }
            
            /// <summary>
            /// Try to match a token.
            /// </summary>
            /// <param name="tokenType">The type of token to match.</param>
            /// <returns>
            /// A value indicating whether the token was matched.
            /// </returns>
            private bool TryMatch(TokenType tokenType)
            {
                Token token = Lexer.Peek();
                if (token != null && token.TokenType == tokenType)
                {
                    Lexer.Get();
                    return true;
                }

                return false;
            }
        }
    }
}