Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions

Public Class Helper

    Public Shared Function GetConfig(ByVal key As String) As String
        Return GetConfig(key, "")
    End Function


    Public Shared Function GetConfig(ByVal key As String, ByVal defaultVal As String) As String

        Dim config As String = System.Configuration.ConfigurationManager.AppSettings(key)

        If Not String.IsNullOrEmpty(config) Then
            Return config
        End If

        Return defaultVal

    End Function


    Public Shared Function GetConfigAsBool(ByVal key As String, ByVal defaultVal As Boolean) As Boolean

        Dim config = GetConfig(key, defaultVal.ToString)

        If config.ToUpper = "TRUE" Then
            Return True
        End If

        Return False
    End Function

    Public Shared Function GetConfigAsInt(ByVal key As String, ByVal defaultVal As Integer) As Integer
        Dim config = GetConfig(key, defaultVal.ToString)

        Dim temp As Integer = 0

        If Integer.TryParse(config, temp) Then
            Return temp
        End If

        Return defaultVal
    End Function


     

    Public Shared Function StringArrayToIntList(arr As String()) As List(Of Integer)
        If arr IsNot Nothing AndAlso arr.Count > 0 Then

            Dim list As New List(Of Integer)

            For Each a In arr
                list.Add(Integer.Parse(a))
            Next

            Return list
        Else
            Return New List(Of Integer)
        End If
    End Function

    Public Shared Function StringArrayToStringList(arr As String()) As List(Of String)
        If arr IsNot Nothing AndAlso arr.Count > 0 Then
            Return arr.ToList()
        Else
            Return New List(Of String)
        End If
    End Function
    Public Shared Function GetStringListFromDelimitedString(ByVal delimitedString As String, ByVal delimiter As String()) As List(Of String)
        Dim list As New List(Of String)

        If Not String.IsNullOrEmpty(delimitedString) Then
            Dim split As String() = delimitedString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
            list.AddRange(split.ToList)
        End If

        Return list
    End Function

    Public Shared Function GetStringListFromCommaSepString(ByVal commaSepString As String) As List(Of String)
        Return GetStringListFromDelimitedString(commaSepString, {","})
    End Function
    Public Shared Function GetIntegerArray(ByVal commaSepString As String) As Integer()
        Dim list As New List(Of Integer)

        If Not String.IsNullOrEmpty(commaSepString) Then
            Dim comma As String() = {","}
            Dim split As String() = commaSepString.Split(comma, StringSplitOptions.RemoveEmptyEntries)

            If split IsNot Nothing AndAlso split.Count > 0 Then
                For Each s As String In split
                    list.Add(Integer.Parse(s))
                Next
            End If
        End If

        Return list.ToArray
    End Function

    Public Shared Function GetCommaSepString(ByVal arr As Integer()) As String
        Dim css As String = String.Empty

        If arr IsNot Nothing AndAlso arr.Count > 0 Then
            For Each s As Integer In arr
                css = css + s.ToString + ","
            Next
        End If

        Return css
    End Function
    Public Shared Function GetFormattedCommaSepString(ByVal arr As String()) As String
        Dim formatted = ""

        If arr IsNot Nothing Then
            For Each a In arr
                If Not String.IsNullOrEmpty(formatted) Then
                    formatted += ", "
                End If
                formatted += a
            Next
        End If
        Return formatted
    End Function

   
    Public Shared Function GetCommaSepString(ByVal arr As String()) As String
        Dim css As String = String.Empty

        If arr IsNot Nothing AndAlso arr.Count > 0 Then
            For Each s As String In arr
                If Not String.IsNullOrEmpty(css) Then
                    css = css + ","
                End If
                css = css + s
            Next
        End If

        Return css
    End Function

    Public Shared Function GetCommaSepString(ByVal arr As List(Of String)) As String
        Dim css As String = String.Empty

        If arr IsNot Nothing AndAlso arr.Count > 0 Then
            css = GetCommaSepString(arr.ToArray)
        End If

        Return css
    End Function
    Public Shared Function ResizeImage(ByVal imgToResize As Image, ByVal size As Size) As Image
        Dim sourceWidth As Integer = imgToResize.Width
        Dim sourceHeight As Integer = imgToResize.Height

        Dim nPercent As Single = 0
        Dim nPercentW As Single = 0
        Dim nPercentH As Single = 0

        nPercentW = (CSng(size.Width) / CSng(sourceWidth))
        nPercentH = (CSng(size.Height) / CSng(sourceHeight))

        If nPercentH < nPercentW Then
            nPercent = nPercentH
        Else
            nPercent = nPercentW
        End If

        Dim destWidth As Integer = CInt(Math.Truncate(sourceWidth * nPercent))
        Dim destHeight As Integer = CInt(Math.Truncate(sourceHeight * nPercent))

        Dim b As New Bitmap(destWidth, destHeight)
        Dim g As Graphics = Graphics.FromImage(DirectCast(b, Image))
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        g.DrawImage(imgToResize, 0, 0, destWidth, destHeight)
        g.Dispose()

        Return DirectCast(b, Image)
    End Function

    'Public Shared Function ResizeImage(ByVal imgToResize As Image, ByVal size As Size) As Image


    '    Dim sourceWidth As Integer = imgToResize.Width
    '    Dim sourceHeight As Integer = imgToResize.Height

    '    Dim nPercent As Decimal = 0
    '    Dim nPercentW As Decimal = 0
    '    Dim nPercentH As Decimal = 0

    '    nPercentW = System.Convert.ToDecimal(size.Width) / System.Convert.ToDecimal(sourceWidth)
    '    nPercentH = System.Convert.ToDecimal(size.Height) / System.Convert.ToDecimal(sourceHeight)

    '    If (nPercentH < nPercentW) Then
    '        nPercent = nPercentH
    '    Else
    '        nPercent = nPercentW
    '    End If


    '          Dim  destWidth as Integer = (int)(sourceWidth * nPercent);
    '          int destHeight = (int)(sourceHeight * nPercent);

    '          Bitmap b = new Bitmap(destWidth, destHeight);
    '          Graphics g = Graphics.FromImage((Image)b);
    '          g.InterpolationMode = InterpolationMode.HighQualityBicubic;

    '          g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
    '          g.Dispose();

    '          return (Image)b;
    'End Function

    Public Shared Function FormatPhoneNumber(phoneNum As String, Optional ByVal phoneFormat As String = "") As String

        If String.IsNullOrWhiteSpace(phoneFormat) Then
            ' Default format is (###) ###-####
            phoneFormat = "(###) ###-####"
        End If

        ' Second, format numbers to phone string 
        If String.IsNullOrWhiteSpace(phoneNum) = False Then
            ' First, remove everything except of numbers
            Dim regexObj As Regex = New Regex("[^\d]")
            phoneNum = regexObj.Replace(phoneNum, "")

            If String.IsNullOrWhiteSpace(phoneNum) = False Then
                phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat)
            End If
        End If

        Return phoneNum
    End Function
    Public Shared Function GetQuarterFromDate(d As DateTime?) As Integer?

        If d.HasValue Then
            Dim quarter As Integer = (d.Value.Month - 1) \ 3 + 1

            Return quarter
        End If

        Return Nothing
    End Function



End Class



