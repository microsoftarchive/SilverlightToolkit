' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Threading
Namespace Samples

    Partial Public Class ContextMenuSample
        Inherits PhoneApplicationPage
        Private _viewModel As ViewModel

        Public Sub New()
            InitializeComponent()
            _viewModel = New ViewModel
            AddHandler _viewModel.Notify, AddressOf viewModel_Notify
            LayoutRoot.DataContext = _viewModel
        End Sub

        Private Sub viewModel_Notify(ByVal sender As Object, ByVal e As CommandEventArgs)
            lastSelection.Text = String.Format("ICommand: {0}", e.Message)
        End Sub

        Private Sub MenuItem_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            lastSelection.Text = CStr((CType(sender, MenuItem)).Header)
        End Sub
    End Class


    Public Class CommandEventArgs
        Inherits EventArgs
        Public Sub New(ByVal myMessage As String)
            Me.Message = myMessage
        End Sub

        Private _message As String
        Public Property Message As String
            Get
                Return _message
            End Get
            Private Set(ByVal value As String)
                _message = value
            End Set
        End Property
    End Class


    Public Class ViewModel
        Private _alwaysCommand As ICommand
        Public Property AlwaysCommand As ICommand
            Get
                Return _alwaysCommand
            End Get
            Private Set(ByVal value As ICommand)
                _alwaysCommand = value
            End Set
        End Property

        Private _intermittentCommand As ICommand
        Public Property IntermittentCommand As ICommand
            Get
                Return _intermittentCommand
            End Get
            Private Set(ByVal value As ICommand)
                _intermittentCommand = value
            End Set
        End Property

        Public Event Notify As EventHandler(Of CommandEventArgs)

        Public Sub New()
            AlwaysCommand = New AlwaysICommand
            AddHandler (CType(AlwaysCommand, AlwaysICommand)).Notify1, AddressOf OnNotify
            IntermittentCommand = New IntermittentICommand
            AddHandler (CType(IntermittentCommand, IntermittentICommand)).Notify2, AddressOf OnNotify
        End Sub

        Private Sub OnNotify(ByVal sender As Object, ByVal e As CommandEventArgs)
            RaiseEvent Notify(Me, e)
        End Sub
    End Class


    Public Class AlwaysICommand
        Implements ICommand
        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged As EventHandler Implements System.Windows.Input.ICommand.CanExecuteChanged

        Public Sub Execute(ByVal parameter As Object) Implements System.Windows.Input.ICommand.Execute
            Dim partMsg = If(parameter, "[null]")
            Dim tmpMsg = "AlwaysICommand - " & partMsg.ToString()
            RaiseEvent Notify1(Me, New CommandEventArgs(tmpMsg))
        End Sub


        Public Event Notify1 As EventHandler(Of CommandEventArgs)

    End Class


    Public Class IntermittentICommand
        Implements ICommand
        Private _canExecute As Boolean

        Public Sub New()
            Dim timer As New DispatcherTimer
            timer.Interval = TimeSpan.FromSeconds(1)
            AddHandler timer.Tick, Sub()
                                       _canExecute = Not _canExecute
                                       RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
                                   End Sub
            timer.Start()
        End Sub

        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute
            Return _canExecute
        End Function

        Public Event CanExecuteChanged As EventHandler Implements System.Windows.Input.ICommand.CanExecuteChanged

        Public Sub Execute(ByVal parameter As Object) Implements System.Windows.Input.ICommand.Execute
            System.Diagnostics.Debug.Assert(_canExecute)
            Dim partMsg = If(parameter, "[null]")
            Dim tmpMsg = "IntermittentICommand - " & partMsg.ToString()
            RaiseEvent Notify2(Me, New CommandEventArgs(tmpMsg))
        End Sub

        Public Event Notify2 As EventHandler(Of CommandEventArgs)

    End Class


    Public Class StringICommand
        Implements ICommand
        Private _string As String

        Public Sub New(ByVal s As String)
            _string = s
        End Sub

        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged As EventHandler Implements System.Windows.Input.ICommand.CanExecuteChanged

        Public Sub Execute(ByVal parameter As Object) Implements System.Windows.Input.ICommand.Execute
            Dim partMsg = If(parameter, "[null]")
            Dim tmpMsg = "StringICommand - " & partMsg.ToString()
            RaiseEvent Notify3(Me, New CommandEventArgs(tmpMsg))
        End Sub

        Public Event Notify3 As EventHandler(Of CommandEventArgs)

    End Class

End Namespace
