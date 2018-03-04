Imports System.IO

Public Class ListViewEntry


    Private fname As String, chksum As Byte()

    Public Property Checksum As String
        Get
            Return BitConverter.ToString(chksum)
        End Get
        Set(value As String)
            fname = value
        End Set
    End Property

    Public Property Filename As String
        Get
            Return Path.GetFileName(fname)
        End Get
        Set(value As String)
            fname = value
        End Set
    End Property

    Public Sub New(checksum As Byte(), filename As String)
        chksum = checksum
        fname = filename
    End Sub


End Class
