Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Configuration
Imports HeadSpin.App.Core.Database


Public Class AppSvc

    Public Shared Sub Initialize()
        Initialize(True, False)
    End Sub

    Public Shared Sub Initialize(ByVal sqltrace As Boolean)
        Initialize(sqltrace, False)
    End Sub

    Public Shared Sub Initialize(ByVal sqltrace As Boolean, ByVal perflog As Boolean)
        Try
            
            Dim sLog As String = ConfigurationManager.AppSettings("log")
             
            MGR.Instance.Initialize(sLog, False, perflog)

            If sqltrace Then
                Dim sTrace As String = ConfigurationManager.AppSettings("trace")
                DBMgr.Instance.Initialize(ConfigurationManager.ConnectionStrings, sTrace)
            Else
                DBMgr.Instance.Initialize(ConfigurationManager.ConnectionStrings, "")
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub


End Class

