Imports System.Text.RegularExpressions
Imports System.Drawing

Public Class ExternalPDFLib

  'Thanks to Antonio Sandoval for making the XPDF Interop library PDFLibNet

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

  Public Shared Function GetPDFInfo(ByVal appPath As String, ByVal filepath As String, Optional ByVal userPassword As String = "") As List(Of DictionaryEntry)
    Dim myList As New List(Of DictionaryEntry)
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}""", "info", filepath, userPassword))
    Dim myPass As String = Regex.Replace(myString, "pass=(.+)\ncount=.+bookmark=.*$", "$1", RegexOptions.Singleline).Trim
    Dim myCount As String = Regex.Replace(myString, "pass=(.+)\ncount=(\d+).+bookmark=(.*)$", "$2", RegexOptions.Singleline)
    Dim myBook As String = Regex.Replace(myString, "pass=(.+)\ncount=(\d+).+bookmark=(.*)$", "$3", RegexOptions.Singleline).Trim
    'TODO: populate myList & fix regex match string
    Try
      myList.Add(New DictionaryEntry("pass", CBool(myPass)))
      myList.Add(New DictionaryEntry("count", CInt(myCount)))
      myList.Add(New DictionaryEntry("book", myBook))
    Catch ex As Exception
      Debugger.Break()
    End Try
    Return myList
  End Function

  Public Shared Function GetPageFromPDF(ByVal appPath As String, ByVal sourceFileName As String _
                                        , ByVal destFolderPath As String _
                                        , ByRef iPageNumber As Integer _
                                        , ByVal useMuPDF As Boolean _
                                        , ByVal objSize As Size _
                                        , ByRef DPI As Integer _
                                        , Optional ByVal password As String = "" _
                                        , Optional ByVal rotations As Integer = 0 _
                                        , Optional ByVal searchText As String = "" _
                                        , Optional ByVal searchDir As Integer = ExternalPDFLib.SearchDirection.FromBeginning _
                                        ) As String
    GetPageFromPDF = ""
    'Usage: pdfcmdline png <filename> <outputdir> <pagenumber> <dpi> <password> <searchtext> <searchdirection>
    Dim myString As String = ""
    If objSize <> Size.Empty Then
      myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}"" {3} {4} {5} ""{6}"" ""{7}"" {8} {9}", "pngauto", sourceFileName, destFolderPath, iPageNumber, objSize.Width, objSize.Height, password, searchText, searchDir, If(useMuPDF, 1, 0)))
    Else
      myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}"" {3} {4} ""{5}"" ""{6}"" {7} {8}", "png", sourceFileName, destFolderPath, iPageNumber, DPI, password, searchText, searchDir, If(useMuPDF, 1, 0)))
    End If

    Try
      If Regex.IsMatch(myString, "^authfail", RegexOptions.IgnoreCase) Then
        Return "authfail"
      End If
      Dim myRegex As New Regex("png=(.+).+page=(\d+).*dpi=(\d+).+", RegexOptions.Singleline)
      Dim myFilePath As String = myRegex.Replace(myString, "$1").Trim
      Dim myPageNumString As String = myRegex.Replace(myString, "$2")
      Dim myDPI As String = myRegex.Replace(myString, "$3")
      ImageUtil.ApplyRotation(myFilePath, rotations)
      GetPageFromPDF = myFilePath
      iPageNumber = CInt(myPageNumString)
      DPI = CInt(myDPI)
    Catch ex As Exception
    End Try
  End Function

  'pdflibcmdline bookmark <filename> [<password>] [<pageNumberOnly>]
  Public Shared Function BuildHTMLBookmarks(ByVal appPath As String, ByVal sourceFileName As String, Optional ByVal userPassword As String = "", Optional ByVal pageNumberOnly As Boolean = False) As String
    BuildHTMLBookmarks = ""
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}"" {3}", "bookmark", sourceFileName, userPassword, If(pageNumberOnly, 1, 0)))
    Try
      BuildHTMLBookmarks = Regex.Replace(myString, "bookmark=", "")
    Catch ex As Exception
    End Try
  End Function

  Public Shared Function GetOptimalDPI(ByVal appPath As String, ByVal filename As String, ByVal pageNumber As Integer, ByRef oSize As Drawing.Size, Optional ByVal userPassword As String = "") As Integer
    GetOptimalDPI = 150
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" {2} {3} {4} ""{5}""", "dpi", filename, pageNumber, oSize.Width, oSize.Height, userPassword))
    Try
      GetOptimalDPI = CInt(Regex.Replace(myString, "dpi=", ""))
    Catch ex As Exception
    End Try
  End Function

  Public Shared Function IsPasswordRequired(ByVal appPath As String, ByVal filename As String) As Boolean
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ", "passreq", filename))
    Try
      Return Regex.IsMatch(myString, "True")
    Catch ex As Exception
    End Try
  End Function

  Public Shared Function IsPasswordValid(ByVal appPath As String, ByVal filename As String, ByVal userPassword As String) As Boolean
    Dim myString = CmdHelper.ExecuteCMD(appPath & "\" & appName, String.Format("{0} ""{1}"" ""{2}""", "pass", filename, userPassword))
    Try
      Return Regex.IsMatch(myString, "True")
    Catch ex As Exception
    End Try
  End Function

  Public Enum SearchDirection
    FromBeginning
    Backwards
    Forwards
  End Enum
End Class
