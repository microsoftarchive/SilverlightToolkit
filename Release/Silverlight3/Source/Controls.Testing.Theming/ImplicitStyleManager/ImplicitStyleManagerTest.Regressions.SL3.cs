// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Regression tests for the System.Windows.Controls.ImplicitStyleManager
    /// class.
    /// </summary>
    public partial class ImplicitStyleManagerTest
    {
        /// <summary>
        /// Sets the application resource dictionary.
        /// </summary>
        /// <param name="uri">The uri of the resource dictionary.</param>
        private static void SetApplicationResourceDictionaryUri(Uri uri)
        {
            if (uri == null)
            {
                Application.Current.Resources.MergedDictionaries.Clear();
            }
            else
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary { Source = uri };
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}