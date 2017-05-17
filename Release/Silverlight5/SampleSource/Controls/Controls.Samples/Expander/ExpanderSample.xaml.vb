' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the Expander.
''' </summary>
<SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId:="Expander", Justification:="Name of the control"), Sample("Expander", DifficultyLevel.Basic, "Expander")> _
Partial Public Class ExpanderSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ExpanderSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Load the demo page.
    ''' </summary>
    ''' <param name="sender">Sample page.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        tbHeader.Text = "Header"
        tbHeaderTemplate.Text = "<DataTemplate><StackPanel Orientation=""Horizontal""><Ellipse Width=""10"" Height=""10"" Fill=""Red""/><Button Content=""{Binding}""/><Ellipse Width=""10"" Height=""10"" Fill=""Red""/></StackPanel></DataTemplate>"
        tbContent.Text = "Content"
        tbContentTemplate.Text = "<DataTemplate><Button Content=""{Binding}""/></DataTemplate>"
    End Sub

    ''' <summary>
    ''' Change customization area expansion behavior.
    ''' </summary>
    ''' <param name="sender">Sender Button.</param>
    ''' <param name="e">Event args.</param>
    Private Sub lbCustDirection_SelectionChanged(ByVal x As Object, ByVal y As Object) Handles lbCustDirection.SelectionChanged
        Dim d As ExpandDirection = CType(lbCustDirection.SelectedIndex, ExpandDirection)
        expNoButton.ExpandDirection = d
        expBRButton.ExpandDirection = d
        expFade.ExpandDirection = d
        expScale.ExpandDirection = d
    End Sub

    ''' <summary>
    ''' Change ContentControl settings (Header, HeaderTemplate, Content, ContentTemplate).
    ''' </summary>
    ''' <param name="sender">Sender Button.</param>
    ''' <param name="e">Event args.</param>
    Private Sub ChangeSettings(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles btnChange.Click
        expander.ExpandDirection = CType(lbExpandDirection.SelectedIndex, ExpandDirection)
        expander.Header = tbHeader.Text
        expander.Content = tbContent.Text
        expander.IsEnabled = If(cbIsEnabled.IsChecked.HasValue, cbIsEnabled.IsChecked.Value, True)

        If String.IsNullOrEmpty(tbHeaderTemplate.Text) Then
            expander.ClearValue(HeaderedContentControl.HeaderTemplateProperty)
        Else
            Try
                expander.HeaderTemplate = CType(XamlReader.Load(AddXmlNS(tbHeaderTemplate.Text)), DataTemplate)
                tbHeaderTemplate.Foreground = New SolidColorBrush(Colors.Black)
            Catch e1 As XamlParseException
                tbHeaderTemplate.Foreground = New SolidColorBrush(Colors.Red)
            End Try
        End If

        If String.IsNullOrEmpty(tbContentTemplate.Text) Then
            expander.ClearValue(HeaderedContentControl.ContentTemplateProperty)
        Else
            Try
                expander.ContentTemplate = CType(XamlReader.Load(AddXmlNS(tbContentTemplate.Text)), DataTemplate)
                tbContentTemplate.Foreground = New SolidColorBrush(Colors.Black)
            Catch e2 As XamlParseException
                tbContentTemplate.Foreground = New SolidColorBrush(Colors.Red)
            End Try
        End If

        OutputExpander(sender, e)
    End Sub

    ''' <summary>
    ''' Utility function to add xmlns to user inputed template string.
    ''' </summary>
    ''' <param name="s">User input template string.</param>
    ''' <returns>Well formated xaml with xmlns added.</returns>
    Private Shared Function AddXmlNS(ByVal s As String) As String
        Dim xmlns As String = " xmlns=""http://schemas.microsoft.com/client/2007""" & ControlChars.CrLf & _
        "                              xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""" & ControlChars.CrLf & _
        "                              xmlns:controls=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.InternalOnly"" "
        Dim i As Integer = s.IndexOf(">", StringComparison.Ordinal)
        Return If((i = -1), ">/", s.Substring(0, i) & xmlns & s.Substring(i))
    End Function

    ''' <summary>
    ''' Display the interactive Expander control's properties.
    ''' </summary>
    Private Sub OutputExpander(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles expander.Expanded, expander.Collapsed
        Dim formatString As String = Constants.vbLf + Constants.vbLf & _
            " ExpandDirection:" & Constants.vbTab & "{0}" & Constants.vbLf & _
            " Header:" & Constants.vbTab & "{1}" & Constants.vbLf & _
            " Content:" & Constants.vbTab & "{2}" & Constants.vbLf & _
            " IsExpanded:" & Constants.vbTab & "{3}" & Constants.vbLf & _
            " IsEnabled:" & Constants.vbTab & "{4}" & Constants.vbLf

        output.Text = String.Format( _
            CultureInfo.InvariantCulture, _
            formatString, _
            expander.ExpandDirection, _
            expander.Header, _
            expander.Content, _
            expander.IsExpanded, _
            expander.IsEnabled)
    End Sub
End Class