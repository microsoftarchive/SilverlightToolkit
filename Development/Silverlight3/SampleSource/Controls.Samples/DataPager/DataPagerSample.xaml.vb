' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DataPager.
''' </summary>
<Sample("DataPager", DifficultyLevel.Basic)> _
<Category("DataPager")> _
Public Class DataPagerSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataPagerSample.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        Dim pcv As PagedCollectionView = New PagedCollectionView(Airport.SampleAirports)
        pcv.PageSize = 6
        DataContext = pcv
    End Sub

    ''' <summary>
    ''' Initializes the sample when the Loaded event is raised.
    ''' </summary>
    ''' <param name="sender">The sample page.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub UserControl_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        pagerFln.DisplayMode = PagerDisplayMode.FirstLastNumeric
        pagerFlpn.DisplayMode = PagerDisplayMode.FirstLastPreviousNext
        pagerFlpnn.DisplayMode = PagerDisplayMode.FirstLastPreviousNextNumeric
        pagerN.DisplayMode = PagerDisplayMode.Numeric
        pagerPn.DisplayMode = PagerDisplayMode.PreviousNext
        pagerPnn.DisplayMode = PagerDisplayMode.PreviousNextNumeric
        pagerFln.PageIndex = 1
        pagerFln.PageIndex = 0
    End Sub
End Class
