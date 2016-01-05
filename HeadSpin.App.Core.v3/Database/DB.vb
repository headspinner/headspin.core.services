Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.IO


Namespace Database
    ''' <summary>        
    '''         
    ''' </summary>
    Public Class DB
        Implements IDisposable

        Private m_Conn As DbConnection = Nothing
        Private _Name As String = Nothing
        Private m_Trans As DbTransaction = Nothing

        ' These will both be set if tracing is enabled.
        Private m_sTraceFile As String
        Private m_bTraceEnabled As Boolean = False

        ' @@@ Should only be created by DBMGR
        ' @@@ Figure out how to do a friend access
        ' @@@ protector in c#.  Friend access is 
        ' internal in c#.  Classes in the same name
        ' space can look @ internal classes.
        Friend Sub New(ByVal sConn As String, ByVal connName As String)
            Try
                If String.IsNullOrEmpty(connName) Then
                    Throw New Exception("Must set a connection name on DB initialize")
                End If

                m_Conn = New SqlConnection(sConn)
                _Name = connName
            Catch e As SqlException
                MGR.Instance.LogException(e)
                Throw e
            End Try
        End Sub
        Friend Sub New(ByVal sConn As String, ByVal sTraceFile As String, ByVal connName As String)
            Me.New(sConn, connName)
            m_bTraceEnabled = True

            m_sTraceFile = sTraceFile
        End Sub

        Public Function NewCommand(ByVal name As String) As DBCmd
            Return New DBCmd(name)
        End Function
        ' @@@ new!
        Public Function Execute(ByVal cmd As DBCmd) As DBReader
            Try

                ' Initialize the connection on the command object.                
                Dim dbCmd As DbCommand = cmd.GetCommand()
                dbCmd.Transaction = m_Trans
                dbCmd.Connection = m_Conn

                If m_bTraceEnabled Then
                    Me.Trace(dbCmd)
                End If


                Me.OpenConnection()

                Dim dbReader As New DBReader(dbCmd.ExecuteReader())

                Return dbReader
            Catch e As Exception
                MGR.Instance.LogException(e)
                Throw
            End Try
        End Function


        'public int Execute(cDBDeleter dbDeleter)
        '{
        '    return this.ExecuteNonQuery(dbDeleter);
        '}

        ' Try this one using a forward only reader instead of a data set.
        ' 15% performance kick w/the data reader impl.
        'public DBReader Execute(cDBSelector dbSelector)
        '{
        '    try
        '    {
        '        cPerformanceTimer oTimer = new cPerformanceTimer();

        '        oTimer.Start(this);

        '        // Initialize the connection on the command object.                
        '        SqlCommand dbCmd = dbSelector.GetCommand();

        '        if (m_bTraceEnabled)
        '        {
        '            this.Trace(dbCmd);
        '        }

        '        //@@@dbCmd.Connection = m_Conn;

        '        this.OpenConnection();

        '        SqlDataReader sqlDR = dbCmd.ExecuteReader(CommandBehavior.CloseConnection);

        '        DBReader dbReader = new DBReader(sqlDR);

        '        oTimer.Stop();

        '        oTimer.LogElapsed();

        '        return dbReader;
        '    }
        '    catch (Exception e)
        '    {
        '        MGR.Instance.LogException(e);
        '        throw;
        '    }
        '    finally
        '    {
        '        //CloseConnection();
        '    }
        '}


        Private Function IsOpen() As Boolean
            Dim bIsOpen As Boolean = False

            If m_Conn IsNot Nothing Then
                If m_Conn.State = ConnectionState.Open Then
                    bIsOpen = True
                End If
            End If

            Return bIsOpen
        End Function

        Public Sub BeginTransaction()
            OpenConnection()
            m_Trans = m_Conn.BeginTransaction()
        End Sub
        Public Sub CommitTransaction()
            Try
                If m_Trans IsNot Nothing Then
                    m_Trans.Commit()

                End If
            Catch e As Exception
                MGR.Instance.LogException(e)
                Throw
            Finally
                CloseConnection()
            End Try
        End Sub
        Public Sub RollbackTransaction()
            Try
                If m_Trans IsNot Nothing Then
                    m_Trans.Rollback()
                End If
            Catch e As Exception
                MGR.Instance.LogException(e)
                Throw
            Finally
                CloseConnection()
            End Try
        End Sub

        'private int ExecuteNonQuery(cDBStatement dbStatement)
        '{
        '    OpenConnection();

        '    SqlCommand dbCmd = dbStatement.GetCommand();

        '   // dbCmd.Connection = m_Conn;
        '   // dbCmd.Transaction = m_Trans;

        '    if (m_bTraceEnabled)
        '    {
        '        this.Trace(dbCmd);
        '    }

        '    int count = dbCmd.ExecuteNonQuery();

        '    return count;
        '}

        Private Sub Trace(ByVal dbCmd As DbCommand)
            Dim sSQL As String = dbCmd.CommandText

            Dim sNameValuePair As String = ""

            Dim dbParms As DbParameterCollection = dbCmd.Parameters

            For Each dbParm As SqlParameter In dbParms
                Dim sName As String = ""
                Dim sValue As String = ""

                ' The param itself should never be null
                If dbParm IsNot Nothing Then
                    sName = dbParm.ParameterName

                    ' The Value could be null!
                    If dbParm.Value IsNot Nothing Then
                        sValue = dbParm.Value.ToString()
                    End If
                End If


                sNameValuePair += " [ "
                sNameValuePair += sName
                sNameValuePair += " "
                sNameValuePair += sValue
                sNameValuePair += " ] "
            Next

            Dim msg As String = "Connection: " & _Name & " " & sSQL & Environment.NewLine & " " & sNameValuePair & Environment.NewLine

            MGR.Instance.LogMessage(msg, m_sTraceFile)
        End Sub

        Friend Sub OpenConnection()
            If Not IsOpen() Then
                If m_Conn IsNot Nothing Then
                    m_Conn.Open()
                End If
            End If
        End Sub

        Friend Sub CloseConnection()
            If m_Conn IsNot Nothing Then
                m_Conn.Close()
                '@@@ refactor with a using statement
                m_Conn.Dispose()
            End If
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    CloseConnection()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
