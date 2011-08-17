' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Samples

    Partial Public Class ListPickerSample
        Inherits PhoneApplicationPage
        Private Shared ReadOnly AccentColors() As String =
            {"magenta", "purple", "teal", "lime", "brown", "pink", "orange", "blue", "red", "green"}

        Public Sub New()
            InitializeComponent()

            DataContext = AccentColors
        End Sub
    End Class

End Namespace
