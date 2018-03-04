Public Class ComparableByteArrayAndSize
    Implements IComparable(Of ComparableByteArrayAndSize)

    Private array As Byte()
    Private size As Long

    Public Sub New(array As Byte(), size As Long)
        Me.array = array
        Me.size = size
    End Sub


    Public Function CompareTo(other As ComparableByteArrayAndSize) As Integer Implements IComparable(Of ComparableByteArrayAndSize).CompareTo
        Return IIf(Me.SequenceEqual(other), 0, Me.size.CompareTo(other.size))
    End Function


    Public Function SequenceEqual(other As ComparableByteArrayAndSize) As Boolean
        Return array.SequenceEqual(other.array)
    End Function

    Public NotOverridable Overrides Function ToString() As String
        Return BitConverter.ToString(Me.array)
    End Function
End Class
