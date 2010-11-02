// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneToolkitSample
{
    public static class Extensions
    {
        /// <summary>
        /// Return a random item from a list.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="rnd">The Random instance.</param>
        /// <param name="list">The list to choose from.</param>
        /// <returns>A randomly selected item from the list.</returns>
        public static T Next<T>(this Random rnd, IList<T> list)
        {
            return list[rnd.Next(list.Count)];
        }
    }

    /// <summary>
    /// A class used to expose the Key property on a dynamically-created Linq grouping.
    /// The grouping will be generated as an internal class, so the Key property will not
    /// otherwise be available to databind.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the items.</typeparam>
    public class PublicGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IGrouping<TKey, TElement> _internalGrouping;

        public PublicGrouping(IGrouping<TKey, TElement> internalGrouping)
        {
            _internalGrouping = internalGrouping;
        }

        public override bool Equals(object obj)
        {
            PublicGrouping<TKey, TElement> that = obj as PublicGrouping<TKey, TElement>;

            return (that != null) && (this.Key.Equals(that.Key));
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #region IGrouping<TKey,TElement> Members

        public TKey Key
        {
            get { return _internalGrouping.Key; }
        }

        #endregion

        #region IEnumerable<TElement> Members

        public IEnumerator<TElement> GetEnumerator()
        {
            return _internalGrouping.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalGrouping.GetEnumerator();
        }

        #endregion
    }

    public class CommandButton : Button
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandButton), new PropertyMetadata(OnCommandChanged));

        private static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((CommandButton)obj).OnCommandChanged(e);
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandButton), new PropertyMetadata(OnCommandParameterChanged));

        private static void OnCommandParameterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((CommandButton)obj).UpdateIsEnabled();
        }

        private void OnCommandChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ICommand command = e.OldValue as ICommand;
                if (command != null)
                {
                    command.CanExecuteChanged -= CommandCanExecuteChanged;
                }
            }

            if (e.NewValue != null)
            {
                ICommand command = e.NewValue as ICommand;
                if (command != null)
                {
                    command.CanExecuteChanged += CommandCanExecuteChanged;
                }
            }

            UpdateIsEnabled();
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateIsEnabled();
        }

        private void UpdateIsEnabled()
        {
            IsEnabled = Command != null ? Command.CanExecute(CommandParameter) : false;
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
