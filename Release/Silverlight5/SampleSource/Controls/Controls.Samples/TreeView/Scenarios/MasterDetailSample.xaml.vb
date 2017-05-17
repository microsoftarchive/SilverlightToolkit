' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating the TreeView used in a Master/Detail scenario.
''' </summary>
<Sample("MasterDetail", DifficultyLevel.Scenario, "TreeView")> _
Partial Public Class MasterDetailSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the MasterDetailSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        MasterTree.ItemsSource = Taxonomy.Life
    End Sub

    ''' <summary>
    ''' Handle the TreeView.SelectedItemChanged event.
    ''' </summary>
    ''' <param name="sender">The TreeView.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub MasterTree_SelectedItemChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Object))
        ' Setting the DataContext on the panel containing all the 
        ' detail controls allows setting the Master object once
        ' instead of once per control.
        DetailsPanel.DataContext = e.NewValue

        ' Simulate looking up data in another data source.
        If e.NewValue IsNot Nothing Then
            Dim taxonomy As Taxonomy = CType(e.NewValue, Taxonomy)

            Dim information As New StringBuilder()
            information.AppendFormat(CultureInfo.CurrentCulture, "The {0} {1}, represents a signifigant portion of this sample text." & Constants.vbLf + Constants.vbLf, taxonomy.Rank, taxonomy.Classification)

            Select Case taxonomy.Subclasses.Count
                Case 0
                    information.Append("Doesn't contain any subclasses.")

                Case 1
                    information.Append("This contains only a single subclass.")

                Case Else
                    information.AppendFormat(CultureInfo.CurrentCulture, "Contains {0} subclasses.", taxonomy.Subclasses.Count)
            End Select

            LookupDetailText.Text = information.ToString()
        End If
    End Sub
End Class

