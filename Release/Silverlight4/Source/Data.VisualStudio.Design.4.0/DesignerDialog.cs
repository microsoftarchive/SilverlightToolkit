// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Wpf = System.Windows;

namespace System.Windows.Controls.Data.Design
{
    internal partial class DesignerDialog : Form 
    {
        public static DialogResult ShowDesignerDialog(string title, Wpf.UIElement wpfContent, int width, int height)
        {
            DesignerDialog dlg = new DesignerDialog(title, wpfContent);
            dlg.Width = width;
            dlg.Height = height;
            dlg.SizeGripStyle = SizeGripStyle.Hide;
            return dlg.ShowDialog();
        }

        public static DialogResult ShowDesignerDialog(string title, Wpf.UIElement wpfContent)
        {
               return (new DesignerDialog(title, wpfContent)).ShowDialog();
        }

        public DesignerDialog(string title, Wpf.UIElement wpfContent)
            : this()
        {
            this.Text = title;
            this.elementHost1.Child = wpfContent;
        }

        public DesignerDialog() 
        {
            InitializeComponent();
        }
    }
}
