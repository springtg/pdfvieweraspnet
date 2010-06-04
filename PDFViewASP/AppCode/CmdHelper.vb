Imports System.Threading

Public Class CmdHelper

  'This will protect the server from runaway processes
  'If a pdf page cannot render by this time, then abort the rendering
  Const CmdTimeout As Integer = 30

  Public Shared Function ExecuteCMD(ByVal cmd As String, ByVal args As String) As String
    ExecuteCMD = ""
    Using myProcess As New Process()
      myProcess.StartInfo.FileName = cmd
      myProcess.StartInfo.Arguments = args
      myProcess.StartInfo.UseShellExecute = False
      myProcess.StartInfo.RedirectStandardOutput = True
      myProcess.StartInfo.CreateNoWindow = True
      myProcess.Start()
      ThreadedKill(myProcess.Id)
      myProcess.PriorityClass = ProcessPriorityClass.Normal
      ExecuteCMD = myProcess.StandardOutput.ReadToEnd
      myProcess.WaitForExit()
    End Using
  End Function

  Public Shared Sub KillProcessAfterTimeout(ByVal pid As Integer)
    Dim pProcess As Process = System.Diagnostics.Process.GetProcessById(pid)
    Dim expiration As DateTime = Now.AddSeconds(CmdTimeout)
    While Now < expiration
      Thread.Sleep(100)
      If Nothing Is pProcess Then
        Exit Sub
      End If
    End While
    If pProcess IsNot Nothing Then
      Try
        pProcess.Kill()
      Catch ex As Exception
      End Try
    End If
  End Sub

  Public Shared Sub ThreadedKill(ByVal pid As Integer)
    Dim myThread As New Thread(New ParameterizedThreadStart(AddressOf KillProcessAfterTimeout))
    myThread.Start(pid)
  End Sub

End Class
