Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Text.RegularExpressions

Public Class PDFLibHelper

  Const RENDER_DPI As Integer = 150
  Public Const BAD_PASSWORD As String = "authfail"

  Public Shared Function IsPasswordRequired(ByVal filepath As String) As Boolean
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    Try
      pdfDoc.LoadPDF(filepath)
    Catch ex As System.Security.SecurityException
      Return True
    Catch ex As Exception
      'Do nothing
    Finally
      pdfDoc.Dispose()
    End Try
    Return False
  End Function

  Public Shared Function IsPasswordValid(ByVal filepath As String, ByVal userPassword As String) As Boolean
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    Try
      pdfDoc.UserPassword = userPassword
      pdfDoc.LoadPDF(filepath)
    Catch ex As System.Security.SecurityException
      Return False
    Catch ex As Exception
      'Do nothing
    Finally
      pdfDoc.Dispose()
    End Try
    Return True
  End Function

  Public Shared Function GetPDFPageCount(ByVal filepath As String, Optional ByVal userPassword As String = "") As Integer
    Dim myPDFDocState As PDFDocState = GetPDFDoc(filepath, userPassword)
    If myPDFDocState.RequiresPassword = True Then
      If Not Nothing Is myPDFDocState.PDFDoc Then
        myPDFDocState.PDFDoc.Dispose()
      End If
      Return -1
    End If
    If Not Nothing Is myPDFDocState.PDFDoc And myPDFDocState.RequiresPassword = False Then
      GetPDFPageCount = myPDFDocState.PDFDoc.PageCount
    Else
      GetPDFPageCount = 0
    End If
    myPDFDocState.PDFDoc.Dispose()
  End Function

  Public Shared Function GetSearchResults(ByVal filename As String, ByVal searchText As String, Optional ByVal Password As String = "", Optional ByVal pageNum As Integer = 1, Optional ByVal searchDir As SearchDirection = SearchDirection.FromBeginning) As List(Of PDFLibNet.PDFSearchResult)
    GetSearchResults = New List(Of PDFLibNet.PDFSearchResult)
    Dim myPDFDocState As PDFDocState = GetPDFDoc(filename, Password)
    If Not Nothing Is myPDFDocState.PDFDoc And myPDFDocState.RequiresPassword = False Then
      Dim lFound As Integer = 0
      lFound = myPDFDocState.PDFDoc.FindText(searchText, pageNum, PDFLibNet.PDFSearchOrder.PDFSearchFromCurrent, False, If(searchDir = SearchDirection.Backwards, True, False), False, False, False, True)
      If lFound > 0 Then
        Return myPDFDocState.PDFDoc.SearchResults
      End If
      myPDFDocState.PDFDoc.Dispose()
    End If
  End Function

  Public Shared Function GetPageFromPDF(ByVal filename As String, ByVal destPath As String, ByRef PageNumber As Integer, Optional ByVal DPI As Integer = RENDER_DPI, Optional ByVal Password As String = "", Optional ByVal searchText As String = "", Optional ByVal searchDir As SearchDirection = 0) As String
    GetPageFromPDF = ""
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    pdfDoc.RenderDPI = 72
    If Password <> "" Then
      pdfDoc.UserPassword = Password
    End If
    Try
      pdfDoc.LoadPDF(filename)
    Catch ex As System.Security.SecurityException
      Return BAD_PASSWORD
    Catch ex As Exception
      Exit Function
    End Try
    If Not Nothing Is pdfDoc Then
      pdfDoc.CurrentPage = PageNumber
      Dim searchResults As New List(Of PDFLibNet.PDFSearchResult)
      If searchText <> "" Then
        If searchDir = SearchDirection.Backwards Then
          If (PageNumber - 1) >= 1 Then
            searchResults = GetSearchResults(filename, searchText, Password, PageNumber - 1, searchDir)
          Else
            searchResults = GetSearchResults(filename, searchText, Password, 1, searchDir)
          End If
        ElseIf searchDir = SearchDirection.Forwards Then
          If (PageNumber + 1) <= pdfDoc.PageCount Then
            searchResults = GetSearchResults(filename, searchText, Password, PageNumber + 1, searchDir)
          Else
            searchResults = GetSearchResults(filename, searchText, Password, pdfDoc.PageCount, searchDir)
          End If
        Else
          searchResults = GetSearchResults(filename, searchText, Password, 1, searchDir)
        End If
      End If
      If Not Nothing Is searchResults Then
        If searchResults.Count > 0 Then
          PageNumber = searchResults(0).Page
        End If
      End If
      Dim outGuid As Guid = Guid.NewGuid()
      Dim output As String = destPath & "\" & outGuid.ToString & ".png"
      Dim pdfPage As PDFLibNet.PDFPage = pdfDoc.Pages(PageNumber)
      Dim bmp As Bitmap = pdfPage.GetBitmap(DPI, True)
      If searchResults.Count > 0 Then
        HighlightSearchCriteria(bmp, DPI, searchResults, PageNumber)
      End If
      bmp.Save(output, System.Drawing.Imaging.ImageFormat.Png)
      bmp.Dispose()
      GetPageFromPDF = output
      pdfDoc.Dispose()
    End If
  End Function

  Public Shared Function GetPageFromPDFNoSearch(ByVal filename As String, ByVal destPath As String, ByRef PageNumber As Integer, Optional ByVal DPI As Integer = RENDER_DPI, Optional ByVal Password As String = "") As String
    GetPageFromPDFNoSearch = ""
    Dim myPDFDocState As PDFDocState = GetPDFDoc(filename, Password)
    If Not Nothing Is myPDFDocState.PDFDoc And myPDFDocState.RequiresPassword = False Then
      Dim outGuid As Guid = Guid.NewGuid()
      Dim output As String = destPath & "\" & outGuid.ToString & ".jpg"
      myPDFDocState.PDFDoc.ExportJpg(output, PageNumber, PageNumber, DPI, 90)
      While (myPDFDocState.PDFDoc.IsJpgBusy)
        Threading.Thread.Sleep(50)
      End While
      myPDFDocState.PDFDoc.Dispose()
      GetPageFromPDFNoSearch = output
    End If
  End Function

  Public Shared Sub HighlightSearchCriteria(ByRef bmp As Drawing.Bitmap, ByVal DPI As Integer, ByRef searchResults As List(Of PDFLibNet.PDFSearchResult), ByVal pageNumber As Integer)

    Dim gBmp As Graphics = Graphics.FromImage(bmp)
    Dim scale As Single = DPI / 72

    ' draw a blue rectangle to the bitmap in memory
    Dim blue As Color = Color.FromArgb(&H40, 0, 0, &HFF)
    Dim blueBrush As Brush = New SolidBrush(blue)

    For Each searchItem In searchResults
      If searchItem.Page = pageNumber Then
        gBmp.FillRectangle(blueBrush, searchItem.Position.X * scale, searchItem.Position.Y * scale, searchItem.Position.Width * scale, searchItem.Position.Height * scale)
      End If
    Next

    gBmp.Dispose()
    blueBrush.Dispose()

  End Sub

  Public Shared Function GetOptimalDPI(ByVal filename As String, ByVal pageNumber As Integer, ByRef oSize As Drawing.Size, Optional ByVal Password As String = "") As Integer
    GetOptimalDPI = 0
    Dim myPDFDocState As PDFDocState = GetPDFDoc(filename, Password)
    If Not Nothing Is myPDFDocState.PDFDoc And myPDFDocState.RequiresPassword = False Then
      If myPDFDocState.PDFDoc.Pages(pageNumber).Width > 0 And myPDFDocState.PDFDoc.Pages(pageNumber).Height > 0 Then
        Dim picHeight As Integer = oSize.Height
        Dim picWidth As Integer = oSize.Width
        Dim docHeight As Integer = myPDFDocState.PDFDoc.Pages(pageNumber).Height
        Dim docWidth As Integer = myPDFDocState.PDFDoc.Pages(pageNumber).Width
        Dim HScale As Single = oSize.Width / docWidth
        Dim VScale As Single = oSize.Height / docHeight
        If VScale > HScale Then
          GetOptimalDPI = Math.Floor(253 * HScale)
        Else
          GetOptimalDPI = Math.Floor(253 * VScale)
        End If
      End If
      myPDFDocState.PDFDoc.Dispose()
    End If
  End Function

  Public Shared Function BuildHTMLBookmarks(ByVal filename As String, Optional ByVal Password As String = "", Optional ByVal pageNumberOnly As Boolean = False) As String

    Dim myPDFDocState As PDFDocState = GetPDFDoc(filename, Password)
    If Nothing Is myPDFDocState.PDFDoc Or myPDFDocState.RequiresPassword = True Then
      Return ""
    End If

    If pageNumberOnly = True Then
      GoTo StartPageList
    End If

    If myPDFDocState.PDFDoc.Outline.Count <= 0 Then
StartPageList:
      BuildHTMLBookmarks = "<!--PageNumberOnly--><ul>"
      For i As Integer = 1 To myPDFDocState.PDFDoc.PageCount
        BuildHTMLBookmarks &= "<li><a href=""javascript:changePage('" & i & "')"">Page " & i & "</a></li>"
      Next
      BuildHTMLBookmarks &= "</ul>"
      Exit Function
    Else
      BuildHTMLBookmarks = ""
      FillHTMLTreeRecursive(myPDFDocState.PDFDoc.Outline, BuildHTMLBookmarks, myPDFDocState.PDFDoc)
      If System.Text.RegularExpressions.Regex.IsMatch(BuildHTMLBookmarks, "\d") = False Then
        BuildHTMLBookmarks = ""
        GoTo StartPageList
      End If
      Exit Function
    End If

  End Function

  Public Shared Function BuildHTMLBookmarksFromSearchResults(ByVal searchResults As List(Of PDFLibNet.PDFSearchResult)) As String
    BuildHTMLBookmarksFromSearchResults = "<!--SearchResults--><ul>"
    For Each item As PDFLibNet.PDFSearchResult In searchResults
      BuildHTMLBookmarksFromSearchResults &= "<li><a href=""javascript:changePage('" & item.Page & "')"">Page " & item.Page & " (position: " & item.Position.Location.ToString & "</a></li>"
    Next
    BuildHTMLBookmarksFromSearchResults &= "</ul>"
  End Function


  Public Shared Sub FillHTMLTreeRecursive(ByVal olParent As PDFLibNet.OutlineItemCollection(Of PDFLibNet.OutlineItem), ByRef htmlString As String, ByRef pdfDoc As PDFLibNet.PDFWrapper)
    htmlString &= "<ul>"
    For Each ol As PDFLibNet.OutlineItem In olParent
      htmlString &= "<li><a href=""javascript:changePage('" & ol.Destination.Page & "')"">" & Web.HttpUtility.HtmlEncode(ol.Title) & "</a></li>"
      If ol.KidsCount > 0 Then
        FillHTMLTreeRecursive(ol.Childrens, htmlString, pdfDoc)
      End If
    Next
    htmlString &= "</ul>"
  End Sub

  Public Enum SearchDirection
    FromBeginning
    Backwards
    Forwards
  End Enum

  Public Shared Function IsTiff(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.tiff*$", RegexOptions.IgnoreCase)
  End Function

  Public Shared Function IsPDF(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.pdf$", RegexOptions.IgnoreCase)
  End Function

  Public Shared Function GetPDFDoc(ByVal filename As String, Optional ByVal password As String = "") As PDFDocState
    Dim myPdfDocState As New PDFDocState
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    myPdfDocState.PDFDoc = pdfDoc
    If password <> "" Then
      myPdfDocState.PDFDoc.UserPassword = password
    End If
    Try
      myPdfDocState.PDFDoc.LoadPDF(filename)
    Catch ex As System.Security.SecurityException
      myPdfDocState.RequiresPassword = True
      Return myPdfDocState
    Catch ex As Exception
      'Do nothing
    End Try
    myPdfDocState.RequiresPassword = False
    Return myPdfDocState
  End Function

End Class

Public Structure PDFDocState
  Public PDFDoc As PDFLibNet.PDFWrapper
  Public RequiresPassword As Boolean
End Structure
