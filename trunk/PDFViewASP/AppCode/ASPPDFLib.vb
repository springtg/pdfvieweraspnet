Public Class ASPPDFLib

#Region "Private Variables"
  Private mFileName As String
#End Region

#Region "Public Methods"

  Public Shared Function GetPageFromPDF(ByVal sourceFileName As String, ByVal destFolderPath As String, ByVal iPageNumber As Integer, Optional ByVal DPI As Integer = 0, Optional ByVal password As String = "", Optional ByVal rotations As Integer = 0) As String
    Dim aspPDF As New ASPPDFLib
    GetPageFromPDF = aspPDF.GetImageFromFile(sourceFileName, destFolderPath, iPageNumber - 1, DPI, password, rotations)
    aspPDF = Nothing
  End Function

  Public Shared Function GetPageCount(ByVal sourceFileName As String, Optional ByVal password As String = "") As Integer
    Return iTextSharpUtil.GetPDFPageCount(sourceFileName, password)
  End Function

  Public Shared Function IsEncrypted(ByVal sourceFileName As String) As Boolean
    Return iTextSharpUtil.IsEncrypted(sourceFileName)
  End Function

  Public Shared Function IsPasswordValid(ByVal sourceFileName As String, ByVal password As String) As Boolean
    Return iTextSharpUtil.IsPasswordValid(sourceFileName, password)
  End Function

  Public Shared Function GetWebBookmarkTree(ByVal sourceFileName As String, ByVal password As String, ByVal treeViewCol As Web.UI.WebControls.TreeNodeCollection) As Boolean
    Return iTextSharpUtil.BuildASPBookmarkTreeFromPDF(sourceFileName, treeViewCol, password)
  End Function

#End Region

#Region "Private Methods"

  Private Function GetImageFromFile(ByVal sFileName As String, ByVal destPath As String, ByVal iFrameNumber As Integer, Optional ByVal DPI As Integer = 0, Optional ByVal password As String = "", Optional ByVal rotations As Integer = 0) As String
    GetImageFromFile = Nothing
    If ImageUtil.IsPDF(sFileName) Then 'convert one frame to an image for viewing
      GetImageFromFile = ConvertPDF.PDFConvert.GetPageFromPDF(sFileName, destPath, iFrameNumber + 1, DPI, password)
    ElseIf ImageUtil.IsTiff(sFileName) Then
      GetImageFromFile = ImageUtil.GetFrameFromTiff(sFileName, destPath, iFrameNumber)
    End If
    ImageUtil.ApplyRotation(GetImageFromFile, rotations)
  End Function

#End Region

End Class
