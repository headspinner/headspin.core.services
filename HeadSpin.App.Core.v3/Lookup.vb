

Imports System.ComponentModel
Imports System.Linq
Imports HeadSpin.App.Core.Database




Public Class Lookup
    Inherits BusinessObject

    Friend Sub New()
    End Sub

#Region "members"


    Public Property LookupId() As Integer
        Get
            Return Me.ObjectId
        End Get
        Set(ByVal value As Integer)
            Me.ObjectId = value
        End Set
    End Property

    Private _LookupType As String

    Public Property LookupType() As String
        Get
            Return _LookupType
        End Get
        Set(ByVal value As String)
            _LookupType = Me.CheckDirty(value, _LookupType)
        End Set
    End Property


    Private _ParentLookupCode As String

    Public Property ParentLookupCode() As String
        Get
            Return _ParentLookupCode
        End Get
        Set(ByVal value As String)
            _ParentLookupCode = Me.CheckDirty(value, _ParentLookupCode)
        End Set
    End Property

    Private _ParentLookupType As String

    Public Property ParentLookupType() As String
        Get
            Return _ParentLookupType
        End Get
        Set(ByVal value As String)
            _ParentLookupType = Me.CheckDirty(value, _ParentLookupType)
        End Set
    End Property
    Private _LookupCode As String

    Public Property LookupCode() As String
        Get
            Return _LookupCode
        End Get
        Set(ByVal value As String)
            _LookupCode = Me.CheckDirty(value, _LookupCode)
        End Set
    End Property

    Private _LookupGroup As String

    Public Property LookupGroup() As String
        Get
            Return _LookupGroup
        End Get
        Set(ByVal value As String)
            _LookupGroup = Me.CheckDirty(value, _LookupGroup)
        End Set
    End Property

    Private _LookupDescription As String

    Public Property LookupDescription() As String
        Get
            Return _LookupDescription
        End Get
        Set(ByVal value As String)
            _LookupDescription = Me.CheckDirty(value, _LookupDescription)
        End Set
    End Property

    Private _Active As Boolean

    Public Property Active() As Boolean
        Get
            Return _Active
        End Get
        Set(ByVal value As Boolean)
            _Active = Me.CheckDirty(value, _Active)
        End Set
    End Property


    Private _ParentLookupId As Integer? = Nothing

    Public Property ParentLookupId() As Integer?
        Get
            Return _ParentLookupId
        End Get
        Set(ByVal value As Integer?)
            _ParentLookupId = Me.CheckDirty(value, _ParentLookupId)
        End Set
    End Property


    Private _DisplayOrder As Integer

    Public Property DisplayOrder() As Integer
        Get
            Return _DisplayOrder
        End Get
        Set(ByVal value As Integer)
            _DisplayOrder = Me.CheckDirty(value, _DisplayOrder)
        End Set
    End Property

    Public Overrides Function GetFullyQualifiedClassName() As String
        Return Me.GetType.AssemblyQualifiedName.ToString()
    End Function


#End Region

#Region "load/save/val"

    Protected Overrides Sub LoadBO(ByVal dbReader As DBReader)

        Me.LookupId = dbReader.GetInteger(TblLookup.COLS.LookupId.ToString())
        Me.LookupType = dbReader.GetString(TblLookup.COLS.LookupType.ToString())
        Me.LookupCode = dbReader.GetString(TblLookup.COLS.LookupCode.ToString())
        Me.LookupDescription = dbReader.GetString(TblLookup.COLS.LookupDescription.ToString())
        Me.Active = dbReader.GetBoolean(TblLookup.COLS.Active.ToString())
        Me.ParentLookupId = dbReader.GetNullableInteger(TblLookup.COLS.ParentLookupId.ToString())
        Me.LookupGroup = dbReader.GetString(TblLookup.COLS.LookupGroup.ToString())
        Me.ParentLookupType = dbReader.GetString(TblLookup.COLS.ParentLookupType.ToString())
        Me.ParentLookupCode = dbReader.GetString(TblLookup.COLS.ParentLookupCode.ToString())
        Me.DisplayOrder = dbReader.GetInteger(TblLookup.COLS.DisplayOrder.ToString())

        LoadBaseProperties(dbReader)

    End Sub

    Protected Overrides Sub SaveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("SaveLookup")
        cmd.AddParam(TblLookup.COLS.LookupId.ToString(), Me.LookupId)
        cmd.AddParam(TblLookup.COLS.LookupType.ToString(), Me.LookupType)
        cmd.AddParam(TblLookup.COLS.LookupCode.ToString(), Me.LookupCode)
        cmd.AddParam(TblLookup.COLS.LookupDescription.ToString(), Me.LookupDescription)
        cmd.AddParam(TblLookup.COLS.Active.ToString(), Me.Active)
        cmd.AddParam(TblLookup.COLS.ParentLookupId.ToString(), Me.ParentLookupId)
        cmd.AddParam(TblLookup.COLS.LookupGroup.ToString(), Me.LookupGroup)
        cmd.AddParam(TblLookup.COLS.ParentLookupType.ToString(), Me.ParentLookupType)
        cmd.AddParam(TblLookup.COLS.ParentLookupCode.ToString(), Me.ParentLookupCode)
        cmd.AddParam(TblLookup.COLS.DisplayOrder.ToString(), Me.DisplayOrder)
      
        SetBaseProperties(cmd)

        ProcessSaveResults(db.Execute(cmd))
    End Sub

    Public Overrides Sub Validate()
        ' @@@ Add validation
    End Sub

    'Protected Overrides Sub LoadBO(ByVal nId As Integer)

    '    Using db As DB = DBMgr.Instance.GetDB()

    '        Dim cmd As DBCmd = db.NewCommand("GetLookup")
    '        cmd.AddParam(TblLookup.COLS.LookupId.ToString(), nId)

    '        Dim reader As DBReader = db.Execute(cmd)

    '        If reader.Read() Then
    '            Me.LoadBO(reader)
    '        End If
    '    End Using

    'End Sub

    Protected Overrides Sub RemoveBO(ByVal db As DB)
        '@@@ tbd
    End Sub


#End Region




    Protected Overrides Function GetListCmdImpl(ByVal db As Database.DB, ByVal crit As Criteria) As Database.DBCmd
        Return Nothing
    End Function
End Class
