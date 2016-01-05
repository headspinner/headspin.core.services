Imports System.IO

 
        ''' <summary>
        ''' Should support loggin to file etc.
        ''' </summary>
        Public Class cMsg
            Inherits Exception
            Private m_sText As String
            Private m_sLogFile As String
            Private m_nMsgNo As Integer = 0

            Public Sub New()
            End Sub
            Public Sub New(ByVal sMsg As String)
                Me.Text = sMsg
            End Sub
            Public Sub New(ByVal sMsg As String, ByVal nMsgNo As Integer)
                Me.Text = sMsg
                Me.Number = nMsgNo
            End Sub

            Public Sub New(ByVal sMsg As String, ByVal sLogFile As String)
                Me.Text = sMsg
                m_sLogFile = sLogFile
            End Sub

            Public Sub Log()
                MGR.Instance.LogMessage(Me.Text, m_sLogFile)
            End Sub

            Public Property Text() As String
                Get
                    Return m_sText
                End Get
                Set(ByVal value As String)
                    m_sText = value
                End Set
            End Property
            Public Property Number() As Integer
                Get
                    Return m_nMsgNo
                End Get
                Set(ByVal value As Integer)
                    m_nMsgNo = value
                End Set
            End Property

        End Class
