
Public Class TblBase
    Public Enum BaseCOLS_New
        CreatedDate
        UpdatedDate
        LockId
        CreatedBy
        UpdatedBy
        SIZE
    End Enum

    Public Class BaseCOLS
        Public Const CreatedDate = "DateAdded"
        Public Const UpdatedDate = "LastEdit"
        Public Const LockId = "LockId"
        Public Const CreatedBy = "AddedBy"
        Public Const UpdatedBy = "EditedBy"

    End Class
End Class



