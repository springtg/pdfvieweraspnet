Public Class ASPPDFLib

#Region "Private Variables"
  Private mFileName As String
#End Region



#Region "Public Methods"

  Public Shared Function GetPageFromPDF(ByVal sourceFileName As String _
                                        , ByVal destFolderPath As String _
                                        , ByRef iPageNumber As Integer _
                                        , Optional ByVal DPI As Integer = 0 _
                                        , Optional ByVal password As String = "" _
                                        , Optional ByVal rotations As Integer = 0 _
                                        , Optional ByVal searchText As String = "" _
                                        , Optional ByVal searchDir As Integer = AFPDFLibUtil.SearchDirection.FromBeginning _
                                        ) As String
    GetPageFromPDF = AFPDFLibUtil.GetPageFromPDF(sourceFileName, destFolderPath, iPageNumber, DPI, password, searchText, searchDir)
    ImageUtil.ApplyRotation(GetPageFromPDF, rotations)
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


  Public Shared Function GetImageFromFileGS(ByVal sFileName As String, ByVal destPath As String, ByVal pageNumber As Integer, Optional ByVal DPI As Integer = 0, Optional ByVal password As String = "", Optional ByVal rotations As Integer = 0) As String
    GetImageFromFileGS = Nothing
    If ImageUtil.IsPDF(sFileName) Then 'convert one frame to an image for viewing
      GetImageFromFileGS = ConvertPDF.PDFConvert.GetPageFromPDF(sFileName, destPath, pageNumber, DPI, password)
    ElseIf ImageUtil.IsTiff(sFileName) Then
      GetImageFromFileGS = ImageUtil.GetFrameFromTiff(sFileName, destPath, pageNumber - 1)
    End If
    ImageUtil.ApplyRotation(GetImageFromFileGS, rotations)
  End Function

#End Region

End Class
