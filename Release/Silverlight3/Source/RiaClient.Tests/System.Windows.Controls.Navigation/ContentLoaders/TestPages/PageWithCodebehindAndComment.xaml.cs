//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Controls;

namespace System.Windows.Navigation.UnitTests
{
    public partial class PageWithCodebehindAndComment : Page
    {
        static int _instances;

        public PageWithCodebehindAndComment()
        {
            InitializeComponent();
            _instances++;
            _txtBlock.Text += string.Format(CultureInfo.InvariantCulture, ", Instance #{0}", _instances);
        }
    }
}
