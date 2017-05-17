// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ex = System.Linq.Expressions.Expression;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    /// <summary>
    /// A class containing an Assert method.
    /// </summary>
    public static class Test
    {
        /// <summary>
        /// The MethodInfo object used to invoke Assert.IsNotNull.
        /// </summary>
        private static readonly MethodInfo IsNotNull = typeof(Assert).GetMethod("IsNotNull", new Type[] { typeof(object), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.AreNotSame.
        /// </summary>        
        private static readonly MethodInfo AreNotSame = typeof(Assert).GetMethod("AreNotSame", new Type[] { typeof(object), typeof(object), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.AreNotEqual.
        /// </summary>        
        private static readonly MethodInfo AreNotEqual = typeof(Assert).GetMethod("AreNotEqual", new Type[] { typeof(object), typeof(object), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.AreSame.
        /// </summary>
        private static readonly MethodInfo AreSame = typeof(Assert).GetMethod("AreSame", new Type[] { typeof(object), typeof(object), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.AreEqual.
        /// </summary>
        private static readonly MethodInfo AreEqual = typeof(Assert).GetMethod("AreEqual", new Type[] { typeof(object), typeof(object), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.IsNull.
        /// </summary>
        private static readonly MethodInfo IsNull = typeof(Assert).GetMethod("IsNull", new Type[] { typeof(object), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.IsFalse.
        /// </summary>
        private static readonly MethodInfo IsFalse = typeof(Assert).GetMethod("IsFalse", new Type[] { typeof(bool), typeof(string) });

        /// <summary>
        /// The MethodInfo object used to invoke Assert.IsTrue.
        /// </summary>
        private static readonly MethodInfo IsTrue = typeof(Assert).GetMethod("IsTrue", new Type[] { typeof(bool), typeof(string) });

        /// <summary>
        /// A regular expression which identifies a pattern signifying an 
        /// anonymous type.  This pattern is emitted by the Expression object 
        /// when it converts an anonymous type to a string.
        /// </summary>
        private static readonly Regex FindAnonymousTypeExpressionInString = new Regex(@"value\([^\+\)]+[^\)]+\).", RegexOptions.None);

        /// <summary>
        /// A regular expression which identifies a pattern emitted by an
        /// Expression object when it converts an anonymous type to a
        /// string.
        /// </summary>
        private static readonly Regex FindTransparentIdentifier = new Regex(@"\<\>h__TransparentIdentifier[^\.]+\.", RegexOptions.None);

        /// <summary>
        /// Asserts a condition is true.
        /// </summary>
        /// <param name="predicate">The condition that must be true.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Signature is required by C# compiler to enable expressions.")]
        public static void Assert(System.Linq.Expressions.Expression<Func<bool>> predicate)
        {
            AssertExpression<object>(predicate, null);
        }

        /// <summary>
        /// Asserts a condition is true.
        /// </summary>
        /// <param name="predicate">The condition that must be true.</param>
        /// <param name="message">The message to be displayed when the test fails.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Signature is required by C# compiler to enable expressions.")]
        public static void Assert(System.Linq.Expressions.Expression<Func<bool>> predicate, string message)
        {
            AssertExpression<object>(predicate, message);
        }

        /// <summary>
        /// Generates an assert failure message using the code block
        /// responsible for the failure.
        /// </summary>
        /// <param name="expression">The predicate that fails.</param>
        /// <param name="message">The user-provided assertion failure message.
        /// </param>
        /// <returns>A constant expression containing the user-provided failure
        /// message or a description of the expression that failed.</returns>
        private static string GetMessageExpression(System.Linq.Expressions.Expression expression, string message)
        {
            if (message != null)
            {
                return message;
            }
            else
            {
                string readableMessage = expression.ToString();

                readableMessage = FindAnonymousTypeExpressionInString.Replace(readableMessage, "");
                readableMessage = FindTransparentIdentifier.Replace(readableMessage, "");
                return readableMessage;
            }
        }

        /// <summary>
        /// Analyzes the expression and creates an action which calls the 
        /// correct assert method based on its contents.
        /// </summary>
        /// <param name="expression">The predicate expression.</param>
        /// <param name="message">The message to display if an assertion fails.</param>
        /// <returns>An action which calls the correct assert method.</returns>
        private static Action CreateAssertExpressionAction(System.Linq.Expressions.Expression expression, string message)
        {
            Action assertAction = null;

            // If expression is a method call that results in a boolean convert
            // it into a comparison to true.
            if (expression is MethodCallExpression && expression.Type == typeof(bool))
            {
                expression = Ex.Equal(expression, Ex.Constant(true));
            }

            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                assertAction = CreateBinaryExpressionAction(binaryExpression, message);
            }
            {
                TypeBinaryExpression typeBinaryExpression = expression as TypeBinaryExpression;
                if (typeBinaryExpression != null)
                {
                    // Test to see if expression is a type
                    // comparison.  If so, call 
                    // Assert.IsInstanceOfType.
                    // Action constructed differently than
                    // others because embedding a type as a
                    // constant in an expression causes a crash.
                    assertAction =
                        () =>
                        {
                            object obj =
                                Ex.Lambda<Func<object>>(
                                    typeBinaryExpression.Expression).Compile()();

                            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(obj, typeBinaryExpression.TypeOperand, GetMessageExpression(typeBinaryExpression, message));
                        };
                }
            }
            UnaryExpression unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Not)
            {
                TypeBinaryExpression typeBinaryExpression = unaryExpression.Operand as TypeBinaryExpression;
                if (typeBinaryExpression != null)
                {
                    // Test to see if ! is around a type
                    // comparison.  If so, call 
                    // Assert.IsNotInstanceOfType.
                    // Action constructed differently than
                    // others because embedding a type as a
                    // constant in an expression causes a crash.
                    assertAction =
                        () =>
                        {
                            object obj =
                                Ex.Lambda<Func<object>>(
                                    typeBinaryExpression.Expression).Compile()();

                            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotInstanceOfType(obj, typeBinaryExpression.TypeOperand, GetMessageExpression(typeBinaryExpression, message));
                        };
                }
                else
                {
                    // Convert a ! around the whole expression
                    // to a call to Assert.IsFalse.
                    assertAction =
                        Ex.Lambda<Action>(
                            Ex.Call(IsFalse, unaryExpression.Operand, Ex.Constant(GetMessageExpression(unaryExpression, message)))).Compile();
                }
            }

            return assertAction;
        }

        /// <summary>
        /// Analyzes a predicate expression and invokes the correct assert 
        /// method.
        /// </summary>
        /// <typeparam name="T">The type of the argument to the predicate.
        /// </typeparam>
        /// <param name="predicate">A predicate to be converted into an 
        /// assert statement.</param>
        /// <param name="message">The assertion message.</param> 
        private static void AssertExpression<T>(Expression<Func<bool>> predicate, string message)
        {
            System.Linq.Expressions.Expression body = predicate.Body;

            Action assertAction = CreateAssertExpressionAction(body, message);

            if (assertAction != null)
            {
                assertAction();
            }
            else
            {
                // If no special cases have been found just call
                // IsTrue on the predicate.
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(predicate.Compile()(), GetMessageExpression(body, message));
            }
        }

        /// <summary>
        /// Creates an action from certain types of supported binary expression
        /// patterns.  If no supported pattern is found null is returned.  Based
        /// on analyzing the predicate the action invokes the correct Assert
        /// method.
        /// </summary>
        /// <param name="binaryExpression">A binary expression extracted from a
        /// predicate expression.</param>
        /// <param name="message">The message to display if the assertion 
        /// fails.</param>
        /// <returns>An action that invokes an Assert method.</returns>
        private static Action CreateBinaryExpressionAction(BinaryExpression binaryExpression, string message)
        {
            ConstantExpression messageExpression = Ex.Constant(GetMessageExpression(binaryExpression, message));
            Action assertAction = null;
            if (binaryExpression != null)
            {
                // If operands are not equal...
                if (binaryExpression.NodeType == ExpressionType.NotEqual)
                {
                    ConstantExpression left = binaryExpression.Left as ConstantExpression;
                    ConstantExpression right = binaryExpression.Right as ConstantExpression;

                    // Test to see if either side is null and if
                    // so, call Assert.IsNotNull()
                    if (left != null && left.Value == null)
                    {
                        assertAction =
                            Ex.Lambda<Action>(
                                Ex.Call(IsNotNull, binaryExpression.Right, messageExpression)).Compile();
                    }
                    else if (right != null && right.Value == null)
                    {
                        assertAction =
                            Ex.Lambda<Action>(
                                Ex.Call(IsNotNull, binaryExpression.Left, messageExpression)).Compile();
                    }
                    else
                    {
                        if (binaryExpression.Left.Type.IsValueType && binaryExpression.Right.Type.IsValueType)
                        {
                            // Test to see if the operands are
                            // value types.  If so, call 
                            // Assert.AreNotSame()
                            assertAction =
                                Ex.Lambda<Action>(
                                    Ex.Call(AreNotEqual, Ex.Convert(binaryExpression.Left, typeof(object)), Ex.Convert(binaryExpression.Right, typeof(object)), messageExpression)).Compile();
                        }
                        else
                        {
                            // Otherwise call 
                            // Assert.AreNotEqual()
                            assertAction =
                                Ex.Lambda<Action>(
                                    Ex.Call(AreNotSame, binaryExpression.Left, binaryExpression.Right, messageExpression)).Compile();
                        }
                    }
                }
                else if (binaryExpression.NodeType == ExpressionType.Equal)
                {
                    // if operands are equal...
                    {
                        ConstantExpression left = binaryExpression.Left as ConstantExpression;
                        ConstantExpression right = binaryExpression.Right as ConstantExpression;

                        if (left != null)
                        {
                            if (left.Value == null)
                            {
                                // Test to see if left side is 
                                // null and if so, call 
                                // Assert.IsNull()
                                assertAction =
                                    Ex.Lambda<Action>(
                                        Ex.Call(IsNull, binaryExpression.Right, messageExpression)).Compile();
                            }
                            else if (left.Value is bool)
                            {
                                // Test to see if left side 
                                // is a boolean and if so, call 
                                // Assert.IsFalse or IsTrue
                                bool value = (bool)left.Value;
                                if (value == true)
                                {
                                    assertAction =
                                        Ex.Lambda<Action>(
                                            Ex.Call(IsTrue, binaryExpression.Right, messageExpression)).Compile();
                                }
                                else
                                {
                                    assertAction =
                                        Ex.Lambda<Action>(
                                            Ex.Call(IsFalse, binaryExpression.Right, messageExpression)).Compile();
                                }
                            }
                        }
                        else if (right != null)
                        {
                            if (right.Value == null)
                            {
                                // Test to see if right side is 
                                // null and if so, call 
                                // Assert.IsNull()
                                assertAction =
                                    Ex.Lambda<Action>(
                                        Ex.Call(IsNull, binaryExpression.Left, messageExpression)).Compile();
                            }
                            else if (right.Value is bool)
                            {
                                // Test to see if right side 
                                // is a boolean and if so, call 
                                // Assert.IsFalse or IsTrue
                                bool value = (bool)right.Value;
                                if (value == true)
                                {
                                    assertAction =
                                        Ex.Lambda<Action>(
                                            Ex.Call(IsTrue, binaryExpression.Left, messageExpression)).Compile();
                                }
                                else
                                {
                                    assertAction =
                                        Ex.Lambda<Action>(
                                            Ex.Call(IsFalse, binaryExpression.Left, messageExpression)).Compile();
                                }
                            }
                        }
                    }
                    if (assertAction == null)
                    {
                        if (binaryExpression.Left.Type.IsValueType && binaryExpression.Right.Type.IsValueType)
                        {
                            // Test to see if the operands are
                            // value types.  If so, call 
                            // Assert.AreSame()
                            assertAction =
                                Ex.Lambda<Action>(
                                    Ex.Call(AreEqual, Ex.Convert(binaryExpression.Left, typeof(object)), Ex.Convert(binaryExpression.Right, typeof(object)), messageExpression)).Compile();
                        }
                        else
                        {
                            // Otherwise call Assert.AreSame()
                            assertAction =
                                Ex.Lambda<Action>(
                                    Ex.Call(AreSame, binaryExpression.Left, binaryExpression.Right, messageExpression)).Compile();
                        }
                    }
                }
                else if (binaryExpression.NodeType == ExpressionType.And || binaryExpression.NodeType == ExpressionType.AndAlso)
                {
                    // Assert both sides of the expression individually.
                    Action firstAction = CreateAssertExpressionAction(
                        binaryExpression.Left,
                        message);

                    Action secondAction = CreateAssertExpressionAction(
                        binaryExpression.Right,
                        message);

                    return () =>
                    {
                        firstAction();
                        secondAction();
                    };
                }
            }

            return assertAction;
        }
    }
}