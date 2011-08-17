' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Windows

Namespace Samples

    Partial Public Class TransitionsSample
        Inherits PhoneApplicationPage
        Public Sub New()
            DataContext = Me
            InitializeComponent()
        End Sub

        Public ReadOnly Property Families As IList(Of String)
            Get
                Return New List(Of String) From {"Roll",
                                                 "Rotate",
                                                 "Slide",
                                                 "Swivel",
                                                 "Turnstile"}
            End Get
        End Property

        Private Function RotateTransitionElement(ByVal mode As String) As RotateTransition
            Dim rotateTransitionMode =
                CType(System.Enum.Parse(GetType(RotateTransitionMode), mode, False), RotateTransitionMode)
            Return New RotateTransition With {.Mode = rotateTransitionMode}
        End Function

        Private Function SlideTransitionElement(ByVal mode As String) As SlideTransition
            Dim slideTransitionMode =
                CType(System.Enum.Parse(GetType(SlideTransitionMode), mode, False), SlideTransitionMode)
            Return New SlideTransition With {.Mode = slideTransitionMode}
        End Function

        Private Function SwivelTransitionElement(ByVal mode As String) As SwivelTransition
            Dim swivelTransitionMode =
                CType(System.Enum.Parse(GetType(SwivelTransitionMode), mode, False), SwivelTransitionMode)
            Return New SwivelTransition With {.Mode = swivelTransitionMode}
        End Function

        Private Function TurnstileTransitionElement(ByVal mode As String) As TurnstileTransition
            Dim turnstileTransitionMode =
                CType(System.Enum.Parse(GetType(TurnstileTransitionMode), mode, False), TurnstileTransitionMode)
            Return New TurnstileTransition With {.Mode = turnstileTransitionMode}
        End Function

        Private Function TransitionElement(ByVal family As String, ByVal mode As String) As TransitionElement
            Select Case family
                Case "Rotate"
                    Return RotateTransitionElement(mode)
                Case "Slide"
                    Return SlideTransitionElement(mode)
                Case "Swivel"
                    Return SwivelTransitionElement(mode)
                Case "Turnstile"
                    Return TurnstileTransitionElement(mode)
            End Select
            Return Nothing
        End Function

        Private Sub See(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim _family = CStr(Family.SelectedItem)
            Dim _mode = CStr(Mode.SelectedItem)
            Dim transitionElement As TransitionElement = Nothing
            If _family.Equals("Roll") Then
                transitionElement = New RollTransition
            Else
                transitionElement = Me.TransitionElement(_family, _mode)
            End If
            Dim phoneApplicationPage = CType(((CType(Application.Current.RootVisual, PhoneApplicationFrame))).Content, PhoneApplicationPage)
            Dim transition = transitionElement.GetTransition(phoneApplicationPage)
            AddHandler transition.Completed, Sub() transition.Stop()
            transition.Begin()
        End Sub

        Private Sub Forward(ByVal sender As Object, ByVal e As RoutedEventArgs)
            NavigationService.Navigate(New Uri("/Samples/NavigationTransitionSample1.xaml", UriKind.Relative))
        End Sub

        Private Sub FamilySelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
            Dim _family = CStr(Family.SelectedItem)
            Mode.Visibility = If(_family.Equals("Roll"), System.Windows.Visibility.Collapsed, System.Windows.Visibility.Visible)
        End Sub
    End Class


    Public Class EnumConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As Type,
                                ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            Dim s = TryCast(value, String)
            If s Is Nothing Then
                Return Nothing
            End If
            Select Case s
                Case "Roll"
                    Return New List(Of String)
                Case "Rotate"
                    Return New List(Of String) From {"In90Clockwise",
                                                     "In90Counterclockwise",
                                                     "In180Clockwise",
                                                     "In180Counterclockwise",
                                                     "Out90Clockwise",
                                                     "Out90Counterclockwise",
                                                     "Out180Clockwise",
                                                     "Out180Counterclockwise"}
                Case "Slide"
                    Return New List(Of String) From {"SlideUpFadeIn",
                                                     "SlideUpFadeOut",
                                                     "SlideDownFadeIn",
                                                     "SlideDownFadeOut",
                                                     "SlideLeftFadeIn",
                                                     "SlideLeftFadeOut",
                                                     "SlideRightFadeIn",
                                                     "SlideRightFadeOut"}
                Case "Swivel"
                    Return New List(Of String) From {"FullScreenIn",
                                                     "FullScreenOut",
                                                     "ForwardIn",
                                                     "ForwardOut",
                                                     "BackwardIn",
                                                     "BackwardOut"}
                Case "Turnstile"
                    Return New List(Of String) From {"ForwardIn",
                                                     "ForwardOut",
                                                     "BackwardIn",
                                                     "BackwardOut"}
            End Select
            Return Nothing
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object,
                                    ByVal culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function

    End Class

End Namespace
