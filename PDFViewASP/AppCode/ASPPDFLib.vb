Public Class ASPPDFLib

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
    Return AFPDFLibUtil.GetPDFPageCount(sourceFileName, password)
  End Function

  Public Shared Function IsEncrypted(ByVal sourceFileName As String) As Boolean
    'Return iTextSharpUtil.IsEncrypted(sourceFileName)
  End Function

  Public Shared Function IsPasswordValid(ByVal sourceFileName As String, ByVal password As String) As Boolean
    'Return iTextSharpUtil.IsPasswordValid(sourceFileName, password)
  End Function

#End Region

End Class
