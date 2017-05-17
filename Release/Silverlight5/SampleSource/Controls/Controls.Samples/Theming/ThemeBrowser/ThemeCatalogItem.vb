' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows.Media
Imports System.Windows.Controls

''' <summary>
''' Buisness Object used in the ThemeBrowserSample.
''' </summary>
Partial Public NotInheritable Class ThemeCatalogItem
    ''' <summary>
    '''  Gets or sets the name of the theme.
    ''' </summary>
    Private privateThemeName As String
    Public Property ThemeName() As String
        Get
            Return privateThemeName
        End Get
        Set(ByVal value As String)
            privateThemeName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the thumnail Image representing the theme.
    ''' </summary>
    Private privateThumbnailImage As Image
    Public Property ThumbnailImage() As Image
        Get
            Return privateThumbnailImage
        End Get
        Set(ByVal value As Image)
            privateThumbnailImage = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the prefered background brush for this theme.
    ''' </summary>
    Private privatePreferredBackground As Brush
    Public Property PreferredBackground() As Brush
        Get
            Return privatePreferredBackground
        End Get
        Set(ByVal value As Brush)
            privatePreferredBackground = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the path of the theme xaml file for this theme.
    ''' </summary>
    Private privateThemeXamlPath As String
    Public Property ThemeXamlPath() As String
        Get
            Return privateThemeXamlPath
        End Get
        Set(ByVal value As String)
            privateThemeXamlPath = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the ThemeCatologItem class.
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the ThemeCatologItem class.
    ''' </summary>
    ''' <param name="themeName">Name of the theme.</param>
    ''' <param name="resourceName">
    ''' Name of the resource defining the Thumbnail for the theme.
    ''' </param>
    ''' <param name="themeXamlPath">
    ''' Path to the XAML file for this theme.
    ''' </param>
    ''' <param name="preferredBackground">
    ''' Preferred Background for the theme.
    ''' </param>
    Friend Sub New(ByVal themeName As String, ByVal resourceName As String, ByVal themeXamlPath As String, ByVal preferredBackground As Brush)
        Me.ThemeName = themeName
        Me.ThumbnailImage = SharedResources.GetImage(resourceName)
        Me.ThemeXamlPath = themeXamlPath
        Me.PreferredBackground = preferredBackground
    End Sub

#Region "Sample Data"
    ''' <summary>
    ''' Gets a Theme object that represents Shiny Blue.
    ''' </summary>
    Public Shared ReadOnly Property ShinyBlue() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 132, 150, 170), Color.FromArgb(255, 43, 49, 56))
            Return New ThemeCatalogItem("Shiny Blue", "thumbShinyBlue.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/ShinyBlue.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents Shiny Red.
    ''' </summary>
    Public Shared ReadOnly Property ShinyRed() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 209, 209, 209), Color.FromArgb(255, 174, 49, 56))
            Return New ThemeCatalogItem("Shiny Red", "thumbShinyRed.png", "System.Windows.Controls.Samples;component/Theming/ThemeBrowser/ShinyRed.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents RainierOrange Theme.
    ''' </summary>
    Public Shared ReadOnly Property RainierOrange() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 7, 15, 19))
            Return New ThemeCatalogItem("Rainier Orange", "thumbRainierOrange.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/RainierOrange.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents RainierPurple Theme.
    ''' </summary>
    Public Shared ReadOnly Property RainierPurple() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 7, 15, 19))
            Return New ThemeCatalogItem("Rainier Purple", "thumbRainierPurple.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/RainierPurple.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents ExpressionDark.
    ''' </summary>
    Public Shared ReadOnly Property ExpressionDark() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 23, 23, 23), Color.FromArgb(255, 10, 10, 10))
            Return New ThemeCatalogItem("Expression Dark", "thumbExpresssionDark.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/ExpressionDark.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents ExpressionDark.
    ''' </summary>
    Public Shared ReadOnly Property ExpressionLight() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 189, 189, 189), Color.FromArgb(255, 160, 160, 160))
            Return New ThemeCatalogItem("Expression Light", "thumbExpresssionLight.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/ExpressionLight.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents Bureau Blue.
    ''' </summary>
    Public Shared ReadOnly Property BureauBlue() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 191, 219, 255), Color.FromArgb(255, 166, 194, 229))
            Return New ThemeCatalogItem("Bureau Blue", "BureauBlue.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/BureauBlue.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents Bureau Black.
    ''' </summary>
    Public Shared ReadOnly Property BureauBlack() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 91, 91, 91), Color.FromArgb(255, 13, 13, 13))
            Return New ThemeCatalogItem("Bureau Black", "BureauBlack.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/BureauBlack.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents Bureau Black.
    ''' </summary>
    Public Shared ReadOnly Property WhistlerBlue() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 255, 255, 255))
            Return New ThemeCatalogItem("Whistler Blue", "WhistlerBlue.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/WhistlerBlue.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents Twilight Blue.
    ''' </summary>
    Public Shared ReadOnly Property TwilightBlue() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 120, 183, 225), Color.FromArgb(255, 120, 183, 225))
            Return New ThemeCatalogItem("Twilight Blue", "TwilightBlue.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/TwilightBlue.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a Theme object that represents Bubble Creme.
    ''' </summary>
    <SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId:="Creme", Justification:="Creme is an alternate spelling.")> _
    Public Shared ReadOnly Property BubbleCreme() As ThemeCatalogItem
        Get
            Dim brush As LinearGradientBrush = GetLinearBrush(Color.FromArgb(255, 249, 249, 230), Color.FromArgb(255, 249, 249, 230))
            Return New ThemeCatalogItem("Bubble Creme", "BubbleCreme.png", "System.Windows.Controls.Samples;component/theming/ThemeBrowser/BubbleCreme.xaml", brush)
        End Get
    End Property

    ''' <summary>
    ''' Gets a collection of theme objects.
    ''' </summary>
    Public Shared ReadOnly Property ThemesCatalog() As IEnumerable(Of ThemeCatalogItem)
        Get
            Return New ThemeCatalogItem() {BubbleCreme, TwilightBlue, ExpressionDark, ExpressionLight, WhistlerBlue, BureauBlack, BureauBlue, ShinyBlue, ShinyRed, RainierOrange, RainierPurple}
        End Get
    End Property

    ''' <summary>
    ''' Create a Theme background LinearGradientBrush.
    ''' </summary>
    ''' <param name="startColor">
    ''' Represents the start color of the gradient.
    ''' </param>
    ''' <param name="endColor">
    ''' Represents the end color of the gradient.
    ''' </param>
    ''' <returns>Returns a linear gradient brush.</returns>
    Private Shared Function GetLinearBrush(ByVal startColor As Color, ByVal endColor As Color) As LinearGradientBrush
        Dim brush As New LinearGradientBrush()
        Dim colorStop1 As New GradientStop()
        colorStop1.Color = startColor
        Dim colorStop2 As New GradientStop()
        colorStop2.Color = endColor
        colorStop2.Offset = 1
        brush.StartPoint = New System.Windows.Point(0.5, 1)
        brush.EndPoint = New System.Windows.Point(0.5, 0)
        brush.GradientStops = New GradientStopCollection()
        brush.GradientStops.Add(colorStop1)
        brush.GradientStops.Add(colorStop2)
        Return brush
    End Function
#End Region ' Sample Data
End Class
