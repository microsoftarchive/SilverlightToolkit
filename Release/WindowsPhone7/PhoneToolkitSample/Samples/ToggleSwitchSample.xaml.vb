' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Samples

    Partial Public Class ToggleSwitchSample
        Inherits PhoneApplicationPage
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub OnAlarmTap(ByVal sender As Object, ByVal e As GestureEventArgs)
            System.Diagnostics.Debug.WriteLine("Alarm tapped!")
        End Sub
    End Class
End Namespace
