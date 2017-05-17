' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.ObjectModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows.Controls
Imports System.Windows.Markup

''' <summary>
''' Represents a Catalog of MediaItems. 
''' </summary>
<ContentProperty("MediaItems")> _
Public Class Catalog
    ''' <summary>
    ''' Gets or sets the name of the Catalog.
    ''' </summary>
    Private privateCatalogName As String
    Public Property CatalogName() As String
        Get
            Return privateCatalogName
        End Get
        Set(ByVal value As String)
            privateCatalogName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the Image representing the catalog.
    ''' </summary>
    Private privateCatalogImage As Image
    Public Property CatalogImage() As Image
        Get
            Return privateCatalogImage
        End Get
        Set(ByVal value As Image)
            privateCatalogImage = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a collection of media items. 
    ''' </summary>
    Private privateMediaItems As Collection(Of MediaItem)
    <SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification:="Simplifies samples.")> _
    Public Property MediaItems() As Collection(Of MediaItem)
        Get
            Return privateMediaItems
        End Get
        Set(ByVal value As Collection(Of MediaItem))
            privateMediaItems = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the Catalog class. 
    ''' </summary>
    ''' <param name="catalogName">Name of the catalog.</param>
    ''' <param name="resourceName">
    ''' Name of the resource representing the catalog thumbnail.
    ''' </param>
    Public Sub New(ByVal catalogName As String, ByVal resourceName As String)
        catalogName = catalogName
        CatalogImage = SharedResources.GetImage(resourceName)
        MediaItems = New Collection(Of MediaItem)()
    End Sub

#Region "Sample Data"
    ''' <summary>
    ''' Gets a Vacation Catalog.
    ''' </summary>
    Public Shared ReadOnly Property VacationCatalog() As Catalog
        Get
            Dim catalog As New Catalog("My Vacations", "Sunset.jpg")
            catalog.MediaItems = catalog.VacationMediaItems
            Return catalog
        End Get
    End Property

    ''' <summary>
    ''' Gets a collection of media items representing a catog of upcoming
    ''' vacation trips.
    ''' </summary>
    Public Shared ReadOnly Property PlannedVacationMediaItems() As Collection(Of MediaItem)
        Get
            Dim myCollection As New Collection(Of MediaItem)
            myCollection.Add(New MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", New DateTime(2006, 3, 1)))
            myCollection.Add(New MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", New DateTime(2007, 3, 12)))
            myCollection.Add(New MediaItem("River.jpg", "Italy", "The best days of my life", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Sunset.jpg", "Los Angeles", "I love LA", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", New DateTime(2008, 6, 13)))
            myCollection.Add(New MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", New DateTime(2006, 3, 1)))
            myCollection.Add(New MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", New DateTime(2007, 3, 12)))
            myCollection.Add(New MediaItem("River.jpg", "Italy", "The best days of my life", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Sunset.jpg", "Los Angeles", "I love LA", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", New DateTime(2008, 6, 13)))
            myCollection.Add(New MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", New DateTime(2006, 3, 1)))
            myCollection.Add(New MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", New DateTime(2007, 3, 12)))
            myCollection.Add(New MediaItem("River.jpg", "Italy", "The best days of my life", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Sunset.jpg", "Los Angeles", "I love LA", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", New DateTime(2008, 6, 13)))
            Return myCollection
        End Get
    End Property

    ''' <summary>
    ''' Gets a collection of media items representing a catog of vacaction
    ''' destinations.
    ''' </summary>
    Public Shared ReadOnly Property VacationMediaItems() As Collection(Of MediaItem)
        Get
            Dim myCollection As New Collection(Of MediaItem)
            myCollection.Add(New MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", New DateTime(2006, 3, 1)))
            myCollection.Add(New MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", New DateTime(2007, 3, 12)))
            myCollection.Add(New MediaItem("River.jpg", "Italy", "The best days of my life", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Sunset.jpg", "Los Angeles", "I love LA", New DateTime(2008, 4, 11)))
            myCollection.Add(New MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", New DateTime(2008, 6, 13)))
            myCollection.Add(New MediaItem("Humpback Whale.jpg", "Alaska", "It's Cold in here....", New DateTime(2008, 1, 12)))
            Return myCollection
        End Get
    End Property
#End Region ' Sample Data
End Class
