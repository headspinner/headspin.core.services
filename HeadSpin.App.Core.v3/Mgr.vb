Imports System.IO
Imports System.Configuration
Imports HeadSpin.App.Core.Database



''' <summary>
''' Summary description for MGR.
''' </summary>
Public NotInheritable Class MGR
    Shared ReadOnly m_instance As New MGR()

    ' Explicit static constructor to tell C# compiler
    ' not to mark type as beforefieldinit
    Shared Sub New()
    End Sub

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property Instance() As MGR
        Get
            Return m_instance
        End Get
    End Property

    Private _lookups As BusinessObjectCollection(Of Lookup) = Nothing


    Public Function GetLookup(ByVal id As Integer) As Lookup
        Dim crit As New Criteria()
        crit.Add(TblLookup.COLS.LookupId.ToString(), id)

        Dim lkps As BusinessObjectCollection(Of Lookup) = GetLookups(crit)

        If lkps Is Nothing Then
            Throw New Exception("No lookup with id " + id.ToString())
        ElseIf lkps.Count = 0 Then
            Throw New Exception("No lookup with id " + id.ToString())
        End If

        Return lkps.First()
    End Function

    Public Function GetSystemConfig(key As String, create As Boolean) As SystemConfig

        Dim c As SystemConfig = SystemConfig.GetByKey(key)

        If c Is Nothing AndAlso create Then
            c = New SystemConfig() With {.Key = key}
        End If

        Return c

    End Function
    Public Function GetLookup(ByVal lookupType As String, ByVal lookupCode As String) As Lookup
        If String.IsNullOrEmpty(lookupType) Then
            Return Nothing
        End If
        If String.IsNullOrEmpty(lookupCode) Then
            Return Nothing
        End If
        Dim crit As New Criteria()
        crit.Add(TblLookup.COLS.LookupType.ToString(), lookupType)
        crit.Add(TblLookup.COLS.LookupCode.ToString(), lookupCode)

        Dim lkps As BusinessObjectCollection(Of Lookup) = GetLookups(crit)

        If lkps IsNot Nothing AndAlso lkps.Count > 0 Then
            Return lkps.First()
        End If

        Return Nothing
    End Function

    Public Function GetLookups(lookupType As String, group As String) As List(Of Lookup)

        Dim lkps = GetLookups(lookupType)

        If lkps IsNot Nothing Then

            Dim results = (From l In lkps Where l.LookupType = lookupType And l.LookupGroup = group Select l)

            If results IsNot Nothing Then
                Return results.ToList()
            End If
        End If

        Return Nothing

        
    End Function

    Public Function GetLookupsByGroup(ByVal group As String) As IEnumerable(Of Lookup)
        If _lookups IsNot Nothing Then
            Dim glkps = From g In _lookups Where Not String.IsNullOrEmpty(g.LookupGroup) _
                        AndAlso g.LookupGroup = group Select g

            If glkps IsNot Nothing Then
                Return glkps
            End If
        End If

        Return Nothing
    End Function

   

    Public Function GetLookups(ByVal lookupType As String, ByVal parentLookupType As String, ByVal parentLookupCode As String) As IEnumerable(Of Lookup)

        Dim lkps = GetLookups(lookupType)

        If lkps IsNot Nothing Then
            If Not String.IsNullOrEmpty(parentLookupCode) Then
                Return (From l In lkps Where l.LookupType = lookupType And l.ParentLookupType = parentLookupType And l.ParentLookupCode = parentLookupCode Select l)
            Else
                Return (From l In lkps Where l.LookupType = lookupType And l.ParentLookupType = parentLookupType Select l)
            End If

        End If

        Return Nothing

    End Function

    Public Function GetLookups(ByVal lookupType As String, ByVal parentId As Integer?) As BusinessObjectCollection(Of Lookup)

        Dim crit As New Criteria()
        crit.Add(TblLookup.COLS.LookupType.ToString(), lookupType)

        If parentId.HasValue() Then
            crit.Add(TblLookup.COLS.ParentLookupId.ToString(), parentId.Value)
        Else
            crit.Add(TblLookup.COLS.ParentLookupId.ToString(), 0)
        End If

        Return GetLookups(crit)
    End Function
    Public Function GetLookups(ByVal lookupType As String) As BusinessObjectCollection(Of Lookup)
        Return GetLookups(lookupType, -1)
    End Function

    Public Function BlankLookup() As Lookup
        Dim l As New Lookup()

        l.LookupDescription = "Select ..."
        l.LookupCode = ""
        Return l
    End Function

    Private Function GetLookups(ByVal crit As Criteria) As BusinessObjectCollection(Of Lookup)

        SyncLock Me
            ' populate cache first time thru...
            If _lookups Is Nothing Then

                Using db As Database.DB = DBMgr.Instance.GetDB()
                    Dim cmd As DBCmd = db.NewCommand("GetLookup")

                    Dim reader As DBReader = db.Execute(cmd)

                    _lookups = New BusinessObjectCollection(Of Lookup)()

                    While (reader.Read())
                        Dim o As New Lookup()
                        o.Load(reader)
                        _lookups.Add(o)
                    End While
                End Using

            End If
        End SyncLock


        Dim lkpType As String = crit.Get(Of String)(TblLookup.COLS.LookupType.ToString(), String.Empty)
        Dim lkpId As Integer = crit.Get(Of Integer)(TblLookup.COLS.LookupId.ToString(), 0)
        Dim parentId As Integer = crit.Get(Of Integer)(TblLookup.COLS.ParentLookupId.ToString(), -1)
        Dim lkpCode As String = crit.Get(Of String)(TblLookup.COLS.LookupCode.ToString(), String.Empty)

        If lkpId > 0 Then

            Dim lkp = From l In _lookups Where l.LookupId = lkpId Select l

            Dim filtered As New BusinessObjectCollection(Of Lookup)()
            filtered.AddRange(lkp)

            Return filtered
        ElseIf (Not String.IsNullOrEmpty(lkpType)) AndAlso (Not String.IsNullOrEmpty(lkpCode)) Then
            Dim typeString As String = lkpType.ToString()

            If parentId > -1 Then
                Dim lkp = From l In _lookups Where l.LookupType = typeString And l.ParentLookupId = parentId And l.LookupCode = lkpCode Select l
                Dim filtered As New BusinessObjectCollection(Of Lookup)()
                filtered.AddRange(lkp)
                Return filtered
            Else
                Dim lkp = From l In _lookups Where l.LookupType = typeString And l.LookupCode = lkpCode Select l
                Dim filtered As New BusinessObjectCollection(Of Lookup)()
                filtered.AddRange(lkp)
                Return filtered
            End If

        ElseIf Not String.IsNullOrEmpty(lkpType) Then
            Dim typeString As String = lkpType

            If parentId > -1 Then
                Dim lkp = From l In _lookups Where l.LookupType = typeString And l.ParentLookupId = parentId Select l
                Dim filtered As New BusinessObjectCollection(Of Lookup)()
                filtered.AddRange(lkp)
                Return filtered
            Else
                Dim lkp = From l In _lookups Where l.LookupType = typeString Select l
                Dim filtered As New BusinessObjectCollection(Of Lookup)()
                filtered.AddRange(lkp)
                Return filtered
            End If
        Else
            Return _lookups
        End If

    End Function
    Public Sub LogDebugMsg(ByVal msg As String)

        Dim debug As String = ConfigurationManager.AppSettings("debug")

        If Not String.IsNullOrEmpty(debug) AndAlso debug.Trim().ToUpper() = "TRUE" Then
            LogMsg(msg)
        End If
    End Sub
    Public Sub LogMsg(ByVal sMsg As String)
        Dim oMsg As cMsg = MGR.Instance.GetNewMsg(sMsg)
        oMsg.Log()
    End Sub

    Friend Sub LogMessage(ByVal msg As String)
        LogMessage(msg, m_sLogFile)
    End Sub
    Friend Sub LogMessage(ByVal msg As String, ByVal logFile As String)
        If Not String.IsNullOrEmpty(logFile) Then


            SyncLock Me
                Dim writer As StreamWriter = Nothing

                Try
                    If Not File.Exists(logFile) Then
                        writer = File.CreateText(logFile)
                    Else
                        Dim fs As FileStream = File.OpenWrite(logFile)
                        fs.Seek(0, SeekOrigin.[End])
                        writer = New StreamWriter(fs)
                    End If

                    Dim dt As DateTime = DateTime.Now

                    Dim sDateTime As String = dt.ToString()
                    sDateTime += " "
                    sDateTime += dt.Millisecond.ToString()

                    writer.WriteLine(sDateTime)
                    writer.WriteLine(msg)

                    Console.WriteLine(sDateTime + " " + msg)


                    'writer.Flush();

                    writer.WriteLine(" ")
                Catch e As Exception
                    ' probably don;t want to re-log this msg b/c wer
                    ' could wind up back in here w/a recursion issue.
                    Throw e
                Finally
                    If writer IsNot Nothing Then
                        writer.Close()
                    End If
                End Try
            End SyncLock
        End If
    End Sub

    Private m_sLogFile As String = String.Empty
    Private m_bAllowMultipleLogins As Boolean = True
    Private m_bLogPerf As Boolean = False

    Public Sub Initialize(ByVal sLogFile As String, ByVal bAllowMultipleLogins As Boolean, ByVal bLogPerf As Boolean)
        Initialize(sLogFile, bAllowMultipleLogins)
        m_bLogPerf = bLogPerf

    End Sub

    Public Sub Initialize(ByVal sLogFile As String, ByVal bAllowMultipleLogins As Boolean)

        If Not String.IsNullOrEmpty(sLogFile) Then
            Dim dllUri As New Uri(System.IO.Path.GetDirectoryName( _
           System.Reflection.Assembly.GetExecutingAssembly().CodeBase))

            Dim dir As New System.IO.DirectoryInfo(dllUri.AbsolutePath)

            Dim logpath As String = dir.Parent.Parent.FullName

            m_sLogFile = Path.Combine(Path.Combine(logpath, "logs"), sLogFile)
        Else
            m_sLogFile = sLogFile
        End If


        m_bAllowMultipleLogins = bAllowMultipleLogins

        MGR.Instance.LogMsg(String.Format("MGR Initialized {0}", DateTime.Now.ToString()))
    End Sub

    Public Sub Initialize(ByVal sLogFile As String)
        Initialize(sLogFile, True)
    End Sub

    Public Function GetTempFile(ByVal dir As String) As String
        ' i.e. c:\temp
        Dim tempDir As String = dir

        ' i.e. c:\someplace\tempfile.tmp
        Dim tempFilenamePath As String = Path.GetTempFileName()

        Return String.Format("{0}{1}{2}", tempDir, Path.DirectorySeparatorChar, Path.GetFileName(tempFilenamePath))

    End Function

    Public Function GetTempFile() As String
        ' i.e. c:\temp
        Dim tempDir As String = ConfigurationManager.AppSettings("temp")

        ' i.e. c:\someplace\tempfile.tmp
        Dim tempFilenamePath As String = Path.GetTempFileName()

        Return String.Format("{0}{1}{2}", tempDir, Path.DirectorySeparatorChar, Path.GetFileName(tempFilenamePath))

    End Function
    Public Function IsLogPerfEnabled() As Boolean
        Return m_bLogPerf
    End Function

    'public cUserCollection GetUsers ( DB db )
    '{
    '   // @@@ Should probably take a crit object.
    '   // @@@ We'll get all of them for now.

    '   cTblUsers tblUsers = new cTblUsers ( );

    '   cDBSelector dbSel = new cDBSelector ( );
    '   tblUsers.GenerateSelect ( dbSel ) ;

    '   cDBCrit dbCrit = new cDBCrit ( );
    '   dbCrit.AndEquals( tblUsers, tblUsers.ColumnName( (int) cTblUsers.COLS.STATUS_CD ), "ACT" );

    '   dbSel.Where( dbCrit );

    '   cDBReader dbReader = db.Execute ( dbSel );

    '   cUserCollection Users = new cUserCollection ( );

    '   while ( dbReader.Read ( ) )
    '   {
    '      cUser user = new cUser ( );

    '      user.Load ( dbReader );

    '      Users.Add ( user );
    '   }			

    '   return Users;
    '}

    'public void LogoutAll ( )
    '{
    '   // log out all users;
    '   cTblUsers tblUsers = new cTblUsers ( );

    '   string sql = "UPDATE USERS SET IS_LOGGED_IN = @IS_LOGGED_IN";

    '   cDBUpdater dbUpd = new cDBUpdater ( sql );

    '   dbUpd.Add ( tblUsers.ColumnName ( ( int ) cTblUsers.COLS.IS_LOGGED_IN ), 0 );                    

    '   DB db = DBMgr.Mgr.GetDB ( );

    '   int count = db.Execute ( dbUpd );

    '   if ( count == 0 )
    '   {
    '      cMsg msg = GetNewMsg ( "No users to log out" );

    '      msg.Log ( );               
    '   }           

    '}

    'public void Logout ( cUser User )
    '{
    '   User.IsLoggedIn = false;
    '   User.Save ( DBMgr.Mgr.GetDB ( ) );
    '}

    'public void Login ( string sLoginId, string sPassword, cUser oUser )
    '{
    '   GetUser ( sLoginId, sPassword,  oUser );

    '   if ( ( ! m_bAllowMultipleLogins ) && ( oUser != null ) && ( oUser.IsLoggedIn ) )
    '   {
    '      cMsg oMsg = GetNewMsg ( "You are curently logged in, multiple login's are not allowed!" );
    '      oMsg.Log ( );
    '      throw oMsg;
    '   }

    '   oUser.IsLoggedIn = true;
    '   oUser.Save ( DBMgr.Mgr.GetDB ( ) );
    '}

    'public cUser Login ( string sLoginId, string sPassword )
    '{
    '   cUser oUser = GetUser ( sLoginId, sPassword );

    '   if ( ( ! m_bAllowMultipleLogins ) && ( oUser != null ) && ( oUser.IsLoggedIn ) )
    '   {
    '      cMsg oMsg = GetNewMsg ( "You are curently logged in, multiple login's are not allowed!" );
    '      oMsg.Log ( );
    '      throw oMsg;
    '   }

    '   oUser.IsLoggedIn = true;
    '   oUser.Save ( DBMgr.Mgr.GetDB ( ) );

    '   return oUser;              
    '}

    'public cUser GetUser ( string sLoginId, string sPassword )
    '{
    '   cUser User = new cUser ( );

    '   MGR.Instance.GetUser ( sLoginId, sPassword,  User );

    '   return User;
    '}

    'public cUser GetUser ( int nUID, DB oDB )
    '{
    '   cUser oUser = new cUser();

    '   oUser.Load ( nUID, oDB );

    '   return oUser;
    '}

    'public void GetUser ( string sLoginId, string sPassword, cUser User )
    '{
    '   DB db = DBMgr.Mgr.GetDB ( );

    '   cTblUsers tblU = new cTblUsers ( );

    '   cUserCrit oUserCrit = new cUserCrit ( db );
    '   oUserCrit.UserName = sLoginId;
    '   oUserCrit.Password = sPassword;

    '   cDBSelector dbSel = new cDBSelector ( );

    '   tblU.GenerateSelect ( dbSel );

    '   cDBCrit dbCrit = new cDBCrit ( );

    '   if ( oUserCrit.UserName != string.Empty )
    '   {
    '      dbCrit.AndEquals ( tblU, tblU.ColumnName ( (int) cTblUsers.COLS.LOGIN_ID ), oUserCrit.UserName );
    '   }                
    '   if ( oUserCrit.Password != string.Empty )
    '   {
    '      dbCrit.AndEquals ( tblU, tblU.ColumnName ( (int) cTblUsers.COLS.PASSWORD_TX ), oUserCrit.Password );
    '   }

    '   dbSel.Where ( dbCrit );

    '   cDBReader dbReader = db.Execute ( dbSel );

    '   // keep track of how many users are returned
    '   // so we can throw if there are duplicates.
    '   int nUserCount = 0;

    '   while ( dbReader.Read ( ) )
    '   {
    '      nUserCount++;

    '      User.Load ( dbReader );                    
    '   }

    '   if ( nUserCount > 1 )
    '   {
    '      User = null;

    '      // Should be only one user w/that id/pwd combo!
    '      cMsg errMsg = GetNewMsg ( "Duplicate users" );
    '      errMsg.Log ( );
    '      throw errMsg;
    '   }

    '   if ( nUserCount == 0 )
    '   {
    '      User = null;

    '      cMsg errMsg = GetNewMsg ( "Incorrect user name and/or password" );               
    '      errMsg.Log ( );
    '      throw errMsg;
    '   }              
    '}


    'public cUserDefDetails GetUserDefinedDetails ( cUserDefDetailCrit oUDDCrit )
    '{  
    '   DB db = oUDDCrit.DB;

    '   // get the contacts!
    '   cTblUserDefDetail tblUDD = new cTblUserDefDetail ( );

    '   cDBSelector dbSel = new cDBSelector ( );

    '   tblUDD.GenerateSelect ( dbSel );

    '   cDBCrit dbCrit = new cDBCrit ( );

    '   if ( oUDDCrit.RelateId > 0 )
    '   {
    '      dbCrit.AndEquals ( tblUDD, tblUDD.ColumnName ( (int) cTblUserDefDetail.COLS.RELATE_ID ), oUDDCrit.RelateId );
    '   }
    '   if ( oUDDCrit.RelateClass != string.Empty )
    '   {                                           
    '      dbCrit.AndEquals ( tblUDD, tblUDD.ColumnName ( (int) cTblUserDefDetail.COLS.RELATE_CLASS ), oUDDCrit.RelateClass );
    '   }
    '   if ( oUDDCrit.TypeCode != string.Empty )
    '   {                                           
    '      dbCrit.AndEquals ( tblUDD, tblUDD.ColumnName ( (int) cTblUserDefDetail.COLS.TYPE_CD ), oUDDCrit.TypeCode );
    '   }
    '   if ( oUDDCrit.SubTypeCode != string.Empty )
    '   {                                           
    '      dbCrit.AndEquals ( tblUDD, tblUDD.ColumnName ( (int) cTblUserDefDetail.COLS.SUB_TYPE_CD ), oUDDCrit.SubTypeCode );
    '   }
    '   if ( oUDDCrit.StatusCode != string.Empty )
    '   {                                           
    '      dbCrit.AndEquals ( tblUDD, tblUDD.ColumnName ( (int) cTblUserDefDetail.COLS.STATUS_CD ), oUDDCrit.StatusCode );
    '   }

    '   dbSel.Where ( dbCrit );

    '   if ( oUDDCrit.OrderBySysTmStamp )
    '   {
    '      dbSel.OrderByDesc ( tblUDD, "SYS_DT_STAMP" );
    '   }

    '   cDBReader dbReader = db.Execute ( dbSel );

    '   cUserDefDetails oUDDs = new cUserDefDetails ( );

    '   while ( dbReader.Read ( ) )
    '   {
    '      cUserDefDetail udd = new cUserDefDetail ( );
    '      udd.Load ( dbReader );
    '      oUDDs.Add ( udd );
    '   }		
    '   return oUDDs;
    '}

    Public Function GetNewMsg(ByVal e As Exception) As cMsg
        Return GetNewMsg(String.Format("Exception {0}, StackTrace{1}", e.Message, e.StackTrace))
    End Function

    Public Function GetNewMsg(ByVal sText As String) As cMsg
        Dim oMsg As cMsg = Nothing

        If m_sLogFile <> String.Empty Then
            oMsg = New cMsg(sText, m_sLogFile)
        Else
            oMsg = New cMsg(sText)
        End If

        Return oMsg
    End Function



    Public Sub LogException(ByVal e As Exception)
        Dim outer As cMsg = MGR.Instance.GetNewMsg(e)
        outer.Log()

        If e.InnerException IsNot Nothing Then
            Dim inner As cMsg = MGR.Instance.GetNewMsg(e.InnerException)
            inner.Log()
        End If
    End Sub
    'public void LogAndThrow(string errorMessage)
    '{
    '    LogAndThrow(new Exception(errorMessage));
    '}
    'public void LogAndThrow(Exception e)
    '{
    '    LogException(e);

    '    if (e.InnerException != null)
    '    {
    '        LogException(e.InnerException);
    '    }

    '    throw e;             
    '}
End Class

