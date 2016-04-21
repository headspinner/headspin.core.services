Imports headspin.App.Core
Imports headspin.App.Core.Database

Public Class DocBlobMgr

    Public Shared Function GetDocBlobById(ByVal id As Integer) As DocBlob
        Dim d As DocBlob = Nothing

        If id > 0 Then
            d = DocBlob.GetById(id)
        End If

        Return d
    End Function

    'Public Shared Function GetDocBlob(ByVal fkId As Integer, ByVal className As String) As BusinessObjectCollection(Of DocBlob)
    '    Return DocBlob.GetByFKIdAndClassName(fkId, className)
    'End Function

    'Public Shared Function SaveDocBlob(ByVal fkId As Integer, ByVal className As String, ByVal bytes As Byte(), ) As DocBlob
    '    Dim d As DocBlob = Nothing

    '    d = DocBlob.GetByFKIdAndClassName(fkId, className)

    '    Return d
    'End Function

    Public Shared Function GetDocBlob(ByVal bo As BusinessObject) As BusinessObjectCollection(Of DocBlob)
        Dim myDocs = DocBlob.GetByObjIdAndClassName(bo.ObjectId, bo.GetFullyQualifiedClassName())

        If myDocs IsNot Nothing AndAlso myDocs.Count() > 0 Then
            Return myDocs
        End If

        Return Nothing
    End Function


    Public Shared Function GetNewDocBlob(ByVal bytes As Byte(), ByVal fileName As String) As DocBlob
        Dim newBlob As New DocBlob

        newBlob.AttachmentBytes = bytes
        newBlob.AttachmentFileName = fileName


        Return newBlob

    End Function

    Public Shared Sub SaveDocBlob(ByVal bo As BusinessObject, ByVal blobs As List(Of DocBlob), ByVal db As DB, ByVal savingUserName As String)

        If blobs IsNot Nothing AndAlso blobs.Count() > 0 Then
            For Each d In blobs
                d.ClassName = bo.GetFullyQualifiedClassName()
                d.DocBlobFK = bo.ObjectId
                d.TypeCode = Constants.DocBlobType.VarBinaryDoc
                d.StatusCode = Constants.DocBlobStatus.Active

                d.Save(savingUserName, db)
            Next
        End If

    End Sub



End Class
