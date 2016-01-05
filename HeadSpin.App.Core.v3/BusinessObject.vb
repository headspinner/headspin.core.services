Imports HeadSpin.App.Core.Database

''' <summary>
''' Summary description for Class1.
''' </summary>
Public MustInherit Class BusinessObject
    Private m_bIsDirty As Boolean = True
    Private m_bIsMarkedForRemoval As Boolean = False
    Private m_nObjectId As Integer = 0
    Private m_nLockId As Integer = 0


    Public Sub New()
    End Sub
    Public Property ObjectId() As Integer
        Get
            Return m_nObjectId
        End Get
        Protected Set(ByVal value As Integer)
            m_nObjectId = value
        End Set
    End Property
    Public Property LockId() As Integer
        Get
            Return m_nLockId
        End Get
        Private Set(ByVal value As Integer)
            m_nLockId = value
        End Set
    End Property

    Public Property UpdatedBy() As String
        Get
            Return _UpdatedBy
        End Get
        Friend Set(ByVal value As String)
            _UpdatedBy = value
        End Set
    End Property
    Private _UpdatedBy As String = Nothing

    Public Property CreatedBy() As String
        Get
            Return _CreatedBy
        End Get
        Friend Set(ByVal value As String)
            _CreatedBy = value
        End Set
    End Property
    Private _CreatedBy As String

    Public Property CreatedDate() As DateTime
        Get
            Return _CreatedDate
        End Get
        Friend Set(ByVal value As DateTime)
            _CreatedDate = value
        End Set
    End Property
    Private _CreatedDate As DateTime = DateTime.Now

    Public Property UpdatedDate() As DateTime?
        Get
            Return _UpdatedDate
        End Get
        Friend Set(ByVal value As DateTime?)
            _UpdatedDate = value
        End Set
    End Property
    Private _UpdatedDate As DateTime? = Nothing

    Public ReadOnly Property LastUpdated() As DateTime
        Get
            If Me.UpdatedDate.HasValue Then
                Return Me.UpdatedDate.Value
            Else
                Return Me.CreatedDate
            End If
        End Get
    End Property

    Public Function IsMarkedForRemoval() As Boolean
        Return m_bIsMarkedForRemoval
    End Function

    Public Sub MarkForRemoval()
        m_bIsMarkedForRemoval = True

        Me.SetDirty(True)
    End Sub

    Public Sub UnMarkForRemoval()
        m_bIsMarkedForRemoval = False
        Me.SetDirty(True)
    End Sub


    Protected MustOverride Function GetListCmdImpl(ByVal db As DB, ByVal crit As Criteria) As DBCmd

    Protected Overridable Function NewForListLoad() As BusinessObject
        Throw New NotImplementedException()
    End Function



    Public Shared Function GetOneObject(Of T As BusinessObject)(ByVal bo As BusinessObject, ByVal crit As Criteria) As T
        Dim oneBo As T = Nothing

        Dim bos As BusinessObjectCollection(Of T) = BusinessObject.GetObjectList(Of T)(bo, crit)

        If bos IsNot Nothing Then
            Dim count As Integer = bos.Count
            If count > 1 Then
                Throw New Exception(String.Format("{0} objects found when expecting 1 for type {1}", count, bo.GetType().Name))
            End If

            oneBo = bos.Item(0)
        End If

        Return oneBo

    End Function

    Protected Shared Function GetObjectList(Of T As BusinessObject)(ByVal bo As BusinessObject, ByVal crit As Criteria) As BusinessObjectCollection(Of T)
        If bo Is Nothing Then
            Throw New ArgumentNullException("bo argument of type BusinessObject must not be null")
        End If

        'Now the db connection is managed in one place for list gets!
        Dim connName As String = Nothing
        If crit IsNot Nothing Then
            connName = crit.ConnectionName
        End If
        Using db As DB = DBMgr.Instance.GetDB(connName)

            Dim ret As BusinessObjectCollection(Of T) = Nothing

            Dim cmd As DBCmd = bo.GetListCmdImpl(db, crit)

            Dim reader As DBReader = db.Execute(cmd)

            While (reader.Read())
                If ret Is Nothing Then
                    ret = New BusinessObjectCollection(Of T)
                End If

                Dim o As T = DirectCast(bo.NewForListLoad(), T)
                o.Load(reader)
                ret.Add(o)
            End While

            Return ret

        End Using
    End Function

    Protected Sub SetBaseProperties(ByVal cmd As DBCmd)
        cmd.AddParam(TblBase.BaseCOLS.LockId.ToString(), Me.LockId)
        cmd.AddParam(TblBase.BaseCOLS.CreatedBy.ToString(), Me.CreatedBy)
        cmd.AddParam(TblBase.BaseCOLS.UpdatedBy.ToString(), Me.UpdatedBy)
    End Sub
    Protected Sub LoadBaseProperties(ByVal reader As DBReader)
        Me.CreatedDate = reader.GetDateTime(TblBase.BaseCOLS.CreatedDate.ToString())
        Me.UpdatedDate = reader.GetNullableDateTime(TblBase.BaseCOLS.UpdatedDate.ToString())
        Me.LockId = reader.GetInteger(TblBase.BaseCOLS.LockId.ToString())
        Me.CreatedBy = reader.GetString(TblBase.BaseCOLS.CreatedBy.ToString())
        Me.UpdatedBy = reader.GetString(TblBase.BaseCOLS.UpdatedBy.ToString())
    End Sub

    Protected Overridable Sub ProcessDeleteResults(ByVal reader As DBReader)
        ' set the object id and lock id
        Using reader
            While reader.Read()

                If reader.ColumnCount > 0 Then
                    Dim rowsDeleted As Integer = reader.GetInteger()
                    If 0 = rowsDeleted Then
                        Throw New Exception("Ojbect updated prior to delete and thus not deleted: " & Me.GetType().Name)
                    End If
                End If
            End While
        End Using
    End Sub

    Protected Overridable Sub ProcessSaveResults(ByVal reader As DBReader)
        ' set the object id and lock id
        Using reader
            While reader.Read()
                Me.ObjectId = reader.GetInteger()
                Me.LockId = reader.GetInteger()
                Me.CreatedDate = reader.GetDateTime()
                Me.UpdatedDate = reader.GetNullableDateTime()

                If reader.ColumnCount > 4 Then
                    Dim rowsUpdated As Integer = reader.GetInteger()
                    If 0 = rowsUpdated Then
                        Throw New ConcurrentUpdateException(Me)
                    End If
                End If
            End While
        End Using


    End Sub

    Public Overridable Sub Delete()
        Me.MarkForRemoval()
        Me.Save()
    End Sub

    Public Overridable Sub Delete(ByVal db As DB)
        Me.MarkForRemoval()
        Me.Save(db)
    End Sub


    Protected ReadOnly Property SavingUsername As String
        Get
            Return _SavingUsername
        End Get
    End Property
    Public Sub SetSavingUsername(ByVal username As String)
        If String.IsNullOrEmpty(username) Then
            username = "anonymous"
        End If
        _SavingUsername = username
    End Sub

    Private _SavingUsername As String = Nothing

    Private Sub SetUserProperties()

        If _SavingUsername IsNot Nothing Then

            If Me.IsNew Then
                Me.CreatedBy = _SavingUsername
            Else
                Me.UpdatedBy = _SavingUsername
            End If
        End If

    End Sub

    Public Overridable Sub Save(ByVal username As String)
        Me.SetSavingUsername(username)
        Me.Save()
    End Sub
    Public Overridable Sub Save(ByVal username As String, ByVal db As DB)
        Me.SetSavingUsername(username)

        If db Is Nothing Then
            Me.Save()
        Else
            Me.Save(db)
        End If
    End Sub
    Public Overridable Sub Save()
        Using db As DB = DBMgr.Instance.GetDB()
            Try
                db.BeginTransaction()
                Me.Save(db)
                db.CommitTransaction()
            Catch e As Exception
                db.RollbackTransaction()
                Throw
            End Try
        End Using
    End Sub


    Public Overridable Sub Save(ByVal db As DB)
        Try
            If Me.IsDirty() Then
                If Me.IsMarkedForRemoval() Then
                    Me.PreRemove(db)
                    Me.RemoveBO(db)
                    Me.PostRemove(db)
                Else
                    Me.SetUserProperties()
                    Me.Validate()
                    Me.PreSave(db)
                    Me.SaveBO(db)
                    Me.PostSave(db)
                End If
                Me.SetDirty(False)
            End If
        Catch msg As Exception
            Throw msg
        End Try

        ' @@@ create a db err
    End Sub

    Public Overridable Sub Load(ByVal dbReader As DBReader)
        Me.LoadBO(dbReader)

        Me.SetDirty(False)
    End Sub

    'Public Overridable Sub Load(ByVal nId As Integer)
    '    Me.LoadBO(nId)

    '    Me.SetDirty(False)
    'End Sub

    'Public Overridable Sub Load(ByVal nId As Integer, ByVal db As DB)
    '    Me.LoadBO(nId, db)

    '    Me.SetDirty(False)
    'End Sub


    'Public Overridable Sub ReLoad()
    '    '@@@Me.Load(Me.ObjectId)
    'End Sub

    Public Function IsNew() As Boolean
        If Me.ObjectId > 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Overridable Function IsDirty() As Boolean
        Return m_bIsDirty
    End Function

    Protected MustOverride Sub SaveBO(ByVal db As DB)
    Protected MustOverride Sub LoadBO(ByVal dbReader As DBReader)
    Protected MustOverride Sub RemoveBO(ByVal db As DB)
    Public MustOverride Sub Validate()

    'Protected Overridable Sub LoadBO(ByVal nId As Integer)
    'End Sub
    'Protected Overridable Sub LoadBO(ByVal nId As Integer, ByVal db As DB)
    'End Sub

    Protected Overridable Sub PreSave(ByVal db As DB)
    End Sub
    Protected Overridable Sub PostSave(ByVal db As DB)
    End Sub

    Protected Overridable Sub PreRemove(ByVal db As DB)
    End Sub
    Protected Overridable Sub PostRemove(ByVal db As DB)
    End Sub

    Protected Function CheckDirty(ByVal sNewVal As Guid, ByVal sCurrentVal As Guid) As Guid
        If sNewVal <> sCurrentVal Then
            Me.SetDirty(True)
            Return sNewVal
        Else
            Return sCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal sNewVal As String, ByVal sCurrentVal As String) As String
        If sNewVal <> sCurrentVal Then
            Me.SetDirty(True)
            Return sNewVal
        Else
            Return sCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal nNewVal As Boolean, ByVal nCurrentVal As Boolean) As Boolean
        If nNewVal <> nCurrentVal Then
            Me.SetDirty(True)
            Return nNewVal
        Else
            Return nCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal nNewVal As Boolean?, ByVal nCurrentVal As Boolean?) As Boolean?
        If Not nNewVal.Equals(nCurrentVal) Then
            Me.SetDirty(True)
            Return nNewVal
        Else
            Return nCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal nNewVal As System.Nullable(Of Integer), ByVal nCurrentVal As System.Nullable(Of Integer)) As System.Nullable(Of Integer)
        If Not nNewVal.Equals(nCurrentVal) Then
            Me.SetDirty(True)
            Return nNewVal
        Else
            Return nCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal nNewVal As System.Nullable(Of Decimal), ByVal nCurrentVal As System.Nullable(Of Decimal)) As System.Nullable(Of Decimal)
        If Not nNewVal.Equals(nCurrentVal) Then
            Me.SetDirty(True)
            Return nNewVal
        Else
            Return nCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal dtNewVal As System.Nullable(Of DateTime), ByVal dtCurrentVal As System.Nullable(Of DateTime)) As System.Nullable(Of DateTime)
        If Not dtNewVal.Equals(dtCurrentVal) Then
            Me.SetDirty(True)
            Return dtNewVal
        Else
            Return dtCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal nNewVal As Integer, ByVal nCurrentVal As Integer) As Integer
        If nNewVal <> nCurrentVal Then
            Me.SetDirty(True)
            Return nNewVal
        Else
            Return nCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal nNewVal As Decimal, ByVal nCurrentVal As Decimal) As Decimal
        If nNewVal <> nCurrentVal Then
            Me.SetDirty(True)
            Return nNewVal
        Else
            Return nCurrentVal
        End If
    End Function

    Protected Function CheckDirty(ByVal dtNewVal As DateTime, ByVal dtCurrentVal As DateTime) As DateTime
        If dtNewVal <> dtCurrentVal Then
            Me.SetDirty(True)
            Return dtNewVal
        Else
            Return dtCurrentVal
        End If
    End Function



    Private Function ByteArrayCompare(ByVal a1 As Byte(), ByVal a2 As Byte()) As Boolean
        If a1 Is a2 Then
            Return True
        End If
        If (a1 IsNot Nothing) AndAlso (a2 IsNot Nothing) Then
            If a1.Length <> a2.Length Then
                Return False
            End If
            For i As Integer = 0 To a1.Length - 1
                If a1(i) <> a2(i) Then
                    Return False
                End If
            Next
            Return True
        End If
        Return False
    End Function

    'Protected Function CheckDirty(ByVal NewEncryptable As EncryptableBytes, ByVal CurrentEncryptable As EncryptableBytes) As EncryptableBytes
    '    If NewEncryptable Is Nothing AndAlso CurrentEncryptable Is Nothing Then
    '        ' both null
    '        Return CurrentEncryptable
    '    ElseIf NewEncryptable IsNot Nothing AndAlso CurrentEncryptable IsNot Nothing Then
    '        ' both not null, check contents
    '        If Not ByteArrayCompare(NewEncryptable.Decrypted, CurrentEncryptable.Decrypted) Then
    '            Me.SetDirty(True)
    '            Return NewEncryptable
    '        Else
    '            Return CurrentEncryptable
    '        End If
    '    Else
    '        ' one is null, other isn't, return the new one.
    '        Me.SetDirty(True)
    '        Return NewEncryptable
    '    End If
    'End Function

    Protected Function CheckDirty(ByVal NewBytes As Byte(), ByVal CurrentBytes As Byte()) As Byte()
        If Not ByteArrayCompare(NewBytes, CurrentBytes) Then
            Me.SetDirty(True)
            Return NewBytes
        Else
            Return CurrentBytes
        End If
    End Function

    Protected Sub SetDirty(ByVal bDirty As Boolean)
        m_bIsDirty = bDirty
    End Sub

End Class


