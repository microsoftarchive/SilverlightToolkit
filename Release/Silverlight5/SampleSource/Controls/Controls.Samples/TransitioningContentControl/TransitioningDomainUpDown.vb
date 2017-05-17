' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System

''' <summary>
''' Implementation of DomainUpDown that uses a TransitioningContentControl
''' it its template.
''' </summary>
''' <QualityBand>Experimental</QualityBand>
''' <remarks>Implemented in the sample project. The sample page will set
''' the custom template that this control expects.</remarks>
Public Class TransitioningDomainUpDown
    Inherits DomainUpDown
    ''' <summary>
    ''' The name of the state that represents a transition effect upwards.
    ''' </summary>
    Public Const UpTransitionState As String = "UpTransition"

    ''' <summary>
    ''' The name of the state that represents a transition effect downwards.
    ''' </summary>
    Public Const DownTransitionState As String = "DownTransition"

    ''' <summary>
    ''' Gets or sets the transition element.
    ''' </summary>
    ''' <value>The transition element.</value>
    Private privateTransitionElement As TransitioningContentControl
    Private Property TransitionElement() As TransitioningContentControl
        Get
            Return privateTransitionElement
        End Get
        Set(ByVal value As TransitioningContentControl)
            privateTransitionElement = value
        End Set
    End Property

    ''' <summary>
    ''' Builds the visual tree for the DomainUpDown control when a new
    ''' template is applied.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()

        TransitionElement = TryCast(GetTemplateChild("Visualization"), TransitioningContentControl)
    End Sub

    ''' <summary>
    ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
    ''' Will not go beyond the first or last item unless IsCyclic is set.
    ''' </summary>
    Protected Overrides Sub OnIncrement()
        If TransitionElement IsNot Nothing Then
            TransitionElement.Transition = UpTransitionState
        End If
        MyBase.OnIncrement()
    End Sub

    ''' <summary>
    ''' Called by OnSpin when the spin direction is SpinDirection.Increase.
    ''' Will not go beyond the first or last item unless IsCyclic is set.
    ''' </summary>
    Protected Overrides Sub OnDecrement()
        If TransitionElement IsNot Nothing Then
            TransitionElement.Transition = DownTransitionState
        End If
        MyBase.OnDecrement()
    End Sub
End Class