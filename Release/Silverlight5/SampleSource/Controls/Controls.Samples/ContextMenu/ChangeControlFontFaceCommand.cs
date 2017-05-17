// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// ICommand implementation for changing the font face of a control.
    /// </summary>
    public class ChangeControlFontFaceCommand : ICommand
    {
        /// <summary>
        /// References the target control.
        /// </summary>
        private Control _control;

        /// <summary>
        /// Initializes a new instance of the ChangeControlFontFaceCommand class.
        /// </summary>
        /// <param name="control">Target control.</param>
        public ChangeControlFontFaceCommand(Control control)
        {
            _control = control;
            EventHandler suppressUnusedEventWarning = CanExecuteChanged;
        }

        /// <summary>
        /// Returns a value indicating whether the command can execute.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        /// <returns>True if the command can execute.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command to change the font family.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public void Execute(object parameter)
        {
            _control.FontFamily = new FontFamily((string)parameter);
        }

        /// <summary>
        /// Event that is fired when the CanExecute state has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}
