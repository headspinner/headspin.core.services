Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports HeadSpin.App.Core.Database


Public Class BusinessObjectCollection(Of T As BusinessObject)
    Inherits List(Of T)

   

    Public Function IsDirty() As Boolean
        For Each bo As T In Me
            If bo.IsDirty() Then
                Return True
            End If
        Next
        Return False
    End Function

    Private _AuthUser As IUser = Nothing
    Public Property AuthUser() As IUser
        Get
            Return _AuthUser
        End Get
        Set(ByVal value As IUser)
            _AuthUser = value
        End Set
    End Property

    Public Sub MarkForRemoval()
        For Each bo As T In Me
            bo.MarkForRemoval()
        Next
    End Sub
    Public Sub Delete(ByVal db As DB)
        For Each bo As T In Me
            bo.MarkForRemoval()
            bo.Save(db)
        Next
    End Sub

    Public Sub Delete()
        Using db As DB = DBMgr.Instance.GetDB()
            Try
                db.BeginTransaction()
                Delete(db)
                db.CommitTransaction()
            Catch ex As Exception
                db.RollbackTransaction()
                MGR.Instance.LogException(ex)
                Throw
            End Try
        End Using
    End Sub

    Public Sub Save(ByVal db As DB)
        For Each bo As T In Me

            If Me.AuthUser IsNot Nothing Then
                If bo.IsNew Then
                    bo.CreatedBy = Me.AuthUser.Username
                Else
                    bo.UpdatedBy = Me.AuthUser.Username
                End If
            End If

            bo.Save(db)
        Next
    End Sub

    Public Sub Save(username As String)
        Using db As DB = DBMgr.Instance.GetDB()

            Try
                db.BeginTransaction()

                For Each bo As T In Me

                    If bo.IsNew Then
                        bo.CreatedBy = username
                    Else
                        bo.UpdatedBy = username
                    End If

                    bo.Save(db)
                Next

                db.CommitTransaction()
            Catch e As Exception
                db.RollbackTransaction()
                Throw
            End Try
        End Using
    End Sub

    Public Sub Save()
        Using db As DB = DBMgr.Instance.GetDB()

            Try
                db.BeginTransaction()

                For Each bo As T In Me

                    If Me.AuthUser IsNot Nothing Then
                        If bo.IsNew Then
                            bo.CreatedBy = Me.AuthUser.Username
                        Else
                            bo.UpdatedBy = Me.AuthUser.Username
                        End If
                    End If

                    bo.Save(db)
                Next

                db.CommitTransaction()
            Catch e As Exception
                db.RollbackTransaction()
                Throw e
            End Try
        End Using
    End Sub
End Class

