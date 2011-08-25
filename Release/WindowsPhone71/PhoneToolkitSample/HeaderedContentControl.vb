' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.
Namespace LocalControls

    Public Class HeaderedContentControl
        Inherits ContentControl
        ' Methods
        Public Sub New()
            MyBase.DefaultStyleKey = GetType(HeaderedContentControl)
        End Sub

        Protected Overridable Sub OnHeaderChanged(ByVal oldHeader As Object, ByVal newHeader As Object)
        End Sub

        Private Shared Sub OnHeaderPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            DirectCast(d, HeaderedContentControl).OnHeaderChanged(e.OldValue, e.NewValue)
        End Sub

        Protected Overridable Sub OnHeaderTemplateChanged(ByVal oldHeaderTemplate As DataTemplate, ByVal newHeaderTemplate As DataTemplate)
        End Sub

        Private Shared Sub OnHeaderTemplatePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            DirectCast(d, HeaderedContentControl).OnHeaderTemplateChanged(DirectCast(e.OldValue, DataTemplate), DirectCast(e.NewValue, DataTemplate))
        End Sub

        ' Properties
        Public Property Header As Object
            Get
                Return MyBase.GetValue(HeaderedContentControl.HeaderProperty)
            End Get
            Set(ByVal value As Object)
                MyBase.SetValue(HeaderedContentControl.HeaderProperty, value)
            End Set
        End Property

        Public Property HeaderTemplate As DataTemplate
            Get
                Return DirectCast(MyBase.GetValue(HeaderedContentControl.HeaderTemplateProperty), DataTemplate)
            End Get
            Set(ByVal value As DataTemplate)
                MyBase.SetValue(HeaderedContentControl.HeaderTemplateProperty, value)
            End Set
        End Property


        ' Fields
        Public Shared ReadOnly HeaderProperty As DependencyProperty = DependencyProperty.Register("Header", GetType(Object), GetType(HeaderedContentControl), New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnHeaderPropertyChanged)))
        Public Shared ReadOnly HeaderTemplateProperty As DependencyProperty = DependencyProperty.Register("HeaderTemplate", GetType(DataTemplate), GetType(HeaderedContentControl), New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnHeaderTemplatePropertyChanged)))
    End Class

End Namespace
