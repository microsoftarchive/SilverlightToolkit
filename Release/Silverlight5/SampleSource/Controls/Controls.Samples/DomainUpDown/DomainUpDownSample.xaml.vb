' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data
Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Collections
Imports System.Reflection
Imports System.ComponentModel
Imports System.Collections.Generic

''' <summary>
''' Sample page demonstrating DomainUpDown.
''' </summary>
<Sample("(0)DomainUpDown", DifficultyLevel.Basic, "DomainUpDown")> _
Partial Public Class DomainUpDownSample
    Inherits UserControl
#Region "Cultures"
    ''' <summary>
    ''' Cultures that are recognized on most configurations.
    ''' </summary>
    Private ReadOnly _cultureNames As String() = New String() {"af", "af-ZA", "ar", "ar-AE", "ar-BH", "ar-DZ", _
                "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", _
                "ar-MA", "ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", _
                "ar-YE", "az", "az-Cyrl-AZ", "az-Latn-AZ", "be", "be-BY", _
                "bg", "bg-BG", "ca", "ca-ES", "cs", "cs-CZ", _
                "da", "da-DK", "de", "de-AT", "de-CH", "de-DE", _
                "de-LI", "de-LU", "dv", "dv-MV", "el", "el-GR", _
                "en", "en-029", "en-AU", "en-BZ", "en-CA", "en-GB", _
                "en-IE", "en-JM", "en-NZ", "en-PH", "en-TT", "en-US", _
                "en-ZA", "en-ZW", "es", "es-AR", "es-BO", "es-CL", _
                "es-CO", "es-CR", "es-DO", "es-EC", "es-ES", "es-GT", _
                "es-HN", "es-MX", "es-NI", "es-PA", "es-PE", "es-PR", _
                "es-PY", "es-SV", "es-UY", "es-VE", "et", "et-EE", _
                "eu", "eu-ES", "fa", "fa-IR", "fi", "fi-FI", _
                "fo", "fo-FO", "fr", "fr-BE", "fr-CA", "fr-CH", _
                "fr-FR", "fr-LU", "fr-MC", "gl", "gl-ES", "gu", _
                "gu-IN", "he", "he-IL", "hi", "hi-IN", "hr", _
                "hr-HR", "hu", "hu-HU", "hy", "hy-AM", "id", _
                "id-ID", "is", "is-IS", "it", "it-CH", "it-IT", _
                "ja", "ja-JP", "ka", "ka-GE", "kk", "kk-KZ", _
                "kn", "kn-IN", "ko", "kok", "kok-IN", "ko-KR", _
                "ky", "ky-KG", "lt", "lt-LT", "lv", "lv-LV", _
                "mk", "mk-MK", "mn", "mn-MN", "mr", "mr-IN", _
                "ms", "ms-BN", "ms-MY", "nb-NO", "nl", "nl-BE", _
                "nl-NL", "nn-NO", "no", "pa", "pa-IN", "pl", _
                "pl-PL", "pt", "pt-BR", "pt-PT", "ro", "ro-RO", _
                "ru", "ru-RU", "sa", "sa-IN", "sk", "sk-SK", _
                "sl", "sl-SI", "sq", "sq-AL", "sr", "sr-Cyrl-CS", _
                "sr-Latn-CS", "sv", "sv-FI", "sv-SE", "sw", "sw-KE", _
                "syr", "syr-SY", "ta", "ta-IN", "te", "te-IN", _
                "th", "th-TH", "tr", "tr-TR", "tt", "tt-RU", _
                "uk", "uk-UA", "ur", "ur-PK", "uz", "uz-Cyrl-UZ", _
                "uz-Latn-UZ", "vi", "vi-VN", "zh-CHS", "zh-CHT", "zh-CN", _
                "zh-HK", "zh-MO", "zh-SG", "zh-TW"}
#End Region

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

        Dim cultures As New List(Of CultureInfo)()
        Dim dutch As CultureInfo = Nothing

        ' work through long list of cultures and check if it is actually 
        ' allowed in this configuration.
        For Each cultureName As String In _cultureNames
            Try
                Dim c As New CultureInfo(cultureName)
                If (cultureName = "nl-NL") Then
                    dutch = c
                End If

                cultures.Add(c)
            Catch generatedExceptionName As ArgumentException
            End Try
        Next

        cultureList.ItemsSource = cultures
        ' preselect dutch, if allowed.
        If (dutch IsNot Nothing) Then
            cultureList.SelectedItem = dutch
        End If

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
            Dim copy As Binding = New Binding(cultureDependentDUD.ValueMemberPath)
            copy.Converter = cultureDependentDUD.ValueMemberBinding.Converter
            copy.ConverterCulture = c
            cultureDependentDUD.ValueMemberBinding = copy
        End If
    End Sub
End Class
