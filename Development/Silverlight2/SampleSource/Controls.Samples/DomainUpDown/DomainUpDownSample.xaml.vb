' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Collections
Imports System.Reflection
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating DomainUpDown.
''' </summary>
<Sample("(0)DomainUpDown", DifficultyLevel.Basic), Category("DomainUpDown")> _
Partial Public Class DomainUpDownSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the <see cref="DomainUpDownSample"/> class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handles the Loaded event of the DomainUpDownSample control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    Private Sub DomainUpDownSample_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        Dim airports As IEnumerable = Airport.SampleAirports
        DataContext = airports

        ' fairly random list of cultures
        Dim cultures() As CultureInfo = New CultureInfo() _
                                        { _
                                            New CultureInfo("zh-Hans"), _
                                            New CultureInfo("da"), _
                                            New CultureInfo("nl-NL"), _
                                            New CultureInfo("en-US"), _
                                            New CultureInfo("fr"), _
                                            New CultureInfo("de"), _
                                            New CultureInfo("he"), _
                                            New CultureInfo("it"), _
                                            New CultureInfo("ru"), _
                                            New CultureInfo("es-ES") _
                                        }
        cultureList.ItemsSource = cultures
        cultureList.SelectedIndex = 3
    End Sub

    ''' <summary>
    ''' Handles the ParseError event of the DomainUpDown control.
    ''' If a color can be found to, a new border will be added to our items collection.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.Branched.UpDownParseErrorEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub DomainUpDown_ParseError(ByVal sender As Object, ByVal e As UpDownParseErrorEventArgs)
        Dim dud As DomainUpDown = CType(sender, DomainUpDown)

        ' get the text that was unable to parse.
        Dim text As String = e.Text

        Dim backgroundColor As SolidColorBrush = Nothing

        ' is might be a known color string, like "Yellow" 
        ' get by looking at the Colors class.
        Dim colorPropertyInfo As PropertyInfo = GetType(Colors).GetProperty(text, BindingFlags.Static Or BindingFlags.Public)
        If colorPropertyInfo IsNot Nothing Then
            backgroundColor = New SolidColorBrush(CType(colorPropertyInfo.GetValue(Nothing, New Object() {}), Color))
        Else
            ' it might be rgba code, like #aarrggbb
            If text.StartsWith("#", StringComparison.OrdinalIgnoreCase) AndAlso text.Length = 9 Then
                ' rrggbbaa
                text = text.Substring(1)

                ' test to see if it can be parsed to an int
                Dim result As Integer
                If Int32.TryParse(text, result) Then
                    Dim rgba(3) As Byte
                    For i As Integer = 0 To 3
                        rgba(i) = Byte.Parse(text.Substring(i * 2, 2), CultureInfo.CurrentCulture)
                    Next i
                    backgroundColor = New SolidColorBrush(New Color With {.A = rgba(0), .B = rgba(3), .G = rgba(2), .R = rgba(1)})
                End If
            End If
        End If

        If backgroundColor IsNot Nothing Then
            dud.Items.Add(New Border With _
            { _
                .Width = 120, _
                .Height = 80, _
                .Background = backgroundColor, _
                .BorderBrush = New SolidColorBrush(Colors.Yellow), _
                .BorderThickness = New Thickness(4) _
            })
            dud.CurrentIndex = dud.Items.Count - 1
        End If
    End Sub

    ''' <summary>
    ''' Handles the SelectionChanged event of the CultureList control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub CultureList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim c As CultureInfo = TryCast(cultureList.SelectedItem, CultureInfo)
        If c IsNot Nothing Then
            cultureDependentDUD.ConverterCulture = c
        End If
    End Sub
End Class
