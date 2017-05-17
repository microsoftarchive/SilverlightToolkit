' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the HeaderedContentControl.
''' </summary>
<SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId:="Headered", Justification:="Name of the control"), Sample("HeaderedContentControl", DifficultyLevel.Basic, "Expander")> _
Partial Public Class HeaderedContentControlSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the HeaderedContentControlSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Change ContentControl settings (Header, HeaderTemplate, Content, ContentTemplate).
    ''' </summary>
    ''' <param name="sender">Sender Button.</param>
    ''' <param name="e">Event args.</param>
    Private Sub ChangeSettings(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles btnChange.Click
        HCC.Header = tbHeader.Text
        HCC.Content = tbContent.Text

        If String.IsNullOrEmpty(tbHeaderTemplate.Text) Then
            HCC.ClearValue(HeaderedContentControl.HeaderTemplateProperty)
        Else
            Try
                HCC.HeaderTemplate = CType(XamlReader.Load(AddXmlNS(tbHeaderTemplate.Text)), DataTemplate)
                tbHeaderTemplate.Foreground = New SolidColorBrush(Colors.Black)
            Catch e1 As XamlParseException
                tbHeaderTemplate.Foreground = New SolidColorBrush(Colors.Red)
            End Try
        End If

        If String.IsNullOrEmpty(tbContentTemplate.Text) Then
            HCC.ClearValue(HeaderedContentControl.ContentTemplateProperty)
        Else
            Try
                HCC.ContentTemplate = CType(XamlReader.Load(AddXmlNS(tbContentTemplate.Text)), DataTemplate)
                tbContentTemplate.Foreground = New SolidColorBrush(Colors.Black)
            Catch e2 As XamlParseException
                tbContentTemplate.Foreground = New SolidColorBrush(Colors.Red)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Utility function to add xmlns to user inputed template string.
    ''' </summary>
    ''' <param name="s">User input template string.</param>
    ''' <returns>Well formated xaml with xmlns added.</returns>
    Private Shared Function AddXmlNS(ByVal s As String) As String
        Dim xmlns As String = " xmlns=""http://schemas.microsoft.com/client/2007""" & ControlChars.CrLf & _
        "                              xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""" & ControlChars.CrLf & _
        "                              xmlns:controls=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls"" "
        Dim i As Integer = s.IndexOf(">", StringComparison.Ordinal)
        Return If((i = -1), ">/", s.Substring(0, i) & xmlns & s.Substring(i))
    End Function
End Class