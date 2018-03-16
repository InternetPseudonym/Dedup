Imports System.IO
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Windows.Forms
Imports C5

Class MainWindow

#Region "member variables"
    Private ReadOnly hashes As New TreeDictionary(Of ComparableByteArrayAndSize, String)
    Private ReadOnly md5 As MD5 = MD5.Create()
    Private Shared exitSignal As Boolean = False
#End Region

#Region "event handler"

    Private Sub windowSizeChanged(sender As Object, evt As SizeChangedEventArgs) Handles Me.SizeChanged

    End Sub

    Private Sub browseClicked() Handles btnFolderBrowse.Click

        Dim dlg As New FolderBrowserDialog()
        dlg.ShowNewFolderButton = False

        dlg.ShowDialog()

        btnFolderBrowse.IsEnabled = False
        tabControl.SelectedItem = tabItemList

        If dlg.SelectedPath IsNot Nothing AndAlso dlg.SelectedPath.Length > 0 Then
            ThreadPool.QueueUserWorkItem(Sub() hashingThread(dlg.SelectedPath))
        End If

    End Sub

    Private Sub onExitButtonPressed() Handles Me.Closed
        exitSignal = True
    End Sub
#End Region

#Region "threads"
    Private Sub hashingThread(path As String)

        Dim hash As Byte() = Nothing
        Dim hashAndSize As ComparableByteArrayAndSize = Nothing
        Dim stream As FileStream = Nothing

        Console.WriteLine("Starting MD5 task ...")

        Dim fileList As IEnumerable(Of String) = Nothing
        Try
            fileList = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
        Catch ex As Exception
            MessageBox.Show("error during filesystem path iteration attempt : \n" & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Dispatcher.Invoke(Sub()
                                  btnFolderBrowse.IsEnabled = True
                                  tabControl.SelectedItem = tabItemFolder
                              End Sub)
            Exit Sub
        End Try

        If fileList Is Nothing Then
            MessageBox.Show("list of selected files cannot be obtained", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Dispatcher.Invoke(Sub()
                                  btnFolderBrowse.IsEnabled = True
                                  tabControl.SelectedItem = tabItemFolder
                              End Sub)
            Exit Sub
        End If

        Dispatcher.Invoke(Sub()
                              progress.Maximum = fileList.Count
                              list.Items.Clear()
                          End Sub)

        For Each file As String In fileList

            Try
                stream = IO.File.OpenRead(file)
            Catch ex As Exception
                Console.Error.WriteLine("file " & file & " cannot be read")
                stream.Close()
                Continue For
            End Try

            Dispatcher.Invoke(Sub() labelFilename.Content = IO.Path.GetFileName(file))


            Try
                hash = md5.ComputeHash(stream)
                hashAndSize = New ComparableByteArrayAndSize(hash, stream.Length)
            Catch e As ObjectDisposedException
                Console.Error.WriteLine("Hash of file " & file & " cannot be computed, file-handle closed")
                Continue For
            Finally
                stream.Close()
            End Try


            Dispatcher.Invoke(Sub() list.Items.Add(New ListViewEntry(hash, file)))
            If (False = hashes.Contains(hashAndSize)) Then
                hashes.Add(hashAndSize, file)
            Else
                Console.WriteLine("DUPLICATE DETECTED : " & IO.Path.GetFileName(file))

            End If

            Dispatcher.Invoke(Sub() progress.Value += 1)


        Next

        Dispatcher.Invoke(Sub()
                              btnFolderBrowse.IsEnabled = True
                              tabControl.SelectedItem = tabItemFolder
                          End Sub)

    End Sub
#End Region

End Class
