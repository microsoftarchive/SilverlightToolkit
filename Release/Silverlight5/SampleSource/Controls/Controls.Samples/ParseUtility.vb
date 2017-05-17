' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Reflection
Imports TypeConverterAttribute = System.ComponentModel.TypeConverterAttribute

''' <summary>
''' Utiilty class contains common helper functions.
''' </summary>
Friend NotInheritable Class ParseUtility

    ''' <summary>
    ''' Initializes a new instance of the ParseUtility class.
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Try to read a value of type T from passed in TextBox. 
    ''' If succeeded, return the value; 
    ''' If failed, return passed in default value, and highlighted the text in red.
    ''' </summary>
    ''' <typeparam name="T">Value type.</typeparam>
    ''' <param name="tb">The TextBox whose Text value is to be parsed.</param>
    ''' <param name="defaultValue">Default value to return in case of failure.</param>
    ''' <returns>Value parsed from TextBox.Text or the passed in default value.</returns>
    <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="No other common base class for all those exceptions.")> _
    Public Shared Function ReadValue(Of T)(ByVal tb As TextBox, ByVal defaultValue As T) As T
        Dim value As T = defaultValue
        Dim success As Boolean = False

        Try
            Dim myT As Type = GetType(T)

            ' first, try Convert.
            Try
                value = CType(Convert.ChangeType(tb.Text, GetType(T), CultureInfo.CurrentCulture), T)
                success = True
            Catch e1 As InvalidCastException
            End Try

            ' second, try TypeConverter.
            If (Not success) Then
                Dim tcas As IEnumerable(Of TypeConverterAttribute)
                Dim tca As TypeConverterAttribute
                Dim tct As Type
                Dim tc As TypeConverter

                tcas = myT.GetCustomAttributes(GetType(TypeConverterAttribute), True).Cast(Of TypeConverterAttribute)()
                tca = tcas.FirstOrDefault()
                tct = Type.GetType(tca.ConverterTypeName)
                tc = TryCast(Activator.CreateInstance(tct), TypeConverter)
                If tcas IsNot Nothing AndAlso tca IsNot Nothing AndAlso tct IsNot Nothing AndAlso tc IsNot Nothing AndAlso tc.CanConvertFrom(GetType(String)) Then
                    value = CType(tc.ConvertFromString(tb.Text), T)
                    success = True
                End If
            End If

            If (Not success) Then
                ' last, try Parse method.
                Dim mi As MethodInfo = myT.GetMethod("Parse", New Type() {GetType(String)})
                If mi IsNot Nothing Then
                    value = CType(mi.Invoke(Nothing, New Object() {tb.Text}), T)
                    success = True
                End If
            End If
        Catch e2 As Exception
            ' don't want to throw any exception.
        End Try

        If success Then
            tb.Foreground = New SolidColorBrush(Colors.Black)
            Return value
        Else
            tb.Foreground = New SolidColorBrush(Colors.Red)
            Return defaultValue
        End If
    End Function
End Class