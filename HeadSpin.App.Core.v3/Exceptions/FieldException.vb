Public Class FieldException
    Inherits System.Exception

    Public Property Field As String

    Public Sub New(field As String, message As String)
        MyBase.New(message)

        Me.Field = field
    End Sub
End Class
