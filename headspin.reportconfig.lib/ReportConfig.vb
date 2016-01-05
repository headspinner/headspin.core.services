Imports HeadSpin.App.Core
Imports HeadSpin.App
Imports HeadSpin.App.Core.Database


Public Class TblReportConfig
    Inherits TblBase

    Public Enum COLS
        ReportConfigId = 0
        StatusCode
        TypeCode
        ConfigInfo
        Name
        AddedBy
        EditedBy
        SIZE
    End Enum
End Class

Public Class ReportConfig
    Inherits BusinessObject
    Public Sub New()
    End Sub

#Region "members"


    Public Property ReportConfigId() As Integer
        Get
            Return Me.ObjectId
        End Get
        Set(ByVal value As Integer)
            Me.ObjectId = value
        End Set
    End Property

    Private _StatusCode As String

    Public Property StatusCode() As String
        Get
            Return _StatusCode
        End Get
        Set(value As String)
            _StatusCode = Me.CheckDirty(value, _StatusCode)
        End Set
    End Property

    Private _TypeCode As String

    Public Property TypeCode() As String
        Get
            Return _TypeCode
        End Get
        Set(value As String)
            _TypeCode = Me.CheckDirty(value, _TypeCode)
        End Set
    End Property

    Private _ConfigInfo As String

    Public Property ConfigInfo() As String
        Get
            Return _ConfigInfo
        End Get
        Set(value As String)
            _ConfigInfo = Me.CheckDirty(value, _ConfigInfo)
        End Set
    End Property

    Private _Name As String

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = Me.CheckDirty(value, _Name)
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

    Public Shared Function GetById(ByVal id As Integer) As ReportConfig
        If id <= 0 Then
            Throw New ArgumentException("Unable to load ReportConfig, id argument was zero or less than zero")
        End If

        Dim crit As New Criteria
        crit.Add(Of Integer)(TblReportConfig.COLS.ReportConfigId.ToString, id)
        Return ReportConfig.GetOneObject(Of ReportConfig)(New ReportConfig(), crit)
    End Function


#Region "getList"
    Public Shared Function GetList(ByVal crit As Criteria) As BusinessObjectCollection(Of ReportConfig)
        Return ReportConfig.GetObjectList(Of ReportConfig)(New ReportConfig(), crit)
    End Function

    Protected Overrides Function NewForListLoad() As BusinessObject
        Return New ReportConfig()
    End Function

    Protected Overrides Function GetListCmdImpl(ByVal db As DB, ByVal crit As Criteria) As DBCmd
        Dim cmd As DBCmd = db.NewCommand("GetReportConfig")

        If crit IsNot Nothing Then
            'Support get by id always!
            Dim id As Integer = crit.Get(Of Integer)(TblReportConfig.COLS.ReportConfigId.ToString(), 0)

            If (id > 0) Then
                cmd.AddParam(TblReportConfig.COLS.ReportConfigId.ToString(), id)
            End If

        End If

        Return cmd
    End Function
#End Region

    Protected Overrides Sub LoadBO(ByVal dbReader As DBReader)

        Me.ReportConfigId = dbReader.GetInteger(TblReportConfig.COLS.ReportConfigId.ToString())

        Me._StatusCode = dbReader.GetString(TblReportConfig.COLS.StatusCode.ToString())

        Me._TypeCode = dbReader.GetString(TblReportConfig.COLS.TypeCode.ToString())

        Me._ConfigInfo = dbReader.GetString(TblReportConfig.COLS.ConfigInfo.ToString())

        Me._Name = dbReader.GetString(TblReportConfig.COLS.Name.ToString())

        Me._AddedBy = dbReader.GetString(TblReportConfig.COLS.AddedBy.ToString())

        Me._EditedBy = dbReader.GetString(TblReportConfig.COLS.EditedBy.ToString())

        LoadBaseProperties(dbReader)
    End Sub

    Protected Overrides Sub SaveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("SaveReportConfig")
        cmd.AddParam(TblReportConfig.COLS.ReportConfigId.ToString(), Me.ReportConfigId)
        cmd.AddParam(TblReportConfig.COLS.StatusCode.ToString(), Me.StatusCode)
        cmd.AddParam(TblReportConfig.COLS.TypeCode.ToString(), Me.TypeCode)
        cmd.AddParam(TblReportConfig.COLS.ConfigInfo.ToString(), Me.ConfigInfo)
        cmd.AddParam(TblReportConfig.COLS.Name.ToString(), Me.Name)
        cmd.AddParam(TblReportConfig.COLS.AddedBy.ToString(), Me.AddedBy)
        cmd.AddParam(TblReportConfig.COLS.EditedBy.ToString(), Me.EditedBy)

        SetBaseProperties(cmd)
        ProcessSaveResults(db.Execute(cmd))
    End Sub

    Public Overrides Sub Validate()
        ' @@@ Add validation
    End Sub


    Protected Overrides Sub RemoveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("DeleteReportConfig")
        cmd.AddParam(TblReportConfig.COLS.ReportConfigId.ToString(), Me.ReportConfigId)
        cmd.AddParam(TblBase.BaseCOLS.LockId.ToString(), Me.LockId)

        ProcessDeleteResults(db.Execute(cmd))
    End Sub
#End Region

End Class
