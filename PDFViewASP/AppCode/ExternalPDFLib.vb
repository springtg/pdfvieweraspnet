Imports System.Text.RegularExpressions
Public Class ExternalPDFLib

  Public Shared appName As String = "PDFLibCmdLine.exe"
  Const RENDER_DPI As Integer = 150

  Public Shared Function GetPDFPageCount(ByVal appPath As String, ByVal filepath As String, Optional ByVal userPassword As String = "") As Integer
    GetPDFPageCount = 0
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}""", "count", filepath, userPassword))
    Try
      GetPDFPageCount = CInt(Regex.Replace(myString, "count=", ""))
    Catch ex As Exception
    End Try
  End Function

  Public Shared Function GetPageFromPDF(ByVal appPath As String, ByVal sourceFileName As String _
                                        , ByVal destFolderPath As String _
                                        , ByRef iPageNumber As Integer _
                                        , Optional ByVal DPI As Integer = 0 _
                                        , Optional ByVal password As String = "" _
                                        , Optional ByVal rotations As Integer = 0 _
                                        , Optional ByVal searchText As String = "" _
                                        , Optional ByVal searchDir As Integer = ExternalPDFLib.SearchDirection.FromBeginning _
                                        ) As String
    GetPageFromPDF = ""
    'Usage: pdfcmdline png <filename> <outputdir> <pagenumber> <dpi> <password> <searchtext> <searchdirection>
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}"" {3} {4} ""{5}"" ""{6}"" {7}", "png", sourceFileName, destFolderPath, iPageNumber, DPI, password, searchText, searchDir))
    Try
      Dim myFilePath As String = Regex.Replace(myString, "png=(.+)\npage=.*$", "$1").Trim
      Dim myPageNumString As String = Regex.Replace(myString, "[\d\D]+page=(\d+)[\d\D]*$", "$1")
      ImageUtil.ApplyRotation(myFilePath, rotations)
      GetPageFromPDF = myFilePath
      iPageNumber = CInt(myPageNumString)
    Catch ex As Exception
    End Try
  End Function

  Public Shared Function BuildHTMLBookmarks(ByVal appPath As String, ByVal sourceFileName As String, Optional ByVal pageNumberOnly As Boolean = False) As String
    BuildHTMLBookmarks = ""
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" {2}", "bookmark", sourceFileName, If(pageNumberOnly, 1, 0)))
    Try
      BuildHTMLBookmarks = Regex.Replace(myString, "bookmark=", "")
    Catch ex As Exception
    End Try
  End Function

  Public Shared Function GetOptimalDPI(ByVal appPath As String, ByVal filename As String, ByVal pageNumber As Integer, ByRef oSize As Drawing.Size) As Integer
    GetOptimalDPI = 150
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" {2} {3} {4}", "dpi", filename, pageNumber, oSize.Width, oSize.Height))
    Try
      GetOptimalDPI = CInt(Regex.Replace(myString, "dpi=", ""))
    Catch ex As Exception
    End Try
  End Function

  Public Enum SearchDirection
    FromBeginning
    Backwards
    Forwards
  End Enum
End Class
