' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic

Namespace Samples

    Partial Public Class PerformanceProgressBarSample
        Inherits PhoneApplicationPage

        Private _progressIndicator As ProgressIndicator

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' This blocks the user interface thread for 4 seconds.
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4))
        End Sub

        Private Sub CheckBox_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs)
            If _progressIndicator IsNot Nothing Then
                _progressIndicator = New ProgressIndicator()
                _progressIndicator.IsVisible = True
                SystemTray.ProgressIndicator = _progressIndicator
            End If
        End Sub

        Private Sub CheckBox_Unchecked(sender As System.Object, e As System.Windows.RoutedEventArgs)
            _progressIndicator.IsIndeterminate = False
        End Sub
    End Class
End Namespace
