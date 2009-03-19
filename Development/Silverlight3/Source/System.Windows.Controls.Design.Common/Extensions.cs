//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Sle = System.Linq.Expressions;

namespace System.Windows.Controls.Design.Common
{
    /// <summary>
    /// This set of internal extension methods provide general solutions and 
    /// utilities in a small enough number to not warrant a dedicated extension
    /// methods class.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Helper method to get member name with compile time verification to avoid typo.
        /// </summary>
        /// <typeparam name="T">The containing class of the member whose name is retrieved.</typeparam>
        /// <param name="expr">The lambda expression usually in the form of o => o.member.</param>
        /// <returns>The name of the property.</returns>
        public static string GetMemberName<T>(Sle.Expression<Func<T, object>> expr)
        {
            Sle.Expression body = ((Sle.LambdaExpression)expr).Body;
            Sle.MemberExpression memberExpression = body as Sle.MemberExpression;
            if (memberExpression == null)
            {
                memberExpression = (Sle.MemberExpression)((Sle.UnaryExpression)body).Operand;
            }
            return memberExpression.Member.Name;
        }
    }
}
