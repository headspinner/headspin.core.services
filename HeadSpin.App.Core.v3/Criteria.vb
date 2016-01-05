Imports System.Collections.Generic
Imports System.Runtime.Serialization

 

Class CritValHolderBase
    Friend Overridable Function GetObject() As Object
        Return Nothing
    End Function
End Class

' This class holds a single strongly-typed criteria item value.

Class CritValHolder(Of ValType)
    Inherits CritValHolderBase
    Friend Sub New(ByVal value As ValType)
        m_HeldValue = value
    End Sub

    Friend ReadOnly Property Value() As ValType
        Get
            Return m_HeldValue
        End Get
    End Property

    Friend Overrides Function GetObject() As Object
        Return m_HeldValue
    End Function


    Private m_HeldValue As ValType = Nothing
End Class

' This class holds multiple strongly-typed criteria item values (for the same
' criteria item).

Class CritValListHolder(Of ValType)
    Inherits CritValHolderBase
    Friend Sub New(ByVal valList As List(Of ValType))
        m_HeldValueList = valList
    End Sub

    Friend ReadOnly Property Value() As List(Of ValType)
        Get
            Return m_HeldValueList
        End Get
    End Property


    Private m_HeldValueList As List(Of ValType) = Nothing
End Class

' This class represents a single named, strongly-typed Criteria item. The
' item may have a single value, or multiple values of the same type. This
' class itself is NOT generic, so it can be put into the Criteria's Dictionary,
' which is strongly-typed on this class, NOT the type of the actual value.
' I would have preferred to set the value(s) using a c'tor, and NOT have the
' "Set" members, but .NET (or at least C#) does not allow generic c'tors.

Public Class CritItem
    ' set a name/value criteria item, where a single value is associated
    ' with "name";
    Friend Sub [Set](Of ValType)(ByVal name As String, ByVal critVal As ValType)
        m_Name = name
        m_holder = New CritValHolder(Of ValType)(critVal)
    End Sub

    ' retrieve the single value in this item;
    Friend Function [Get](Of ItemType)() As ItemType
        If TypeOf m_holder Is CritValHolder(Of ItemType) Then
            Return TryCast(m_holder, CritValHolder(Of ItemType)).Value
        End If

        Dim msg As String = String.Format("held value is requested as '{0}', but is really '{1}'", GetType(ItemType), m_holder.[GetType]().GetGenericArguments()(0).Name)

        Throw New CriteriaException(msg)
    End Function

    ' set a named criteria item, where multiple values are associated
    ' with "name";
    Friend Sub [Set](Of ValueType)(ByVal name As String, ByVal valList As List(Of ValueType))
        m_Name = name
        m_holder = New CritValListHolder(Of ValueType)(valList)
    End Sub

    ' retrieve the multiple values in this item;
    Friend Function GetList(Of Y)(ByVal name As String) As List(Of Y)
        If TypeOf m_holder Is CritValListHolder(Of Y) Then
            Return TryCast(m_holder, CritValListHolder(Of Y)).Value
        End If

        Dim msg As String = String.Format("held value is requested as '{0}', but is really '{1}'", GetType(Y), m_holder.[GetType]().GetGenericArguments()(0).Name)

        Throw New CriteriaException(msg)
    End Function

    ' retrieve value as an object
    Friend Function GetObject() As Object
        Return m_holder.GetObject()
    End Function

    '''////////////////////////////////////////
    Friend ReadOnly Property Name() As String
        Get
            Return m_Name
        End Get
    End Property

    '''////////////////////////////////////////

    Private m_Name As String = String.Empty

    Private m_holder As CritValHolderBase = Nothing
End Class

' This is a design for a general-purpose Criteria class, meaning we would need
' ONLY this class for all our criteria purposes, and would NOT have to create
' or derive BO-specific criteria.
' A Criteria is basically a list of CritItems. A CritItem can hold either:
' a) a single strongly-typed value of any type (or at least any type we would
' use as a criteria), or b) a strongly-typed list of values (all of the same type).

Public Class Criteria
    Public ReadOnly Property ConnectionName() As String
        Get
            Return _ConnName
        End Get
    End Property

    Private _ConnName As String = Nothing
    Public Sub New(ByVal connectionName As String)
        _ConnName = connectionName
    End Sub

    Public Sub New()
    End Sub

    Public Sub New(ByVal id__1 As ULong)
        Id = id__1
    End Sub

    'public const string TrueValue = "Y";
    'public const string FalseValue = "N";

    'quick way to pass an Id as a criteria

    Public Id As ULong = 0

    ' add a crit item that has a single value;
    Public Sub Add(Of ValType)(ByVal name As String, ByVal value As ValType)
        Dim val As New CritItem()
        val.[Set](Of ValType)(name, value)
        m_Collection.Add(name, val)
    End Sub

    ''' <summary>
    ''' Returns true if the criteria is specified
    ''' </summary>
    ''' <param name="name">Criteria key</param>
    ''' <returns></returns>
    Public Function Exists(ByVal name As String) As Boolean
        Return m_Collection.ContainsKey(name)
    End Function

    Public Function [Get](ByVal name As String) As Object
        Dim value As CritItem = Nothing

        If m_Collection.TryGetValue(name, value) Then
            Return value.GetObject()
        End If

        Return Nothing
    End Function

    ' retrieve a crit item that has a single value;
    Public Function [Get](Of T)(ByVal name As String) As T
        Try
            ' remember that 'm_Collection' is a collection of CritValues!
            Dim item As T = m_Collection(name).[Get](Of T)()

            ' item is a T if we get here, or the above would have thrown;
            Return item
        Catch ex As CriteriaException
            Dim msg As String = String.Format("Criteria: type or other error retrieving crit item '{0}': {1}", name, ex.Message)
            Throw New CriteriaException(msg, ex)
        End Try
    End Function

    ''' <summary>
    '''   Retrieve a crit item that has a single value; if not found, return the
    '''   passed "defaultValue".
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="name"></param>
    ''' <param name="defaultValue"></param>
    ''' <returns></returns>
    Public Function [Get](Of T)(ByVal name As String, ByVal defaultValue As T) As T
        Dim value As CritItem = Nothing

        If m_Collection.TryGetValue(name, value) Then
            Return value.[Get](Of T)()
        End If

        Return defaultValue
    End Function

    ' add a crit item that has multiple values;
    Public Sub Add(Of ItemCollType)(ByVal name As String, ByVal list As List(Of ItemCollType))
        Dim val As New CritItem()
        val.Set(Of ItemCollType)(name, list)
        m_Collection.Add(name, val)
    End Sub

    ' retrieve a crit item that has multiple values;
    Public Function GetValues(Of CritItemType)(ByVal critItemName As String) As List(Of CritItemType)
        Try
            Dim itemList As List(Of CritItemType) = m_Collection(critItemName).GetList(Of CritItemType)(critItemName)

            Return itemList
        Catch ex As CriteriaException
            Dim msg As String = String.Format("Criteria: type or other error retrieving crit item " & "'{0}': {1}", critItemName, ex.Message)

            Throw New CriteriaException(msg, ex)
        End Try
    End Function

    '''////////////////////////////////////////

    Private m_Collection As New Dictionary(Of String, CritItem)()
End Class

' Exception class for use by the criteria mechanism above;
Public Class CriteriaException
    Inherits Exception
    Public Sub New()
    End Sub
    Public Sub New(ByVal msg As String)
        MyBase.New(msg)
    End Sub
    Public Sub New(ByVal msg As String, ByVal innerEx As System.Exception)
        MyBase.New(msg, innerEx)
    End Sub
    Public Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
    End Sub
End Class
