Imports System.Data
Imports System.Data.Common
'@@@ common would be
Imports System.Data.SqlClient

Namespace Database
    
    Public Class DBReader
        Implements IDisposable

        ''' <summary>
        ''' 
        ''' </summary>
        Protected Overrides Sub Finalize()
            Try
                Dispose(False)
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        Private m_bIsDisposed As Boolean = False

        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="bDisposing"></param>
        Protected Overridable Sub Dispose(ByVal bDisposing As Boolean)
            If Not m_bIsDisposed Then
                Close()

                m_bIsDisposed = True
            End If
        End Sub

        ' A DataRow Collection the way we implement
        ' the db queries will be a series of rows.
        ' Sort of like an in memory table of the query
        ' result... complete w/repeating rows.

        Public ReadOnly Property ColumnCount() As Integer
            Get
                Return m_DataReader.VisibleFieldCount
            End Get
        End Property
        '@@@ better to use common here...
        Private m_DataReader As DbDataReader

        'private int m_nColIndex = 0;
        Private _colIdx As Integer = 0

        Private ReadOnly Property ColIndex() As Integer
            Get
                Return _colIdx
            End Get
        End Property
        Private Sub IncrementColIndex()
            _colIdx += 1
        End Sub
        Private Sub ResetColIndex()
            _colIdx = 0
        End Sub

        Public Sub New(ByVal dataReader As DbDataReader)
            ' Set our member
            m_DataReader = dataReader
        End Sub


        Public ReadOnly Property HasRows() As Boolean
            Get
                If m_DataReader.HasRows Then
                    Return True
                Else
                    Close()
                    Return False
                End If
            End Get
        End Property

        Public Function PeekString(ByVal columnOrdinal As Integer) As String
            Dim s As String = Nothing

            If Not m_DataReader.IsDBNull(columnOrdinal) Then
                s = m_DataReader.GetString(columnOrdinal)
            End If

            Return s
        End Function

#Region "Get_X"
        'Public Function GetEncryptedBytes() As EncryptedBytes
        '    Dim enc As New EncryptedBytes(DirectCast(m_DataReader, SqlDataReader).GetSqlBytes(Me.ColIndex).Buffer)
        '    IncrementColIndex()
        '    Return enc
        'End Function
        Public Function GetXML() As System.Xml.Linq.XDocument

            Dim sqlXml As System.Data.SqlTypes.SqlXml = DirectCast(m_DataReader, SqlDataReader).GetSqlXml(Me.ColIndex)
            IncrementColIndex()

            If sqlXml.IsNull Then
                Return Nothing
            Else
                Return XDocument.Load(sqlXml.CreateReader)
            End If
            
        End Function

        Public Function GetByteArray() As Byte()
            'DateTime dt = m_DataReader.GetBytes(this.ColIndex,0,,0,size);
            Dim byteArr As Byte() = DirectCast(m_DataReader, SqlDataReader).GetSqlBytes(Me.ColIndex).Buffer
            IncrementColIndex()
            Return byteArr
        End Function

        Public Function GetNullableDateTime() As System.Nullable(Of DateTime)
            Dim i As System.Nullable(Of DateTime) = Nothing

            If Not m_DataReader.IsDBNull(Me.ColIndex) Then
                i = m_DataReader.GetDateTime(Me.ColIndex)
            End If
            IncrementColIndex()
            Return i
        End Function

        Public Function GetDateTime() As DateTime
            Dim dt As DateTime = m_DataReader.GetDateTime(Me.ColIndex)
            IncrementColIndex()
            Return dt

        End Function
        Public Function GetString() As String
            Dim s As String = Nothing
            If Not m_DataReader.IsDBNull(Me.ColIndex) Then
                s = m_DataReader.GetString(Me.ColIndex)
            End If
            IncrementColIndex()
            Return s
        End Function

        Public Function GetDecimal() As Decimal
            Dim d As Decimal = m_DataReader.GetDecimal(Me.ColIndex)
            IncrementColIndex()
            Return d
        End Function

        Public Function GetNullableDecimal() As System.Nullable(Of Decimal)
            Dim i As System.Nullable(Of Decimal) = Nothing

            If Not m_DataReader.IsDBNull(Me.ColIndex) Then
                i = m_DataReader.GetDecimal(Me.ColIndex)
            End If
            IncrementColIndex()
            Return i
        End Function

        Public Function GetNullableInteger() As System.Nullable(Of Integer)
            Dim i As System.Nullable(Of Integer) = Nothing

            If Not m_DataReader.IsDBNull(Me.ColIndex) Then
                i = m_DataReader.GetInt32(Me.ColIndex)
            End If
            IncrementColIndex()
            Return i
        End Function
        Public Function GetInteger() As Integer

            Dim i As Integer = m_DataReader.GetInt32(Me.ColIndex)
            IncrementColIndex()
            Return i
        End Function

        Public Function GetLong() As Int64

            Dim i As Int64 = m_DataReader.GetInt64(Me.ColIndex)
            IncrementColIndex()
            Return i
        End Function

        Public Function GetBoolean() As Boolean
            Dim i As Boolean = m_DataReader.GetBoolean(Me.ColIndex)
            IncrementColIndex()
            Return i
        End Function

        Public Function GetNullableBoolean() As Boolean?
            Dim i As Boolean? = m_DataReader.GetBoolean(Me.ColIndex)
            IncrementColIndex()
            Return i
        End Function
#End Region


#Region "Get_X_idx"


        'Public Function GetEncryptedBytes(ByVal colIdx As Integer) As EncryptedBytes
        '    Dim enc As New EncryptedBytes(DirectCast(m_DataReader, SqlDataReader).GetSqlBytes(colIdx).Buffer)

        '    Return enc
        'End Function
        Public Function GetXML(ByVal colIdx As Integer) As System.Xml.Linq.XDocument

            Dim sqlXml As System.Data.SqlTypes.SqlXml = DirectCast(m_DataReader, SqlDataReader).GetSqlXml(colIdx)

            If sqlXml.IsNull Then
                Return Nothing
            Else
                Return XDocument.Load(sqlXml.CreateReader)
            End If
        End Function

        Public Function GetByteArray(ByVal colIdx As Integer) As Byte()
            'DateTime dt = m_DataReader.GetBytes(this.ColIndex,0,,0,size);
            Dim byteArr As Byte() = DirectCast(m_DataReader, SqlDataReader).GetSqlBytes(colIdx).Buffer

            Return byteArr
        End Function

        Public Function GetNullableDateTime(ByVal colIdx As Integer) As System.Nullable(Of DateTime)
            Dim i As System.Nullable(Of DateTime) = Nothing

            If Not m_DataReader.IsDBNull(colIdx) Then
                i = m_DataReader.GetDateTime(colIdx)
            End If

            Return i
        End Function

        Public Function GetDateTime(ByVal colIdx As Integer) As DateTime
            Dim dt As DateTime = m_DataReader.GetDateTime(colIdx)

            Return dt

        End Function
        Public Function GetString(ByVal colIdx As Integer) As String
            Dim s As String = Nothing
            If Not m_DataReader.IsDBNull(colIdx) Then
                s = m_DataReader.GetString(colIdx)
            End If

            Return s
        End Function

        Public Function GetGuid(ByVal colIdx As Integer) As Guid
            Dim g As Guid = Nothing
            If Not m_DataReader.IsDBNull(colIdx) Then
                g = m_DataReader.GetGuid(colIdx)
            End If

            Return g
        End Function

        Public Function GetDecimal(ByVal colIdx As Integer) As Decimal
            Dim d As Decimal = m_DataReader.GetDecimal(colIdx)

            Return d
        End Function

        Public Function GetNullableDecimal(ByVal colIdx As Integer) As System.Nullable(Of Decimal)
            Dim i As System.Nullable(Of Decimal) = Nothing

            If Not m_DataReader.IsDBNull(colIdx) Then
                i = m_DataReader.GetDecimal(colIdx)
            End If

            Return i
        End Function

        Public Function GetNullableInteger(ByVal colIdx As Integer) As System.Nullable(Of Integer)
            Dim i As System.Nullable(Of Integer) = Nothing

            If Not m_DataReader.IsDBNull(colIdx) Then
                i = m_DataReader.GetInt32(colIdx)
            End If

            Return i
        End Function
        Public Function GetInteger(ByVal colIdx As Integer) As Integer

            Dim i As Integer = m_DataReader.GetInt32(colIdx)

            Return i
        End Function

        Public Function GetLong(ByVal colIdx As Integer) As Int64

            Dim i As Int64 = m_DataReader.GetInt64(colIdx)

            Return i
        End Function

        Public Function GetBoolean(ByVal colIdx As Integer) As Boolean
            Dim i As Boolean = m_DataReader.GetBoolean(colIdx)
            Return i
        End Function

        Public Function GetNullableBoolean(ByVal colIdx As Integer) As Boolean?
            Dim i As Boolean? = Nothing

            If Not m_DataReader.IsDBNull(colIdx) Then
                i = m_DataReader.GetBoolean(colIdx)
            End If

            Return i
        End Function

        Public ReadOnly Property GetColumnName(ByVal colIdx As Integer) As String
            Get
                Return m_DataReader.GetName(colIdx)
            End Get
        End Property


#End Region

#Region "get_by_col_name"

        'Public Function GetEncryptedBytes(ByVal colName As String) As EncryptedBytes
        '    Return GetEncryptedBytes(m_DataReader.GetOrdinal(colName))
        'End Function

        Public Function GetXML(ByVal colName As String) As System.Xml.Linq.XDocument
            Return GetXML(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetByteArray(ByVal colName As String) As Byte()
            Return GetByteArray(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetNullableDateTime(ByVal colName As String) As System.Nullable(Of DateTime)
            Return GetNullableDateTime(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetDateTime(ByVal colName As String) As DateTime
            Return GetDateTime(m_DataReader.GetOrdinal(colName))
        End Function
        Public Function GetString(ByVal colName As String) As String
            Return GetString(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetGuid(ByVal colName As String) As Guid
            Return GetGuid(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetDecimal(ByVal colName As String) As Decimal
            Return GetDecimal(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetNullableDecimal(ByVal colName As String) As System.Nullable(Of Decimal)
            Return GetNullableDecimal(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetNullableInteger(ByVal colName As String) As System.Nullable(Of Integer)
            Return GetNullableInteger(m_DataReader.GetOrdinal(colName))
        End Function
        Public Function GetInteger(ByVal colName As String) As Integer
            Return GetInteger(m_DataReader.GetOrdinal(colName))
        End Function
        Public Function GetLong(ByVal colName As String) As Int64
            Return GetLong(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetBoolean(ByVal colName As String) As Boolean
            Return GetBoolean(m_DataReader.GetOrdinal(colName))
        End Function

        Public Function GetNullableBoolean(ByVal colName As String) As Boolean?
            Return GetNullableBoolean(m_DataReader.GetOrdinal(colName))
        End Function


#End Region


        Public Function Read() As Boolean
            Dim bRead As Boolean = False

            If m_DataReader IsNot Nothing Then
                ResetColIndex()

                bRead = m_DataReader.Read()

                If Not bRead Then
                    Close()
                End If
            End If

            Return bRead
        End Function

        Public Sub Close()
            If m_DataReader IsNot Nothing Then
                m_DataReader.Close()
            End If
        End Sub

    End Class
End Namespace
