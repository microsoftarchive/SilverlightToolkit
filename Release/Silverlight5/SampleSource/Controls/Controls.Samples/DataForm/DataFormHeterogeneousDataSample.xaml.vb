' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Collections.ObjectModel
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DataForm control with heterogeneous data.
''' </summary>
<Sample("DataForm with heterogeneous data", DifficultyLevel.Intermediate, "DataForm")> _
Partial Public Class DataFormHeterogeneousDataSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataFormHeterogeneousDataSample.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        Dim objects As ObservableCollection(Of Object) = New ObservableCollection(Of Object)
        For Each obj In Contact.People
            objects.Add(obj)
        Next
        Dim r As Random = New Random()
        For Each obj In Airport.SampleAirports
            objects.Insert(r.Next(objects.Count), obj)
        Next
        DataContext = objects
    End Sub
End Class