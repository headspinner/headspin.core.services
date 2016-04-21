Imports headspin.App.Core
Imports headspin.App.Core.Database

Public Class TblDocBlob
    Inherits TblBase

    Public Enum COLS
        DocBlobId = 0
        DocBlobFK
        TypeCode
        StatusCode
        AttachmentBytes
        AttachmentFileName
        AddedBy
        EditedBy
        Desc
        ClassName
        SIZE
    End Enum
End Class


Public Class DocBlob
    Inherits BusinessObject
    Friend Sub New()
    End Sub

#Region "members"


    Public Property DocBlobId() As Integer
        Get
            Return Me.ObjectId
        End Get
        Set(ByVal value As Integer)
            Me.ObjectId = value
        End Set
    End Property

    Private _DocBlobFK As Integer

    Public Property DocBlobFK() As Integer
        Get
            Return _DocBlobFK
        End Get
        Set(value As Integer)
            _DocBlobFK = Me.CheckDirty(value, _DocBlobFK)
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

    Private _StatusCode As String

    Public Property StatusCode() As String
        Get
            Return _StatusCode
        End Get
        Set(value As String)
            _StatusCode = Me.CheckDirty(value, _StatusCode)
        End Set
    End Property

    Private _AttachmentBytes As Byte()

    Public Property AttachmentBytes() As Byte()
        Get
            Return _AttachmentBytes
        End Get
        Set(value As Byte())
            _AttachmentBytes = Me.CheckDirty(value, _AttachmentBytes)
        End Set
    End Property

    Private _AttachmentFileName As String

    Public Property AttachmentFileName() As String
        Get
            Return _AttachmentFileName
        End Get
        Set(value As String)
            _AttachmentFileName = Me.CheckDirty(value, _AttachmentFileName)
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

    Private _Desc As String

    Public Property Desc() As String
        Get
            Return _Desc
        End Get
        Set(value As String)
            _Desc = Me.CheckDirty(value, _Desc)
        End Set
    End Property

    Private _ClassName As String

    Public Property ClassName() As String
        Get
            Return _ClassName
        End Get
        Set(value As String)
            _ClassName = Me.CheckDirty(value, _ClassName)
        End Set
    End Property

    Public Overrides Function GetFullyQualifiedClassName() As String
        Return String.Format("{0}.{1}", Me.GetType.Namespace, Me.GetType.Name.ToString())
    End Function

#End Region

#Region "load/save/val/getList"

    Public Shared Function GetById(ByVal id As Integer) As DocBlob
        If id <= 0 Then
            Throw New ArgumentException("Unable to load DocBlob, id argument was zero or less than zero")
        End If

        Dim crit As New Criteria
        crit.Add(Of Integer)(TblDocBlob.COLS.DocBlobId.ToString, id)
        Return DocBlob.GetOneObject(Of DocBlob)(New DocBlob(), crit)
    End Function

    Public Shared Function GetByObjIdAndClassName(ByVal fkId As Integer, ByVal className As String) As BusinessObjectCollection(Of DocBlob)
        If fkId <= 0 Then
            Throw New ArgumentException("Unable to load DocBlob, id argument was zero or less than zero")
        End If
        If String.IsNullOrWhiteSpace(className) Then
            Throw New ArgumentException("Unable to load DocBlob, classname argument was null.")
        End If

        Dim crit As New Criteria
        crit.Add(Of Integer)(TblDocBlob.COLS.DocBlobFK.ToString, fkId)
        crit.Add(Of String)(TblDocBlob.COLS.ClassName.ToString, className)
        Return DocBlob.GetObjectList(Of DocBlob)(New DocBlob(), crit)
    End Function


#Region "getList"
    Public Shared Function GetList(ByVal crit As Criteria) As BusinessObjectCollection(Of DocBlob)
        Return DocBlob.GetObjectList(Of DocBlob)(New DocBlob(), crit)
    End Function

    Protected Overrides Function NewForListLoad() As BusinessObject
        Return New DocBlob()
    End Function

    Protected Overrides Function GetListCmdImpl(ByVal db As DB, ByVal crit As Criteria) As DBCmd
        Dim cmd As DBCmd = db.NewCommand("GetDocBlob")

        If crit IsNot Nothing Then
            'Support get by id always!
            Dim id As Integer = crit.Get(Of Integer)(TblDocBlob.COLS.DocBlobId.ToString(), 0)
            Dim docBlobFK As Integer = crit.Get(Of Integer)(TblDocBlob.COLS.DocBlobFK.ToString(), 0)
            Dim className As String = crit.Get(Of String)(TblDocBlob.COLS.ClassName.ToString(), 0)

            If (id > 0) Then
                cmd.AddParam(TblDocBlob.COLS.DocBlobId.ToString(), id)
            End If

            If (docBlobFK > 0) Then
                cmd.AddParam(TblDocBlob.COLS.DocBlobFK.ToString(), docBlobFK)
            End If

            If String.IsNullOrWhiteSpace(className) = False Then
                cmd.AddParam(TblDocBlob.COLS.ClassName.ToString(), className)
            End If

        End If

        Return cmd
    End Function
#End Region

    Protected Overrides Sub LoadBO(ByVal dbReader As DBReader)

        Me.DocBlobId = dbReader.GetInteger(TblDocBlob.COLS.DocBlobId.ToString())

        Me._DocBlobFK = dbReader.GetInteger(TblDocBlob.COLS.DocBlobFK.ToString())

        Me._TypeCode = dbReader.GetString(TblDocBlob.COLS.TypeCode.ToString())

        Me._StatusCode = dbReader.GetString(TblDocBlob.COLS.StatusCode.ToString())

        Me._AttachmentBytes = dbReader.GetByteArray(TblDocBlob.COLS.AttachmentBytes.ToString())

        Me._AttachmentFileName = dbReader.GetString(TblDocBlob.COLS.AttachmentFileName.ToString())

        'Me._AddedBy = dbReader.GetString(TblDocBlob.COLS.AddedBy.ToString())

        'Me._EditedBy = dbReader.GetString(TblDocBlob.COLS.EditedBy.ToString())

        Me._Desc = dbReader.GetString(TblDocBlob.COLS.Desc.ToString())

        Me._ClassName = dbReader.GetString(TblDocBlob.COLS.ClassName.ToString())

        LoadBaseProperties(dbReader)
    End Sub

    Protected Overrides Sub SaveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("SaveDocBlob")
        cmd.AddParam(TblDocBlob.COLS.DocBlobId.ToString(), Me.DocBlobId)
        cmd.AddParam(TblDocBlob.COLS.DocBlobFK.ToString(), Me.DocBlobFK)
        cmd.AddParam(TblDocBlob.COLS.TypeCode.ToString(), Me.TypeCode)
        cmd.AddParam(TblDocBlob.COLS.StatusCode.ToString(), Me.StatusCode)
        cmd.AddParam(TblDocBlob.COLS.AttachmentBytes.ToString(), Me.AttachmentBytes)
        cmd.AddParam(TblDocBlob.COLS.AttachmentFileName.ToString(), Me.AttachmentFileName)
        'cmd.AddParam(TblDocBlob.COLS.AddedBy.ToString(), Me.AddedBy)
        'cmd.AddParam(TblDocBlob.COLS.EditedBy.ToString(), Me.EditedBy)
        cmd.AddParam(TblDocBlob.COLS.Desc.ToString(), Me.Desc)
        cmd.AddParam(TblDocBlob.COLS.ClassName.ToString(), Me.ClassName)

        SetBaseProperties(cmd)
        ProcessSaveResults(db.Execute(cmd))
    End Sub

    Public Overrides Sub Validate()
        ' @@@ Add validation
    End Sub


    Protected Overrides Sub RemoveBO(ByVal db As DB)
        Dim cmd As DBCmd = db.NewCommand("DeleteDocBlob")
        cmd.AddParam(TblDocBlob.COLS.DocBlobId.ToString(), Me.DocBlobId)
        cmd.AddParam(TblBase.BaseCOLS.LockId.ToString(), Me.LockId)

        ProcessDeleteResults(db.Execute(cmd))
    End Sub
#End Region



End Class
