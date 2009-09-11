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

  Public Shared Function GetFrameFromTiff(ByVal Filename As String, ByVal destPath As String, ByVal FrameNumber As Integer) As String
    Dim fs As FileStream = File.Open(Filename, FileMode.Open, FileAccess.Read)
    Dim bm As System.Drawing.Bitmap = CType(System.Drawing.Bitmap.FromStream(fs), System.Drawing.Bitmap)
    bm.SelectActiveFrame(FrameDimension.Page, FrameNumber)
    Dim temp As New System.Drawing.Bitmap(bm.Width, bm.Height)
    Dim g As Graphics = Graphics.FromImage(temp)
    g.InterpolationMode = InterpolationMode.NearestNeighbor
    g.DrawImage(bm, 0, 0, bm.Width, bm.Height)
    g.Dispose()
    Dim outFileName As String = destPath & Now.Ticks.ToString & ".png"
    temp.Save(outFileName, Drawing.Imaging.ImageFormat.Png)
    fs.Close()
    Return outFileName
  End Function

  Public Shared Function GetImageFrameCount(ByVal sFileName As String, Optional ByVal userPassword As String = "") As Integer
    If ImageUtil.IsPDF(sFileName) Then
      GetImageFrameCount = AFPDFLibUtil.GetPDFPageCount(sFileName, userPassword)
    ElseIf ImageUtil.IsTiff(sFileName) Then
      GetImageFrameCount = GetTiffFrameCount(sFileName)
    End If
  End Function

  Public Shared Function GetTiffFrameCount(ByVal FileName As String) As Integer
    Dim bm As New System.Drawing.Bitmap(FileName)
    GetTiffFrameCount = bm.GetFrameCount(FrameDimension.Page)
    bm.Dispose()
  End Function

  Public Shared Sub DeleteFile(ByVal filename As String)
    Try
      System.IO.File.Delete(filename)
    Catch ex As Exception
    End Try
  End Sub

  Public Shared Function IsTiff(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.tiff*$", RegexOptions.IgnoreCase)
  End Function

  Public Shared Function IsPDF(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.pdf$", RegexOptions.IgnoreCase)
  End Function


End Class
