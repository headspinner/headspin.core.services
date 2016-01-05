Imports System.Threading
Imports System.IO
Imports System.Configuration

Namespace Database
    ''' <summary>
    ''' The singleton manager class.
    ''' </summary>        
    Public NotInheritable Class DBMgr

        Public Const BOOTSTRAP_DB_CONN = "DefaultConnection"
        Public Const BOOTSTRAP_DB_PROVIDER = "System.Data.SqlClient"

        'Private m_sConnectionString As String ' the bootstrap conn string 'ApplicationServices'
        Private _ConnStrings As Dictionary(Of String, String) = Nothing

        Private m_sTraceFile As String
        Private m_bTraceEnabled As Boolean = False
        
        Shared ReadOnly m_instance As New DBMgr()

        ' Explicit static constructor to tell C# compiler
        ' not to mark type as beforefieldinit
        Shared Sub New()
        End Sub

        Private Sub New()
        End Sub

        Public Shared ReadOnly Property Instance() As DBMgr
            Get
                Return m_instance
            End Get
        End Property




        Public Sub Initialize(ByVal sConn As String, ByVal sTraceFile As String)
            Dim coll As New ConnectionStringSettingsCollection
            Dim conn As New ConnectionStringSettings
            conn.Name = BOOTSTRAP_DB_CONN
            conn.ConnectionString = sConn
            conn.ProviderName = BOOTSTRAP_DB_PROVIDER
            coll.Add(conn)

            Initialize(coll, sTraceFile)
        End Sub

        Public Sub Initialize(ByVal connections As ConnectionStringSettingsCollection, ByVal sTraceFile As String)

            If connections Is Nothing Then
                Throw New Exception("must declare connections to init db mgr")
            End If

            For Each conn As ConnectionStringSettings In connections
                If _ConnStrings Is Nothing Then
                    _ConnStrings = New Dictionary(Of String, String)
                End If

                If Not _ConnStrings.ContainsKey(conn.Name) Then
                    _ConnStrings.Add(conn.Name, conn.ConnectionString)
                End If
            Next



            m_bTraceEnabled = False

            If Not String.IsNullOrEmpty(sTraceFile) Then

                Dim dllUri As New Uri(System.IO.Path.GetDirectoryName( _
                    System.Reflection.Assembly.GetExecutingAssembly().CodeBase))

                Dim dir As New System.IO.DirectoryInfo(dllUri.AbsolutePath)

                Dim logpath As String = dir.Parent.Parent.FullName
                m_sTraceFile = Path.Combine(Path.Combine(logpath, "logs"), sTraceFile)

                m_bTraceEnabled = True
            End If

            MGR.Instance.LogMsg(String.Format("DBMGR Initialized {0}", DateTime.Now.ToString()))
        End Sub

        Public Function GetDB() As DB
            Return GetDB(BOOTSTRAP_DB_CONN)
        End Function

        Public Function GetDB(ByVal connectionName As String) As DB
            ' Use the connection string to return a valid cDB
            Dim db As DB = Nothing

            Try
                Dim conn As String = Nothing

                If String.IsNullOrEmpty(connectionName) Then
                    connectionName = BOOTSTRAP_DB_CONN
                End If

                If _ConnStrings.ContainsKey(connectionName) Then
                    conn = _ConnStrings.Item(connectionName)
                Else
                    Throw New Exception("No Connection defined by the name " + connectionName)
                End If

                If m_bTraceEnabled Then
                    db = New DB(conn, m_sTraceFile, connectionName)
                Else
                    db = New DB(conn, connectionName)
                End If
            Catch msg As Exception
                Throw msg
            End Try

            Return db
        End Function
    End Class
End Namespace

