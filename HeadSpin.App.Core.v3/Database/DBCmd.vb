Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient

Namespace Database
    Public Class DBCmd
        Private _command As DbCommand = Nothing

        Friend Sub New(ByVal procedureName As String)
            _command = New System.Data.SqlClient.SqlCommand()

            _command.CommandText = procedureName

            _command.CommandType = CommandType.StoredProcedure
        End Sub

        Friend Function GetCommand() As DbCommand
            Return _command
        End Function

        Private Function GetParam() As SqlParameter
            Dim parm As New SqlParameter()
            parm.IsNullable = True
            parm.Value = DBNull.Value
            Return parm

        End Function

        'Public Sub AddParam(ByVal sColumn As String, ByVal bytes As EncryptableBytes)
        '    Dim parm As SqlParameter = Me.GetParam()
        '    parm.ParameterName = "@" & sColumn
        '    parm.SqlDbType = SqlDbType.VarBinary

        '    If (bytes IsNot Nothing) AndAlso (bytes.Encrypted IsNot Nothing) Then
        '        parm.Value = bytes.Encrypted
        '    End If

        '    _command.Parameters.Add(parm)
        'End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal guid As Guid)
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.UniqueIdentifier

            If Not String.IsNullOrEmpty(guid.ToString) Then
                parm.Value = guid
            End If


            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal byteArr As Byte())
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.VarBinary

            If byteArr IsNot Nothing Then
                parm.Value = byteArr
            End If


            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal nVal As Integer)
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.Int

            parm.Value = nVal

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal dtVal As System.Nullable(Of DateTime))
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.DateTime

            If dtVal.HasValue Then
                parm.Value = dtVal
            End If

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal nVal As System.Nullable(Of Boolean))
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.Bit

            If nVal.HasValue Then
                parm.Value = nVal
            End If

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal nVal As System.Nullable(Of Integer))
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.Int

            If nVal.HasValue Then
                parm.Value = nVal
            End If

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal nVal As System.Nullable(Of Decimal))
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.[Decimal]

            If nVal.HasValue Then
                parm.Value = nVal
            End If

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal nVal As Boolean)
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.Bit

            parm.Value = nVal

            _command.Parameters.Add(parm)
        End Sub

        'Public Sub AddParamN(ByVal sColumn As String, ByVal sVal As String)
        '    Dim parm As SqlParameter = Me.GetParam()
        '    parm.ParameterName = "@" & sColumn
        '    parm.SqlDbType = SqlDbType.NVarChar

        '    If sVal IsNot Nothing Then
        '        parm.Value = sVal
        '    End If

        '    _command.Parameters.Add(parm)
        'End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal sVal As String)
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            'parm.SqlDbType = SqlDbType.NVarChar

            If sVal IsNot Nothing Then
                parm.Value = sVal
            End If

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal nVal As Decimal)
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.[Decimal]
            parm.Value = nVal

            _command.Parameters.Add(parm)
        End Sub

        Public Sub AddParam(ByVal sColumn As String, ByVal dateVal As DateTime)
            Dim parm As SqlParameter = Me.GetParam()
            parm.ParameterName = "@" & sColumn
            parm.SqlDbType = SqlDbType.DateTime

            If dateVal <> Nothing Then
                parm.Value = dateVal
            End If

            _command.Parameters.Add(parm)
        End Sub
    End Class
End Namespace
