' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

' To see the correct ApplicationBar icons in the DatePicker and TimePicker, you will need 
' to create a folder in the root of your project called "Toolkit.Content" and put the icons 
' in there. You can copy them from this project. They must be named "ApplicationBar.Cancel.png" 
' and "ApplicationBar.Check.png", and the build action must be "Content".

Namespace Samples

    Partial Public Class DateTimePickerSample
        Inherits PhoneApplicationPage
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub DatePicker_ValueChanged(ByVal sender As Object, ByVal e As DateTimeValueChangedEventArgs)

        End Sub

        Private Sub TimePicker_ValueChanged(ByVal sender As Object, ByVal e As DateTimeValueChangedEventArgs)

        End Sub
    End Class

End Namespace
