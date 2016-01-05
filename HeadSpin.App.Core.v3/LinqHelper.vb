Imports System.Linq.Expressions

Public Module PredicateBuilder2
    Public Function [True](Of T)() As Expression(Of Func(Of T, Boolean))
        Return Function(f) True
    End Function

    Public Function [False](Of T)() As Expression(Of Func(Of T, Boolean))
        Return Function(f) False
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function [Or](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
        Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[Or](expr1.Body, invokedExpr), expr1.Parameters)
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function [And](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
        Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[And](expr1.Body, invokedExpr), expr1.Parameters)
    End Function
End Module
