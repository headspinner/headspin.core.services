Public Class ConcurrentUpdateException
    Inherits System.Exception

    Public Sub New(ByVal bo As BusinessObject)
        MyBase.New(String.Format( _
                   "The object ({0}) with an Id of ({1}) was updated while you were editing it. Concurrent Update Exception.", _
                   bo.GetType().Name, bo.ObjectId))
    End Sub
End Class
