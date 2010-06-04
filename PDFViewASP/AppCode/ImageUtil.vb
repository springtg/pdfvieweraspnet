Imports System.Drawing.Imaging
Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Public Class ImageUtil

  Public Shared Sub ApplyRotation(ByRef fileName As String, ByVal numberOfRotations As Integer)
    Dim img As Bitmap
    If numberOfRotations < 0 Then
      img = New Bitmap(fileName)
      For i As Integer = 1 To Math.Abs(numberOfRotations)
        img.RotateFlip(RotateFlipType.Rotate270FlipNone)
      Next
      img.Save(fileName, Imaging.ImageFormat.Png)
    End If

    If numberOfRotations > 0 Then
      img = New Bitmap(fileName)
      For i As Integer = 1 To numberOfRotations
        img.RotateFlip(RotateFlipType.Rotate90FlipNone)
      Next
      img.Save(fileName, Imaging.ImageFormat.Png)
    End If

  End Sub

  Public Shared Sub DeleteFile(ByVal filename As String)
    Try
      System.IO.File.Delete(filename)
    Catch ex As Exception
    End Try
  End Sub

  Public Shared Function IsPDF(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.pdf$", RegexOptions.IgnoreCase)
  End Function

End Class
