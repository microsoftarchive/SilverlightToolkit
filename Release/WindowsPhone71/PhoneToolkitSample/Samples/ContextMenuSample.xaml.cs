// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class ContextMenuSample : PhoneApplicationPage
    {
        ViewModel _viewModel;

        public ContextMenuSample()
        {
            InitializeComponent();
            _viewModel = new ViewModel();
            _viewModel.Notify += OnViewModelNotify;
            LayoutRoot.DataContext = _viewModel;
        }

        void OnViewModelNotify(object sender, CommandEventArgs e)
        {
            lastSelection.Text = string.Format("ICommand: {0}", e.Message);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            lastSelection.Text = (string)((MenuItem)sender).Header;
        }
    }

    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class ViewModel
    {
        public ICommand AlwaysCommand { get; private set; }
        public ICommand IntermittentCommand { get; private set; }

        public event EventHandler<CommandEventArgs> Notify;

        public ViewModel()
        {
            AlwaysCommand = new AlwaysICommand();
            ((AlwaysICommand)AlwaysCommand).Notify += OnNotify;
            IntermittentCommand = new IntermittentICommand();
            ((IntermittentICommand)IntermittentCommand).Notify += OnNotify;
        }

        private void OnNotify(object sender, CommandEventArgs e)
        {
            var notify = Notify;
            if (notify != null)
            {
                notify(this, e);
            }
        }
    }

    public class AlwaysICommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            var unused = CanExecuteChanged;
            var notify = Notify;
            if (notify != null)
            {
                notify(this, new CommandEventArgs("AlwaysICommand - " + (parameter ?? "[null]")));
            }
        }
        public event EventHandler<CommandEventArgs> Notify;
    }

    public class IntermittentICommand : ICommand
    {
        private bool _canExecute;
        public IntermittentICommand()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += delegate
            {
                _canExecute = !_canExecute;
                var handler = CanExecuteChanged;
                if (null != handler)
                {
                    handler(this, EventArgs.Empty);
                }
            };
            timer.Start();
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            Debug.Assert(_canExecute);
            var notify = Notify;
            if (notify != null){
                notify(this, new CommandEventArgs("IntermittentICommand - " + (parameter ?? "[null]")));
            }
        }
        public event EventHandler<CommandEventArgs> Notify;
    }

    public class StringICommand : ICommand
    {
        private string _string;
        public StringICommand(string s)
        {
            _string = s;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            var unused = CanExecuteChanged;
            var notify = Notify;
            if (notify != null)
            {
                notify(this, new CommandEventArgs("StringICommand - " + (parameter ?? "[null]")));
            }
        }
        public event EventHandler<CommandEventArgs> Notify;
    }
}