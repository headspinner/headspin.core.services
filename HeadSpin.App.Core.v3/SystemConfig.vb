

Imports System
Imports System.ComponentModel
Imports System.Linq
Imports HeadSpin.App.Core
Imports HeadSpin.App.Core.Database
Public Class TblSystemConfig
    Inherits TblBase

    Public Enum COLS
        SystemConfigId = 0
        Key
        Value
        AddedBy
        EditedBy
        SIZE
    End Enum
End Class


Public Class SystemConfig
    Inherits BusinessObject
    Friend Sub New()
    End Sub

#Region "members"


    Public Property SystemConfigId() As Integer
        Get
            Return Me.ObjectId
        End Get
        Set(ByVal value As Integer)
            Me.ObjectId = value
        End Set
    End Property

    Private _Key As String

    Public Property Key() As String
        Get
            Return _Key
        End Get
        Set(value As String)
            _Key = Me.CheckDirty(value, _Key)
        End Set
    End Property

    Private _Value As String

    Public Property Value() As String
        Get
            Return _Value
        End Get
        Set(value As String)
            _Value = Me.CheckDirty(value, _Value)
        End Set
    End Property



    Private _AddedBy As String

    Public Property AddedBy() As String
        Get
            Return _AddedBy
        End Get
        Set(value As String)
            _AddedBy = Me.CheckDirty(value, _AddedBy)
        End Set
    End Property


    Private _EditedBy As String

    Public Property EditedBy() As String
        Get
            Return _EditedBy
        End Get
        Set(value As String)
            _EditedBy = Me.CheckDirty(value, _EditedBy)
        End Set
    End Property

#End Region

#Region "load/save/val/getList"

    Public Shared Function GetById(ByVal id As Integer) As SystemConfig
        If id <= 0 Then
            Throw New ArgumentException("Unable to load SystemConfig, id argument was zero or less than zero")
        End If

        Dim crit As New Criteria
        crit.Add(Of Integer)(TblSystemConfig.COLS.SystemConfigId.ToString, id)
        Return SystemConfig.GetOneObject(Of SystemConfig)(New SystemConfig(), crit)
    End Function

    Public Shared Function GetByKey(ByVal key As String) As SystemConfig
        If String.IsNullOrEmpty(key) Then
            Throw New ArgumentException("Unable to load SystemConfig, key argument was not set")
        End If

        Dim crit As New Criteria
        crit.Add(Of String)(TblSystemConfig.COLS.Key.ToString, key)
        Return SystemConfig.GetOneObject(Of SystemConfig)(New SystemConfig(), crit)
    End Function


#Region "getList"
    Public Shared Function GetList(ByVal crit As Criteria) As BusinessObjectCollection(Of SystemConfig)
        Return SystemConfig.GetObjectList(Of SystemConfig)(New SystemConfig(), crit)
    End Function

    Protected Overrides Function NewForListLoad() As BusinessObject
        Return New SystemConfig()
    End Function

    Protected Overrides Function GetListCmdImpl(ByVal db As DB, ByVal crit As Criteria) As DBCmd
        Dim cmd As DBCmd = db.NewCommand("GetSystemConfig")

        If crit IsNot Nothing Then
            'Support get by id always!
            Dim id As Integer = crit.Get(Of Integer)(TblSystemConfig.COLS.SystemConfigId.ToString(), 0)
            Dim key As String = crit.Get(Of String)(TblSystemConfig.COLS.Key.ToString(), String.Empty)

            If (id > 0) Then
                cmd.AddParam(TblSystemConfig.COLS.SystemConfigId.ToString(), id)
            End If

            If Not String.IsNullOrEmpty(key) Then
                cmd.AddParam(TblSystemConfig.COLS.Key.ToString(), key)
            End If

        End If

        Return cmd
    End Function
#End Region

    Protected Overrides Sub LoadBO(ByVal dbReader As DBReader)

        Me.SystemConfigId = dbReader.GetInteger(TblSystemConfig.COLS.SystemConfigId.ToString())

        Me._Key = dbReader.GetString(TblSystemConfig.COLS.Key.ToString())

        Me._Value = dbReader.GetString(TblSystemConfig.COLS.Value.ToString())

        'Me._AddedBy = dbReader.GetString(TblSystemConfig.COLS.AddedBy.ToString())

        'Me._EditedBy = dbReader.GetString(TblSystemConfig.COLS.EditedBy.ToString())

        LoadBaseProperties(dbReader)
    End Sub

    Protected Overrides Sub SaveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("SaveSystemConfig")
        cmd.AddParam(TblSystemConfig.COLS.SystemConfigId.ToString(), Me.SystemConfigId)
        cmd.AddParam(TblSystemConfig.COLS.Key.ToString(), Me.Key)
        cmd.AddParam(TblSystemConfig.COLS.Value.ToString(), Me.Value)
        'cmd.AddParam(TblSystemConfig.COLS.AddedBy.ToString(), Me.AddedBy)
        'cmd.AddParam(TblSystemConfig.COLS.EditedBy.ToString(), Me.EditedBy)

        SetBaseProperties(cmd)
        ProcessSaveResults(db.Execute(cmd))
    End Sub

    Public Overrides Sub Validate()
        ' @@@ Add validation
    End Sub


    Protected Overrides Sub RemoveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("DeleteSystemConfig")
        cmd.AddParam(TblSystemConfig.COLS.SystemConfigId.ToString(), Me.SystemConfigId)
        cmd.AddParam(TblBase.BaseCOLS.LockId.ToString(), Me.LockId)

        ProcessDeleteResults(db.Execute(cmd))
    End Sub
#End Region

End Class


