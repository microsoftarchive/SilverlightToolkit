' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

<Assembly: SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope:="member", Target:="System.Windows.Controls.Samples.UpDownBaseSample+SUD.#.ctor()")> 
<Assembly: SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope:="member", Target:="System.Windows.Controls.Samples.UpDownBaseSample+TUD.#.ctor()")> 
<Assembly: SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope:="member", Target:="System.Windows.Controls.Samples.UpDownBaseSample+NUD.#.ctor()")> 

''' <summary>
''' UpDownBase sample page.
''' </summary>
<Sample("UpDownBase", DifficultyLevel.Basic, "UpDownBase")> _
Partial Public Class UpDownBaseSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the UpDownBaseSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' create instance of the test non generic sub classes
        spNUD.Children.Add(New NUD())
        spTUD.Children.Add(New TUD())
        spSUD.Children.Add(New SUD())
    End Sub

    ''' <summary>
    ''' This function does following three things:
    ''' 1. Remove existing interactive UpDownBase control if it exists;
    ''' 2. Create a new instance of selected UpDownBase subclass type;
    ''' 3. Add the newly created UpDownBase control to visual tree.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub CreateUDB(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles btnChange.Click
        If udb IsNot Nothing Then
            Debug.Assert(udb Is sp.Children(0), "udb should be sp's only child!")
            sp.Children.Remove(udb)
        End If

        Select Case lbType.SelectedIndex
            Case 0
                udb = New NUD()
                ConfigUDB(CType(udb, UpDownBase(Of Integer)))
            Case 1
                udb = New TUD()
                ConfigUDB(CType(udb, UpDownBase(Of DateTime)))
            Case 2
                udb = New SUD()
                ConfigUDB(CType(udb, UpDownBase(Of String)))
        End Select

        sp.Children.Add(udb)
    End Sub

    ''' <summary>
    ''' Configure a UpDownBase&lt;T&gt; control.
    ''' </summary>
    ''' <typeparam name="T">Type parameter.</typeparam>
    ''' <param name="udbt">UpDownBase&lt;T&gt; control.</param>
    Private Sub ConfigUDB(Of T)(ByVal udbt As UpDownBase(Of T))
        Debug.Assert(udbt IsNot Nothing, "udb should not be null!")

        udbt.Value = ParseUtility.ReadValue(tbValue, udbt.Value)
        ' udbt.IsCyclic = (bool)cbIsCyclic.IsChecked;
        udbt.IsEditable = CBool(cbIsEditable.IsChecked)
        udbt.IsEnabled = CBool(cbIsEnabled.IsChecked)
    End Sub

    ''' <summary>
    ''' The interactive UpDownBase control.
    ''' </summary>
    Private udb As UpDownBase

#Region "Non-generic test classes inheriting from UpDownBase<T>"
    ''' <summary>
    ''' Non-generic test class inheriting from UpDownBase&lt;int&gt;.
    ''' </summary>
    ''' <remarks>
    ''' Value is of type int, in between 0 and 10.
    ''' IsCyclic decides whether Value will wrap around 0 and 10.
    ''' OnIncrement and OnDecrement change Value by 1.
    ''' </remarks>
    <TemplateVisualState(Name:="Normal", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="MouseOver", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Pressed", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Disabled", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Focused", GroupName:="FocusStates"), _
    TemplateVisualState(Name:="Unfocused", GroupName:="FocusStates"), _
    TemplatePart(Name:="TextName", Type:=GetType(TextBox)), _
    TemplatePart(Name:="SpinnerName", Type:=GetType(Spinner)), _
    StyleTypedProperty(Property:="SpinnerStyle", StyleTargetType:=GetType(Spinner))> _
    Friend Class NUD
        Inherits UpDownBase(Of Integer)
        ''' <summary>
        ''' Private variable to replace IsCyclic property for now removed from UpDownBase&lt;T&gt;.
        ''' </summary>
        Private IsCyclic As Boolean = True

        ''' <summary>
        ''' Internal constructor.
        ''' </summary>
        Friend Sub New()
            MyBase.New()
            Value = 0
        End Sub

        ''' <summary>
        ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
        ''' </summary>
        Protected Overrides Sub OnIncrement()
            Value = If(IsCyclic, (Value + 1) Mod 10, Math.Min(10, Value + 1))
        End Sub

        ''' <summary>
        ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
        ''' </summary>
        Protected Overrides Sub OnDecrement()
            Value = If(IsCyclic, (Value - 1) Mod 10, Math.Max(0, Value - 1))
        End Sub

        ''' <summary>
        ''' Called by ApplyValue to parse user input.
        ''' </summary>
        ''' <param name="text">User input.</param>
        ''' <returns>Value parsed from user input.</returns>
        Protected Overrides Function ParseValue(ByVal text As String) As Integer
            Return Integer.Parse(text, CultureInfo.CurrentCulture)
        End Function

        ''' <summary>
        ''' Called to render Value for Text template part to display.
        ''' </summary>
        ''' <returns>Formatted Value.</returns>
        Protected Overrides Function FormatValue() As String
            Return Value.ToString(CultureInfo.CurrentCulture)
        End Function
    End Class

    ''' <summary>
    ''' Non-generic test class inheriting from UpDownBase&lt;DateTime&gt;.
    ''' </summary>
    ''' <remarks>
    ''' Value is of type DateTime. 
    ''' IsCyclic decides whether Value will wrap around DateTime.MinValue and DateTime.MaxValue.
    ''' OnIncrement and OnDecrement change Value by one hour.
    ''' </remarks>
    <TemplateVisualState(Name:="Normal", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="MouseOver", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Pressed", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Disabled", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Focused", GroupName:="FocusStates"), _
    TemplateVisualState(Name:="Unfocused", GroupName:="FocusStates"), _
    TemplatePart(Name:="TextName", Type:=GetType(TextBox)), _
    TemplatePart(Name:="SpinnerName", Type:=GetType(Spinner)), _
    StyleTypedProperty(Property:="SpinnerStyle", StyleTargetType:=GetType(Spinner))> _
    Friend Class TUD
        Inherits UpDownBase(Of DateTime)
        ''' <summary>
        ''' Private variable to replace IsCyclic property for now removed from UpDownBase&lt;T&gt;.
        ''' </summary>
        Private IsCyclic As Boolean = True

        ''' <summary>
        ''' Internal constructor.
        ''' </summary>
        Friend Sub New()
            MyBase.New()
            Value = DateTime.Now
        End Sub

        ''' <summary>
        ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
        ''' </summary>
        Protected Overrides Sub OnIncrement()
            Try
                Value = Value.AddHours(1)
            Catch e1 As ArgumentOutOfRangeException
                Value = If(IsCyclic, DateTime.MinValue, DateTime.MaxValue)
            End Try
        End Sub

        ''' <summary>
        ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
        ''' </summary>
        Protected Overrides Sub OnDecrement()
            Try
                Value = Value.AddHours(-1)
            Catch e1 As ArgumentOutOfRangeException
                Value = If(IsCyclic, DateTime.MaxValue, DateTime.MinValue)
            End Try
        End Sub

        ''' <summary>
        ''' Called by ApplyValue to parse user input.
        ''' </summary>
        ''' <param name="text">User input.</param>
        ''' <returns>Value parsed from user input.</returns>
        Protected Overrides Function ParseValue(ByVal text As String) As DateTime
            Return DateTime.Parse(text, CultureInfo.CurrentCulture)
        End Function

        ''' <summary>
        ''' Called to render Value for Text template part to display.
        ''' </summary>
        ''' <returns>Formatted Value.</returns>
        Protected Overrides Function FormatValue() As String
            Return Value.ToShortTimeString()
        End Function
    End Class

    ''' <summary>
    ''' Non-generic class for UpDownBase&lt;T&gt; testing only.
    ''' </summary>
    ''' <remarks>
    ''' Value is of type string.
    ''' OnIncrement and OnDecrement change string length by 1 character.
    ''' IsCyclic decides whether Value length will cycle through 0 to 10.
    ''' </remarks>
    <TemplateVisualState(Name:="Normal", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="MouseOver", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Pressed", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Disabled", GroupName:="CommonStates"), _
    TemplateVisualState(Name:="Focused", GroupName:="FocusStates"), _
    TemplateVisualState(Name:="Unfocused", GroupName:="FocusStates"), _
    TemplatePart(Name:="TextName", Type:=GetType(TextBox)), _
    TemplatePart(Name:="SpinnerName", Type:=GetType(Spinner)), _
    StyleTypedProperty(Property:="SpinnerStyle", StyleTargetType:=GetType(Spinner))> _
    Friend Class SUD
        Inherits UpDownBase(Of String)
        ''' <summary>
        ''' Private variable to replace IsCyclic property for now removed from UpDownBase&lt;T&gt;.
        ''' </summary>
        Private IsCyclic As Boolean = True

        ''' <summary>
        ''' Internal constructor.
        ''' </summary>
        Friend Sub New()
            MyBase.New()
            Me.Value = "0"
        End Sub

        ''' <summary>
        ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
        ''' </summary>
        Protected Overrides Sub OnIncrement()
            Dim value As String = If(String.IsNullOrEmpty(Me.Value), "0", Me.Value)
            If value.Length >= 10 Then
                If IsCyclic Then
                    value = "0"
                End If
            Else
                value = value & value.Length.ToString(CultureInfo.CurrentCulture)
            End If
            Me.Value = value
        End Sub

        ''' <summary>
        ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
        ''' </summary>
        Protected Overrides Sub OnDecrement()
            Dim value As String = If(String.IsNullOrEmpty(Me.Value), "0", Me.Value)
            If value.Length <= 1 Then
                If IsCyclic Then
                    value = "0123456789"
                End If
            Else
                value = value.Substring(0, value.Length - 1)
            End If
            Me.Value = value
        End Sub

        ''' <summary>
        ''' Called by ApplyValue to parse user input.
        ''' </summary>
        ''' <param name="text">User input.</param>
        ''' <returns>Value parsed from user input.</returns>
        Protected Overrides Function ParseValue(ByVal text As String) As String
            If String.IsNullOrEmpty(text) Then
                text = "0"
            End If

            If text.Length > 10 Then
                text = text.Substring(0, 10)
            End If

            Return text
        End Function

        ''' <summary>
        ''' Called to render Value for Text template part to display.
        ''' </summary>
        ''' <returns>Formatted Value.</returns>
        Protected Overrides Function FormatValue() As String
            Return If(String.IsNullOrEmpty(Value), "0", Value)
        End Function
    End Class
#End Region
End Class