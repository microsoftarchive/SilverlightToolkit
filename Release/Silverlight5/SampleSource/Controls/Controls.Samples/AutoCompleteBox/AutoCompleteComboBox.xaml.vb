' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Reflection
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Windows.Data
Imports System.Windows.Media

<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope:="member", Target:="System.Windows.Controls.Samples.AutoCompleteComboBox.#ComboToggleButton", Justification:="Artifact of using a name inside the custom control template.")> 

''' <summary>
''' The AutoCompleteComboBox sample page shows the use of custom data
''' objects, data templates, and a completely custom control template that 
''' acts and looks much like a ComboBox with AutoCompleteBox capabilities.
''' </summary>
<Sample("(3)Styling", DifficultyLevel.Intermediate, "AutoCompleteBox")> _
Partial Public Class AutoCompleteComboBox
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the AutoCompleteComboBox class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the Loaded event of the page.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="rea">The event arguments.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal rea As RoutedEventArgs) Handles Me.Loaded
        ControlApi.ItemFilter = AddressOf MyItemFilter

        ' Reflect and load data
        ControlApi.ItemsSource = MemberInfoData.GetSetForType(GetType(AutoCompleteBox))
        ControlPicker.ItemsSource = InitializeTypeList()

        ' Setup the dictionary converter
        ControlPicker.ValueMemberBinding = New Binding With _
        { _
            .Converter = New DictionaryKeyValueConverter(Of String, Type)() _
        }
    End Sub

    ''' <summary>
    ''' AutoComplete search predicate
    ''' </summary>
    ''' <param name="prefix">The search string.</param>
    ''' <param name="item">The target object.</param>
    Private Function MyItemFilter(ByVal prefix As String, ByVal item As Object) As Boolean
        If String.IsNullOrEmpty(prefix) Then
            Return True
        End If
        Dim pme As MemberInfoData = TryCast(item, MemberInfoData)
        If pme Is Nothing Then
            Return False
        End If
        Return (pme.Name.ToUpper(CultureInfo.InvariantCulture).Contains(prefix.ToUpper(CultureInfo.InvariantCulture)))
    End Function

    ''' <summary>
    ''' Update the API system.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The selection changed event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event is wired up in XAML.")> _
    Private Sub OnPickerChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If ControlPicker.SelectedItem Is Nothing Then
            ControlApi.ItemsSource = Nothing
            ControlApi.Text = Nothing
            IntelliSenseIcon.Content = Nothing
            ControlApi.IsEnabled = False
        Else
            Dim pair As KeyValuePair(Of String, Type) = CType(ControlPicker.SelectedItem, KeyValuePair(Of String, Type))
            ControlApi.ItemsSource = MemberInfoData.GetSetForType(pair.Value)
            ControlApi.IsEnabled = True
        End If
    End Sub

    ''' <summary>
    ''' Update the visible content.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The selection changed event data.</param>
    Private Sub OnApiChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) _
        Handles ControlApi.SelectionChanged, ControlPicker.SelectionChanged

        Dim mim As MemberInfoData = TryCast(ControlApi.SelectedItem, MemberInfoData)
        IntelliSenseIcon.Content = If(ControlApi.SelectedItem Is Nothing, Nothing, mim.Icon)
        SelectedInformation.Text = If(mim Is Nothing, String.Empty, mim.MemberInfo.Name)
    End Sub

    ''' <summary>
    ''' Initializes the type list.
    ''' </summary>
    ''' <returns>Returns a dictionary of string to Type values.</returns>
    Private Shared Function InitializeTypeList() As Dictionary(Of String, Type)
        Dim typeList As Dictionary(Of String, Type) = New Dictionary(Of String, Type)()
        Dim assemblies() As System.Reflection.Assembly = _
        { _
            GetType(Button).Assembly, _
            GetType(TreeView).Assembly, _
            GetType(AutoCompleteBox).Assembly, _
            GetType(Chart).Assembly _
        }
        For Each [assembly] As System.Reflection.Assembly In assemblies
            For Each type As Type In [assembly].GetExportedTypes()
                If type.IsSubclassOf(GetType(Control)) Then
                    typeList.Add(type.FullName, type)
                End If
            Next type
        Next [assembly]
        Return typeList
    End Function

    ''' <summary>
    ''' Toggle the drop down for the control, part of the custom template.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Wired up in markup.")> _
    Private Sub DropDownToggle_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fe As FrameworkElement = TryCast(sender, FrameworkElement)
        Dim acb As AutoCompleteBox = Nothing
        While fe IsNot Nothing AndAlso acb Is Nothing
            fe = TryCast(VisualTreeHelper.GetParent(fe), FrameworkElement)
            acb = TryCast(fe, AutoCompleteBox)
        End While
        If acb IsNot Nothing Then
            If String.IsNullOrEmpty(acb.SearchText) Then
                acb.Text = String.Empty
            End If
            acb.IsDropDownOpen = Not acb.IsDropDownOpen
        End If
    End Sub

End Class

