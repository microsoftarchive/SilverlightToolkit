' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Globalization

''' <summary>
''' Tracks vacation budget across a period of time.
''' </summary>
Public Class VacationBudgetCollection
    Implements IEnumerable(Of Budget)
    ''' <summary>
    ''' Returns a stream of expenses across a period of time.
    ''' </summary>
    ''' <returns>A stream of expenses for this vacation.</returns>
    Public Function GetEnumerator() As IEnumerator(Of Budget) Implements IEnumerable(Of Budget).GetEnumerator
        Dim myList As New List(Of Budget)()
        myList.Add(New Budget With {.Date = DateTime.Parse("9/6/2008", CultureInfo.InvariantCulture), .ExpenseValue = 53})
        myList.Add(New Budget With {.Date = DateTime.Parse("9/2/2008", CultureInfo.InvariantCulture), .ExpenseValue = 42})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/31/2008", CultureInfo.InvariantCulture), .ExpenseValue = 43})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/30/2008", CultureInfo.InvariantCulture), .ExpenseValue = 43})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/23/2008", CultureInfo.InvariantCulture), .ExpenseValue = 47})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/22/2008", CultureInfo.InvariantCulture), .ExpenseValue = 45})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/21/2008", CultureInfo.InvariantCulture), .ExpenseValue = 40})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/19/2008", CultureInfo.InvariantCulture), .ExpenseValue = 39})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/17/2008", CultureInfo.InvariantCulture), .ExpenseValue = 42})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/16/2008", CultureInfo.InvariantCulture), .ExpenseValue = 43})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/15/2008", CultureInfo.InvariantCulture), .ExpenseValue = 42})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/12/2008", CultureInfo.InvariantCulture), .ExpenseValue = 47})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/10/2008", CultureInfo.InvariantCulture), .ExpenseValue = 43})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/7/2008", CultureInfo.InvariantCulture), .ExpenseValue = 38})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/3/2008", CultureInfo.InvariantCulture), .ExpenseValue = 39})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/2/2008", CultureInfo.InvariantCulture), .ExpenseValue = 41})
        myList.Add(New Budget With {.Date = DateTime.Parse("8/1/2008", CultureInfo.InvariantCulture), .ExpenseValue = 41})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/28/2008", CultureInfo.InvariantCulture), .ExpenseValue = 44})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/26/2008", CultureInfo.InvariantCulture), .ExpenseValue = 49})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/25/2008", CultureInfo.InvariantCulture), .ExpenseValue = 42})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/22/2008", CultureInfo.InvariantCulture), .ExpenseValue = 40})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/19/2008", CultureInfo.InvariantCulture), .ExpenseValue = 41})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/12/2008", CultureInfo.InvariantCulture), .ExpenseValue = 46})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/11/2008", CultureInfo.InvariantCulture), .ExpenseValue = 40})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/10/2008", CultureInfo.InvariantCulture), .ExpenseValue = 41})
        myList.Add(New Budget With {.Date = DateTime.Parse("7/9/2008", CultureInfo.InvariantCulture), .ExpenseValue = 39})
        Return myList.GetEnumerator()
    End Function

    ''' <summary>
    ''' Returns a stream of vacation expenses.
    ''' </summary>
    ''' <returns>A stream of vacation expenses.</returns>
    Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return (CType(Me, IEnumerable(Of Budget))).GetEnumerator()
    End Function
End Class
