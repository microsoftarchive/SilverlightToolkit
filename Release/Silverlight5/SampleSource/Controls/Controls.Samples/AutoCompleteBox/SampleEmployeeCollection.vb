' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Windows.Controls

''' <summary>
''' A collection type that makes it easy to place sample employee data into 
''' XAML.
''' </summary>
Public Class SampleEmployeeCollection
    Inherits ObjectCollection
    ''' <summary>
    ''' Initializes a new instance of the SampleEmployeeCollection class.
    ''' </summary>
    Public Sub New()
        MyBase.New(Employee.AllExecutives)
    End Sub
End Class
