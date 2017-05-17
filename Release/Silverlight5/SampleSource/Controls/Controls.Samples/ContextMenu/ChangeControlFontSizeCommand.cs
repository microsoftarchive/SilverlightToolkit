// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// ICommand implementation for changing the font size of a control.
    /// </summary>
    public class ChangeControlFontSizeCommand : ICommand
    {
        /// <summary>
        /// Minimum valid font size.
        /// </summary>
        private const int MinimumFontSize = 12;

        /// <summary>
        /// Maximum valid font size.
        /// </summary>
        private const int MaximumFontSize = 16;

        /// <summary>
        /// References the target control.
        /// </summary>
        private Control _control;

        /// <summary>
        /// Initializes a new instance of the ChangeControlFontSizeCommand class.
        /// </summary>
        /// <param name="control">Target control.</param>
        public ChangeControlFontSizeCommand(Control control)
        {
            _control = control;
        }

        /// <summary>
        /// Returns a value indicating whether the command can execute.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        /// <returns>True if the command can execute.</returns>
        public bool CanExecute(object parameter)
        {
            int deltaFontSize = (int)parameter;
            double newFontSize = _control.FontSize + deltaFontSize;
            return (MinimumFontSize <= newFontSize) && (newFontSize <= MaximumFontSize);
        }

        /// <summary>
        /// Executes the command to change the font size.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public void Execute(object parameter)
        {
            int deltaFontSize = (int)parameter;
            _control.FontSize += deltaFontSize;
            EventHandler handler = CanExecuteChanged;
            if (null != handler)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Event that is fired when the CanExecute state has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}
