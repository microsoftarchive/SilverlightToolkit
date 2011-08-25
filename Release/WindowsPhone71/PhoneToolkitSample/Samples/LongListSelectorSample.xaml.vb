' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports PhoneToolkitSample.Data

Namespace Samples

    Partial Public Class LongListSelectorSample
        Inherits PhoneApplicationPage

        Private currentSelector As LongListSelector

        Public Sub New()
            InitializeComponent()
            LoadLinqMovies()
            AddHandler linqMovies.SelectionChanged, AddressOf MovieSelectionChanged
            AddHandler codeMovies.SelectionChanged, AddressOf MovieSelectionChanged
        End Sub


        Private Sub LoadLinqMovies()
            Dim movies As New List(Of Movie)

            For i = 0 To 49
                movies.Add(Movie.CreateRandom())
            Next i

            Dim g = movies.GroupBy(Function(p) p.Category).OrderBy(Function(p) p.Key)
            Dim moviesByCategory = From c In g
                  Select New PublicGrouping(Of String, Movie)(c)

            linqMovies.ItemsSource = moviesByCategory
        End Sub

        Private Sub MovieSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)

        End Sub

        Private Sub PersonSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) Handles buddies.SelectionChanged
            Dim _person = TryCast(buddies.SelectedItem, Person)
            If _person IsNot Nothing Then
                NavigationService.Navigate(New Uri("/Samples/PersonDetail.xaml?ID=" & _person.ID, UriKind.Relative))
            End If
        End Sub

        Private Sub LongListSelector_GroupViewOpened(sender As System.Object, e As Microsoft.Phone.Controls.GroupViewOpenedEventArgs)
            currentSelector = CType(sender, LongListSelector)

            Dispatcher.BeginInvoke(
            Sub()
                Dim quadraticEase As IEasingFunction = New QuadraticEase With {.EasingMode = EasingMode.EaseOut}
                Dim _swivelShow As Storyboard = New Storyboard()
                Dim groupItems As ItemsControl = e.ItemsControl

                For Each item In groupItems.Items
                    Dim container As UIElement = CType(groupItems.ItemContainerGenerator.ContainerFromItem(item), UIElement)
                    If container IsNot Nothing Then
                        Dim content As Border = CType(VisualTreeHelper.GetChild(container, 0), Border)
                        If content IsNot Nothing Then
                            Dim showAnimation As DoubleAnimationUsingKeyFrames = New DoubleAnimationUsingKeyFrames()

                            Dim showKeyFrame1 As EasingDoubleKeyFrame = New EasingDoubleKeyFrame() With {.KeyTime = TimeSpan.FromMilliseconds(0), .Value = -60, .EasingFunction = quadraticEase}
                            Dim showKeyFrame2 As EasingDoubleKeyFrame = New EasingDoubleKeyFrame() With {.KeyTime = TimeSpan.FromMilliseconds(85), .Value = 0, .EasingFunction = quadraticEase}

                            showAnimation.KeyFrames.Add(showKeyFrame1)
                            showAnimation.KeyFrames.Add(showKeyFrame2)

                            Storyboard.SetTargetProperty(showAnimation, New PropertyPath(PlaneProjection.RotationXProperty))
                            Storyboard.SetTarget(showAnimation, content.Projection)

                            _swivelShow.Children.Add(showAnimation)
                        End If
                    End If
                Next
                _swivelShow.Begin()
            End Sub)
        End Sub

        Private Sub LongListSelector_GroupViewClosing(sender As System.Object, e As Microsoft.Phone.Controls.GroupViewClosingEventArgs)
            e.Cancel = True
            If e.SelectedGroup IsNot Nothing Then
                currentSelector.ScrollToGroup(e.SelectedGroup)
            End If

            Dispatcher.BeginInvoke(
            Sub()
                Dim quadraticEase As IEasingFunction = New QuadraticEase With {.EasingMode = EasingMode.EaseOut}
                Dim _swivelHide As Storyboard = New Storyboard()
                Dim groupItems As ItemsControl = e.ItemsControl

                For Each item In groupItems.Items
                    Dim container As UIElement = CType(groupItems.ItemContainerGenerator.ContainerFromItem(item), UIElement)
                    If container IsNot Nothing Then
                        Dim content As Border = CType(VisualTreeHelper.GetChild(container, 0), Border)
                        If content IsNot Nothing Then
                            Dim showAnimation As DoubleAnimationUsingKeyFrames = New DoubleAnimationUsingKeyFrames()

                            Dim showKeyFrame1 As EasingDoubleKeyFrame = New EasingDoubleKeyFrame() With {.KeyTime = TimeSpan.FromMilliseconds(0), .Value = 0, .EasingFunction = quadraticEase}
                            Dim showKeyFrame2 As EasingDoubleKeyFrame = New EasingDoubleKeyFrame() With {.KeyTime = TimeSpan.FromMilliseconds(125), .Value = 90, .EasingFunction = quadraticEase}

                            showAnimation.KeyFrames.Add(showKeyFrame1)
                            showAnimation.KeyFrames.Add(showKeyFrame2)

                            Storyboard.SetTargetProperty(showAnimation, New PropertyPath(PlaneProjection.RotationXProperty))
                            Storyboard.SetTarget(showAnimation, content.Projection)

                            _swivelHide.Children.Add(showAnimation)
                        End If
                    End If
                Next

                AddHandler _swivelHide.Completed, AddressOf _swivelHide_Completed
                _swivelHide.Begin()
            End Sub)
        End Sub

        Private Sub _swivelHide_Completed(sender As Object, e As EventArgs)
            If currentSelector IsNot Nothing Then
                currentSelector.CloseGroupView()
                currentSelector = Nothing
            End If
        End Sub
    End Class

End Namespace
