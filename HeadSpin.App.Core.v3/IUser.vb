Public Interface IUser

    'ReadOnly Property UserId() As Integer

    ReadOnly Property Username() As String

    'ReadOnly Property Password() As String

    Function IsInRole(ByVal role As String) As Boolean

End Interface
