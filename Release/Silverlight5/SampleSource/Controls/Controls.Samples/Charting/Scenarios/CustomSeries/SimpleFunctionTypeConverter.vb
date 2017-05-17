' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports Sle = System.Linq.Expressions
Imports System.Text

''' <summary>
''' TypeConverter used for creating single variable functions from simple
''' arithmetic expressions.
''' </summary>
Public Class SimpleFunctionTypeConverter
    Inherits TypeConverter
    ''' <summary>
    ''' Initializes a new instance of the SimpleFunctionTypeConverter class.
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Determine whether the sourceType can be converted to a single
    ''' variable function.
    ''' </summary>
    ''' <param name="context">Conversion context.</param>
    ''' <param name="sourceType">The type to convert from.</param>
    ''' <returns>
    ''' A value indicating whether the type can be converted.
    ''' </returns>
    Public Overloads Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
        Return sourceType Is GetType(String)
    End Function

    ''' <summary>
    ''' Convert the value into a single variable function.
    ''' </summary>
    ''' <param name="context">Conversion context.</param>
    ''' <param name="culture">Conversion culture.</param>
    ''' <param name="value">The value to convert.</param>
    ''' <returns>A single variable function.</returns>
    Public Overloads Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
        Dim [function] As Func(Of Double, Double) = Nothing

        Dim text As String = TryCast(value, String)
        If (Not String.IsNullOrEmpty(text)) Then
            [function] = New Parser(text).Function
        End If

        Return [function]
    End Function

    ''' <summary>
    ''' Defines the type of a token.
    ''' </summary>
    Private Enum TokenType
        ''' <summary>
        ''' A numeric value..
        ''' </summary>
        Number

        ''' <summary>
        ''' An identifier.
        ''' </summary>
        Identifier

        ''' <summary>
        ''' The addition operator.
        ''' </summary>
        Addition

        ''' <summary>
        ''' The substraction operator.
        ''' </summary>
        Subtraction

        ''' <summary>
        ''' The exponentiation operator.
        ''' </summary>
        Multiplication

        ''' <summary>
        ''' The division operator.
        ''' </summary>
        Division

        ''' <summary>
        ''' The exponentiation operator.
        ''' </summary>
        Exponentiation

        ''' <summary>
        ''' A left parenthesis.
        ''' </summary>
        LeftParenthesis

        ''' <summary>
        ''' A right parenthesis.
        ''' </summary>
        RightParenthesis
    End Enum

    ''' <summary>
    ''' Represents a lexical token.
    ''' </summary>
    Private Class Token
        ''' <summary>
        ''' Gets or sets the type of the token.
        ''' </summary>
        Private privateTokenType As TokenType
        Public Property TokenType() As TokenType
            Get
                Return privateTokenType
            End Get
            Set(ByVal value As TokenType)
                privateTokenType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of a token (for numbers and identifiers).
        ''' </summary>
        Private privateValue As Object
        Public Property Value() As Object
            Get
                Return privateValue
            End Get
            Set(ByVal value As Object)
                privateValue = value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance of the Token class.
        ''' </summary>
        Public Sub New()
        End Sub
    End Class

    ''' <summary>
    ''' Perform lexical analysis of simple expressions.
    ''' </summary>
    Private Class Lexer
        ''' <summary>
        ''' Gets or sets the input string to scan.
        ''' </summary>
        Private privateInput As String
        Friend Property Input() As String
            Get
                Return privateInput
            End Get
            Set(ByVal value As String)
                privateInput = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the current position of the lexer.
        ''' </summary>
        Private privatePosition As Integer
        Friend Property Position() As Integer
            Get
                Return privatePosition
            End Get
            Set(ByVal value As Integer)
                privatePosition = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the lexer has room to advance
        ''' through the input.
        ''' </summary>
        Friend ReadOnly Property CanAdvance() As Boolean
            Get
                Return Position < Input.Length
            End Get
        End Property

        ''' <summary>
        ''' Gets the character at the current input position.
        ''' </summary>
        Private ReadOnly Property Current() As Char
            Get
                Return Input(Position)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the lookahead token.
        ''' </summary>
        Private privateLookahead As Token
        Private Property Lookahead() As Token
            Get
                Return privateLookahead
            End Get
            Set(ByVal value As Token)
                privateLookahead = value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance of the Lexer class.
        ''' </summary>
        ''' <param name="input">The input to analyze.</param>
        Public Sub New(ByVal input As String)
            Debug.Assert((Not String.IsNullOrEmpty(input)), "input shuould not be null or empty!")
            Me.Input = input
        End Sub

        ''' <summary>
        ''' Advance the token to the next input.
        ''' </summary>
        ''' <returns>The token that was read.</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification:="Literal token matching code is consistent with C# implementation.")> _
        Private Function ReadNext() As Token
            ' Eat as much whitespace as possible
            Do While CanAdvance AndAlso Char.IsWhiteSpace(Current)
                Position += 1
            Loop

            ' Match a literal token
            Dim token As Token = MatchLiteral("("c, TokenType.LeftParenthesis)
            If token Is Nothing Then
                token = MatchLiteral(")"c, TokenType.RightParenthesis)
                If token Is Nothing Then
                    token = MatchLiteral("+"c, TokenType.Addition)
                    If token Is Nothing Then
                        token = MatchLiteral("-"c, TokenType.Subtraction)
                        If token Is Nothing Then
                            token = MatchLiteral("*"c, TokenType.Multiplication)
                            If token Is Nothing Then
                                token = MatchLiteral("/"c, TokenType.Division)
                                If token Is Nothing Then
                                    token = MatchLiteral("^"c, TokenType.Exponentiation)
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            ' Match identifier or number tokens
            If token Is Nothing Then
                Dim start As Integer = Position

                ' Try and match identifiers
                Do While CanAdvance AndAlso Char.IsLetter(Current)
                    Position += 1
                Loop
                If start <> Position Then
                    token = New Token With {.TokenType = TokenType.Identifier}
                    token.Value = Input.Substring(start, Position - start)
                Else
                    ' Try and match numbers
                    Do While CanAdvance AndAlso Char.IsDigit(Current)
                        Position += 1
                    Loop
                    If CanAdvance AndAlso Current = "."c Then
                        Position += 1
                    End If
                    Do While CanAdvance AndAlso Char.IsDigit(Current)
                        Position += 1
                    Loop

                    If start <> Position Then
                        token = New Token With {.TokenType = TokenType.Number}
                        token.Value = Double.Parse(Input.Substring(start, Position - start), CultureInfo.InvariantCulture)
                    End If
                End If
            End If

            If token IsNot Nothing Then
                Return token
            ElseIf CanAdvance Then
                Throw New FormatException(String.Format(CultureInfo.InvariantCulture, "Unknown token at position {0}!", Position))
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Match a literal token.
        ''' </summary>
        ''' <param name="tokenChar">Character of the token.</param>
        ''' <param name="tokenType">The type of the token.</param>
        ''' <returns>The literal token, if matched.</returns>
        Private Function MatchLiteral(ByVal tokenChar As Char, ByVal tokenType As TokenType) As Token
            If CanAdvance AndAlso Current = tokenChar Then
                Position += 1
                Return New Token With {.TokenType = tokenType}
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Get the next input token.
        ''' </summary>
        ''' <returns>The next input token.</returns>
        Public Function [Get]() As Token
            Dim token As Token = Nothing
            If Lookahead IsNot Nothing Then
                token = Lookahead
                Lookahead = Nothing
            Else
                token = ReadNext()
            End If
            Return token
        End Function

        ''' <summary>
        ''' Peek at the lookahead token.
        ''' </summary>
        ''' <returns>The lookahead token.</returns>
        Public Function Peek() As Token
            If Lookahead Is Nothing Then
                Lookahead = ReadNext()
            End If
            Return Lookahead
        End Function
    End Class

    ''' <summary>
    ''' Perform syntactic analysis of simple expressions.
    ''' </summary>
    ''' <remarks>
    ''' The parser uses the following grammar:
    '''    {Expression}
    '''         := {Term} '+' {Expression}
    '''         |  {Term} '-' {Expression}
    '''         |  {Term}
    '''    {Term}
    '''         := {Exponent} '*' {Term}
    '''         |  {Exponent} '/' {Term}
    '''         |  {Exponent}
    '''    {Exponent}
    '''         := {Factor} '^' {Exponent}
    '''         |  {Factor}
    '''    {Factor}
    '''         := {Number}
    '''         |  {Identifier}
    '''         | '(' {Expression} ')'
    ''' </remarks>
    Private Class Parser
        ''' <summary>
        ''' Gets or sets the lexer used for lexical analysis.
        ''' </summary>
        Private privateLexer As Lexer
        Private Property Lexer() As Lexer
            Get
                Return privateLexer
            End Get
            Set(ByVal value As Lexer)
                privateLexer = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the single variable of the function.
        ''' </summary>
        Private privateParameter As Sle.ParameterExpression
        Private Property Parameter() As Sle.ParameterExpression
            Get
                Return privateParameter
            End Get
            Set(ByVal value As Sle.ParameterExpression)
                privateParameter = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the function created from the input.
        ''' </summary>
        Private privateFunction As Func(Of Double, Double)
        Public Property [Function]() As Func(Of Double, Double)
            Get
                Return privateFunction
            End Get
            Private Set(ByVal value As Func(Of Double, Double))
                privateFunction = value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance of the Parser class.
        ''' </summary>
        ''' <param name="input">The input to analyze.</param>
        Public Sub New(ByVal input As String)
            Lexer = New Lexer(input)
            Parse()
        End Sub

        ''' <summary>
        ''' Parse the input and create a function.
        ''' </summary>
        Private Sub Parse()
            ' Continue only if there is something to parse
            If Not Lexer.CanAdvance Then
                Return
            End If

            ' Build the expression
            Dim expression As Sle.Expression = GetExpression()

            ' Ensure we exhausted the input
            Dim finalPosition As Integer = Lexer.Position
            Dim finalToken As Token = Lexer.Get()
            If finalToken IsNot Nothing Then
                Throw New FormatException(String.Format(CultureInfo.InvariantCulture, "Unexpected token {0} at position {1}!", finalToken.TokenType, finalPosition))
            End If

            ' Wrap the expression in a function
            Dim functionExpression As Sle.Expression(Of Func(Of Double, Double)) = _
                Sle.Expression.Lambda(Of Func(Of Double, Double))( _
                    expression, _
                    If(Parameter IsNot Nothing, Parameter, Sle.Expression.Parameter(GetType(Double), "x")))

            ' Compile the expression into a function
            [Function] = functionExpression.Compile()
        End Sub

        ''' <summary>
        ''' Read an expression.
        ''' </summary>
        ''' <returns>The parsed expression.</returns>
        Private Function GetExpression() As Sle.Expression
            Dim term As Sle.Expression = GetTerm()
            If Lexer.CanAdvance AndAlso TryMatch(TokenType.Addition) Then
                Dim expr As Sle.Expression = GetExpression()
                Return Sle.Expression.Add(term, expr)
            ElseIf Lexer.CanAdvance AndAlso TryMatch(TokenType.Subtraction) Then
                Dim expr As Sle.Expression = GetExpression()
                Return Sle.Expression.Subtract(term, expr)
            End If
            Return term
        End Function

        ''' <summary>
        ''' Read a term.
        ''' </summary>
        ''' <returns>The parsed term.</returns>
        Private Function GetTerm() As Sle.Expression
            Dim exponent As Sle.Expression = GetExponent()
            If Lexer.CanAdvance AndAlso TryMatch(TokenType.Multiplication) Then
                Dim term As Sle.Expression = GetTerm()
                Return Sle.Expression.Multiply(exponent, term)
            ElseIf Lexer.CanAdvance AndAlso TryMatch(TokenType.Division) Then
                Dim term As Sle.Expression = GetTerm()
                Return Sle.Expression.Divide(exponent, term)
            End If
            Return exponent
        End Function

        ''' <summary>
        ''' Read an exponent.
        ''' </summary>
        ''' <returns>The parsed exponent.</returns>
        Private Function GetExponent() As Sle.Expression
            Dim factor As Sle.Expression = GetFactor()
            If Lexer.CanAdvance AndAlso TryMatch(TokenType.Exponentiation) Then
                Dim power As Sle.Expression = GetExponent()
                Return Sle.Expression.Power(factor, power)
            Else
                Return factor
            End If
        End Function

        ''' <summary>
        ''' Read a factor.
        ''' </summary>
        ''' <returns>The parsed factor.</returns>
        Private Function GetFactor() As Sle.Expression
            Dim token As Token = Lexer.Get()
            If token IsNot Nothing Then
                If token.TokenType = TokenType.Number Then
                    Return Sle.Expression.Constant(token.Value, GetType(Double))
                ElseIf token.TokenType = TokenType.Identifier Then
                    Dim name As String = TryCast(token.Value, String)

                    ' Linq expressions use referential equality on
                    ' parameters, so we need to use the same instance
                    If Parameter IsNot Nothing AndAlso Parameter.Name <> name Then
                        Throw New FormatException("Only single variable functions are supported!")
                    ElseIf Parameter Is Nothing Then
                        Parameter = Sle.Expression.Parameter(GetType(Double), name)
                    End If
                    Return Parameter
                ElseIf Lexer.CanAdvance AndAlso token.TokenType = TokenType.LeftParenthesis Then
                    Dim nested As Sle.Expression = GetExpression()
                    If TryMatch(TokenType.RightParenthesis) Then
                        Return nested
                    End If
                End If
            End If

            Throw New FormatException(String.Format(CultureInfo.InvariantCulture, "Unexpected token type at position {0}!", Lexer.Position))
        End Function

        ''' <summary>
        ''' Try to match a token.
        ''' </summary>
        ''' <param name="tokenType">The type of token to match.</param>
        ''' <returns>
        ''' A value indicating whether the token was matched.
        ''' </returns>
        Private Function TryMatch(ByVal tokenType As TokenType) As Boolean
            Dim token As Token = Lexer.Peek()
            If token IsNot Nothing AndAlso token.TokenType = tokenType Then
                Lexer.Get()
                Return True
            End If

            Return False
        End Function
    End Class
End Class
