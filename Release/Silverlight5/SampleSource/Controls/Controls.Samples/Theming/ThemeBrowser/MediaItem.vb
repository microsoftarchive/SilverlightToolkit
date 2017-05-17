' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Controls.Samples
Imports System.Collections.ObjectModel
Imports System.Windows.Controls

''' <summary>
'''  Business Object used in the ThemeBrowserSample.
''' </summary>
Partial Public NotInheritable Class MediaItem
    ''' <summary>
    ''' Gets the name of the Media.
    ''' </summary>
    Private privateMediaName As String
    Public Property MediaName() As String
        Get
            Return privateMediaName
        End Get
        Private Set(ByVal value As String)
            privateMediaName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets an Image representing the media.
    ''' </summary>
    Private privateImage As Image
    Public Property Image() As Image
        Get
            Return privateImage
        End Get
        Private Set(ByVal value As Image)
            privateImage = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets Description of the media.
    ''' </summary>
    Private privateDescription As String
    Public Property Description() As String
        Get
            Return privateDescription
        End Get
        Set(ByVal value As String)
            privateDescription = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets Date Stump of the media.
    ''' </summary>
    Private privateDate As DateTime
    Public Property [Date]() As DateTime
        Get
            Return privateDate
        End Get
        Set(ByVal value As DateTime)
            privateDate = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the Catalog class.
    ''' </summary>
    ''' <param name="resourceName">Name of the resource defining the Image of the media.</param>
    ''' <param name="mediaName">Name of the media.</param>
    ''' <param name="description">Description of the media.</param>
    ''' <param name="date">Date Stump for the media.</param>
    Public Sub New(ByVal resourceName As String, ByVal mediaName As String, ByVal description As String, ByVal [date] As DateTime)
        Me.MediaName = mediaName
        Me.Description = description
        Me.Image = SharedResources.GetImage(resourceName)
        Me.[Date] = [date]
    End Sub

    ''' <summary>
    ''' Overrides the string to return the name.
    ''' </summary>
    ''' <returns>Returns the photograph name.</returns>
    Public Overrides Function ToString() As String
        Return MediaName
    End Function
End Class

