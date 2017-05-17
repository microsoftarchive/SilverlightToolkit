' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' Sample page demonstrating the GlobalCalendar.
''' </summary>
<Sample("(1)GlobalCalendar", DifficultyLevel.Basic, "GlobalCalendar")> _
Partial Public Class GlobalCalendarSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a new instance of the GlobalCalendarSample class.
    ''' </summary>
    <SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="Setting up the sample")> _
    Sub New()
        InitializeComponent()
        CultureOptions.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Update the culture when the drop down changes.
    ''' </summary>
    ''' <param name="sender">The culture ComboBox.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Attached in XAML.")> _
    Private Sub OnCultureChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim selected As ComboBoxItem = TryCast(CultureOptions.SelectedItem, ComboBoxItem)
        If selected Is Nothing Then
            Return
        End If

        Dim culture As CultureInfo = New CultureInfo(TryCast(selected.Tag, String))
        CulturedCalendar.CalendarInfo = New CultureCalendarInfo(culture)
    End Sub
End Class