Imports System.IO
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Windows.Forms
Imports System.Windows.Threading
Imports C5

Class MainWindow

        Private ReadOnly hashes As New TreeDictionary(Of ComparableByteArrayAndSize, String)
        Private ReadOnly md5 As MD5 = MD5.Create
        Private Shared ReadOnly remainingFiles As New Queue(Of String)
        Private Shared ReadOnly waitHandle As New EventWaitHandle(False, EventResetMode.AutoReset)
    Private Shared exitSignal As Boolean = False



    Private Sub browseClicked() Handles btnFolderBrowse.Click

            Dim dlg As New FolderBrowserDialog()
            dlg.ShowNewFolderButton = False


            dlg.ShowDialog()

            ThreadPool.QueueUserWorkItem(Sub() mainThread(dlg.SelectedPath))

        End Sub

    Private Sub mainThread(path As String)

        If (path.Length = 0) Then
            Return
        End If


        Dedup.Dispatcher.Invoke(New Action(Sub() progress.IsIndeterminate = True))
        Console.WriteLine("generating queue ...")
        generateQueue(path)
        Console.WriteLine("queue created!")
        Dedup.Dispatcher.Invoke(Sub() progress.IsIndeterminate = False)
        Console.WriteLine("Starting first task ...")
        Task.Factory.StartNew(Sub() calcMd5(remainingFiles.Dequeue()))

        While (False = exitSignal And waitHandle.WaitOne())
            'Console.WriteLine("next thread ...")
            Task.Factory.StartNew(Sub() calcMd5(remainingFiles.Dequeue()))
        End While

    End Sub

    Private Sub generateQueue(path As String)

        If (path.Length = 0) Then
            Return

        End If
        Dim files As IEnumerable(Of String) = Directory.EnumerateFiles(path)

        Console.WriteLine("detected " & files.Count & " files in selected directory")
        Dedup.Dispatcher.Invoke(Sub()
                                    progress.Value = 0
                                    progress.Maximum = files.Count
                                End Sub)

        Dim i As Integer = 1
        For Each file As String In files
            Console.WriteLine("queueing file no. " & i)
            If (remainingFiles.Count > 0) Then
                remainingFiles.Enqueue(file)
            Else
                remainingFiles.Enqueue(file)
            End If

            i += 1
        Next
    End Sub



    Private Sub calcMd5(path As String)
        Dim stream As FileStream = File.Open(path, FileMode.Open)
        Dedup.Dispatcher.Invoke(Sub() labelFilename.Content = IO.Path.GetFileName(path))
        Dim hash As New ComparableByteArrayAndSize(md5.ComputeHash(stream), stream.Length)
        Dim delete As Boolean = False
        If (False = hashes.Contains(hash)) Then
            hashes.Add(hash, path)
        Else
            Console.WriteLine("DUPLICATE DETECTED : " & IO.Path.GetFileName(path))
            delete = True

        End If

        Dedup.Dispatcher.Invoke(Sub() progress.Value += 1)
        stream.Close()
            If delete = True Then
                File.Delete(path)
                Console.WriteLine("... deleted")
            End If
            waitHandle.Set()
        End Sub
    End Class
