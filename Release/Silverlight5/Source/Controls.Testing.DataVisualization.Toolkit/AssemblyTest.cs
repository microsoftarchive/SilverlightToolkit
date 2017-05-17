// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Class used for testing the DataVisualization assembly.
    /// </summary>
    [TestClass]
    public class AssemblyTest : TestBase
    {
        /// <summary>
        /// Makes sure that no control in the DataVisualization assembly is a tab stop.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Makes sure that no control in the DataVisualization assembly is a tab stop.")]
        [Bug("535547: Verify all Charting Controls have IsTabStop=False in their default Style Setters", Fixed = true)]
        public void NoControlShouldBeATabStop()
        {
            Assembly assembly = typeof(Chart).Assembly;
            IEnumerable controls = assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Control)) && t.IsPublic && !t.IsAbstract)
                .Select(t => t.GetConstructor(new Type[0]).Invoke(new object[0]))
                .ToArray();
            Panel panel = new Grid();
            foreach (Control control in controls)
            {
                Axis controlAsAxis = control as Axis;
                if (null != controlAsAxis)
                {
                    // Axis subclasses need an orientation
                    controlAsAxis.Orientation = AxisOrientation.X;
                }
                panel.Children.Add(control);
            }
            TestAsync(
                panel,
                () =>
                {
                    foreach (Control control in controls)
                    {
                        control.ApplyTemplate();
                        Assert.IsFalse(control.IsTabStop);
                    }
                });
        }
    }
}