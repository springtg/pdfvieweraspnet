Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Printing

Public Class AFPDFLibUtil

  'This uses an XPDF wrapper written by Jose Antonio Sandoval Soria of Guadalajara, México
  'The source is available at http://www.codeproject.com/KB/files/xpdf_csharp.aspx
  '
  'I have ported over to VB.NET select functionality from the C# PDF viewer in the above project

  Const RENDER_DPI As Integer = 150

  Public Shared Function GetPageFromPDF(ByVal filename As String, ByVal destPath As String, ByRef PageNumber As Integer, Optional ByVal DPI As Integer = RENDER_DPI, Optional ByVal Password As String = "", Optional ByVal searchText As String = "", Optional ByVal searchDir As SearchDirection = 0) As String
    GetPageFromPDF = ""
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    pdfDoc.RenderDPI = 72
    pdfDoc.LoadPDF(filename)
    If Not Nothing Is pdfDoc Then
      pdfDoc.CurrentPage = PageNumber
      pdfDoc.SearchCaseSensitive = False
      Dim searchResults As New List(Of PDFLibNet.PDFSearchResult)
      If searchText <> "" Then
        Dim lFound As Integer = 0
        If searchDir = SearchDirection.FromBeginning Then
          lFound = pdfDoc.FindFirst(searchText, PDFLibNet.PDFSearchOrder.PDFSearchFromdBegin, False, False)
        ElseIf searchDir = SearchDirection.Forwards Then
          lFound = pdfDoc.FindFirst(searchText, PDFLibNet.PDFSearchOrder.PDFSearchFromCurrent, False, False)
        ElseIf searchDir = SearchDirection.Backwards Then
          lFound = pdfDoc.FindFirst(searchText, PDFLibNet.PDFSearchOrder.PDFSearchFromCurrent, True, False)
        End If
        If lFound > 0 Then
          If searchDir = SearchDirection.FromBeginning Then
            PageNumber = pdfDoc.SearchResults(0).Page
            searchResults = GetAllSearchResults(filename, searchText, PageNumber)
          ElseIf searchDir = SearchDirection.Forwards Then
            If pdfDoc.SearchResults(0).Page > PageNumber Then
              PageNumber = pdfDoc.SearchResults(0).Page
              searchResults = GetAllSearchResults(filename, searchText, PageNumber)
            Else
              searchResults = SearchForNextText(pdfDoc, filename, searchText, PageNumber, searchDir)
              If searchResults.Count > 0 Then
                PageNumber = searchResults(0).Page
              End If
            End If
          ElseIf searchDir = SearchDirection.Backwards Then
            If pdfDoc.SearchResults(0).Page < PageNumber Then
              PageNumber = pdfDoc.SearchResults(0).Page
              searchResults = GetAllSearchResults(filename, searchText, PageNumber)
            Else
              searchResults = SearchForNextText(pdfDoc, filename, searchText, PageNumber, searchDir)
              If searchResults.Count > 0 Then
                PageNumber = searchResults(0).Page
              End If
            End If
          End If
        End If
      End If
      Dim outGuid As Guid = Guid.NewGuid()
      Dim output As String = destPath & "\" & outGuid.ToString & ".png"
      pdfDoc.ExportJpg(output, PageNumber, PageNumber, DPI, 90)
      While (pdfDoc.IsJpgBusy)
        Threading.Thread.Sleep(50)
      End While
      'Dim pdfPage As PDFLibNet.PDFPage = pdfDoc.Pages(PageNumber)
      'Dim bmp As Bitmap = pdfPage.GetBitmap(DPI, True)
      'bmp.Save(output, System.Drawing.Imaging.ImageFormat.Png)
      'bmp.Dispose()
      pdfDoc.Dispose()
      GetPageFromPDF = output
      If searchResults.Count > 0 Then
        GetPageFromPDF = HighlightSearchCriteria(output, DPI, searchResults)
      End If
    End If
  End Function

  Public Shared Function GetPageFromPDFNoSearch(ByVal filename As String, ByVal destPath As String, ByRef PageNumber As Integer, Optional ByVal DPI As Integer = RENDER_DPI, Optional ByVal Password As String = "", Optional ByVal searchText As String = "", Optional ByVal searchDir As SearchDirection = 0) As String
    GetPageFromPDFNoSearch = ""
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    pdfDoc.LoadPDF(filename)
    If Not Nothing Is pdfDoc Then
      Dim outGuid As Guid = Guid.NewGuid()
      Dim output As String = destPath & "\" & outGuid.ToString & ".png"
      pdfDoc.ExportJpg(output, PageNumber, PageNumber, DPI, 90)
      While (pdfDoc.IsJpgBusy)
        Threading.Thread.Sleep(50)
      End While
      pdfDoc.Dispose()
      GetPageFromPDFNoSearch = output
    End If
  End Function

  Public Shared Function SearchForNextText(ByVal pdfDoc As PDFLibNet.PDFWrapper, ByVal filename As String, ByVal searchText As String, ByVal currentPage As Integer, ByVal searchDir As SearchDirection) As List(Of PDFLibNet.PDFSearchResult)
    SearchForNextText = New List(Of PDFLibNet.PDFSearchResult)
    If Not Nothing Is pdfDoc Then
      pdfDoc.SearchCaseSensitive = False
SearchPDF:
      Dim lFound As Integer = 0
      Dim nextPageFound As Integer = 0
      If searchDir = SearchDirection.Forwards Then
        lFound = pdfDoc.FindNext(searchText)
      ElseIf searchDir = SearchDirection.Backwards Then
        lFound = pdfDoc.FindPrevious(searchText)
      End If
      If lFound > 0 Then
        If (pdfDoc.SearchResults(0).Page > currentPage And searchDir = SearchDirection.Forwards) _
        Or (pdfDoc.SearchResults(0).Page < currentPage And searchDir = SearchDirection.Backwards) Then
          nextPageFound = pdfDoc.SearchResults(0).Page
          Return GetAllSearchResults(filename, searchText, nextPageFound)
        Else
          GoTo SearchPDF
        End If
      End If
    End If
  End Function

  Public Shared Function GetAllSearchResults(ByVal filename As String, ByVal searchText As String, Optional ByVal pageNumber As Integer = 0, Optional ByVal searchBackwards As Boolean = False) As List(Of PDFLibNet.PDFSearchResult)
    GetAllSearchResults = New List(Of PDFLibNet.PDFSearchResult)
    Dim pdfDoc As New PDFLibNet.PDFWrapper
    pdfDoc.LoadPDF(filename)
    If Not Nothing Is pdfDoc Then
      pdfDoc.SearchCaseSensitive = False
      Dim lFound As Integer = 0
      pdfDoc.CurrentPage = pageNumber
      If searchBackwards = True And (pageNumber) <= pdfDoc.PageCount Then
        lFound = pdfDoc.FindFirst(searchText, PDFLibNet.PDFSearchOrder.PDFSearchFromCurrent, True, False)
      ElseIf (pageNumber) > 1 Then
        lFound = pdfDoc.FindFirst(searchText, PDFLibNet.PDFSearchOrder.PDFSearchFromCurrent, False, False)
      Else
        lFound = pdfDoc.FindFirst(searchText, PDFLibNet.PDFSearchOrder.PDFSearchFromdBegin, False, False)
      End If

      If lFound > 0 Then
        For Each item As PDFLibNet.PDFSearchResult In pdfDoc.SearchResults
          If (pageNumber = 0 Or item.Page = pageNumber) Then
            GetAllSearchResults.Add(item)
          End If
        Next
FindLoop:
        lFound = pdfDoc.FindNext(searchText)
        If lFound > 0 And (pageNumber = 0 Or pdfDoc.SearchResults(0).Page = pageNumber) Then
          For Each item As PDFLibNet.PDFSearchResult In pdfDoc.SearchResults
            If (pageNumber = 0 Or item.Page = pageNumber) Then
              GetAllSearchResults.Add(item)
            End If
          Next
        Else
          pdfDoc.Dispose()
          Exit Function
        End If
        GoTo FindLoop
      End If
    End If
  End Function

  Public Shared Function HighlightSearchCriteria(ByVal fileName As String, ByVal DPI As Integer, ByRef searchResults As List(Of PDFLibNet.PDFSearchResult)) As String
    HighlightSearchCriteria = fileName

    Dim bmp As New Bitmap(fileName)
    Dim gBmp As Graphics = Graphics.FromImage(bmp)
    Dim scale As Single = DPI / 72

    ' draw a blue rectangle to the bitmap in memory
    Dim blue As Color = Color.FromArgb(&H40, 0, 0, &HFF)
    Dim blueBrush As Brush = New SolidBrush(blue)

    For Each searchItem In searchResults
      gBmp.FillRectangle(blueBrush, searchItem.Position.X * scale, searchItem.Position.Y * scale, searchItem.Position.Width * scale, searchItem.Position.Height * scale)
    Next

    Dim outputPath As String = Regex.Replace(fileName, "(^.+\\).+$", "$1")
    Dim outputFileName As String = outputPath & Guid.NewGuid().ToString & ".jpg"
    bmp.Save(outputFileName, Imaging.ImageFormat.Jpeg)
    bmp.Dispose()
    ImageUtil.DeleteFile(fileName)
    HighlightSearchCriteria = outputFileName
    gBmp.Dispose()
    blueBrush.Dispose()

  End Function

  Public Shared Function GetJPGFromPDFDoc(ByRef pdfDoc As PDFLibNet.PDFWrapper, ByVal destPath As String, ByVal PageNumber As Integer, ByVal DPI As Integer) As String
    GetJPGFromPDFDoc = ""
    Dim outGuid As Guid = Guid.NewGuid()
    Dim output As String = destPath & "\" & outGuid.ToString & ".jpg"
    pdfDoc.ExportJpg(output, PageNumber, PageNumber, DPI, 90)
    While (pdfDoc.IsJpgBusy)
      Threading.Thread.Sleep(50)
    End While
    pdfDoc.Dispose()
  End Function

  Public Shared Function GetOptimalDPI(ByRef pdfDoc As PDFLibNet.PDFWrapper, ByRef oSize As Drawing.Size) As Integer
    GetOptimalDPI = 0
    If pdfDoc IsNot Nothing Then
      If pdfDoc.PageWidth > 0 And pdfDoc.PageHeight > 0 Then
        Dim DPIScalePercent As Single = 72 / pdfDoc.RenderDPI
        Dim picHeight As Integer = oSize.Height
        Dim picWidth As Integer = oSize.Width
        Dim docHeight As Integer = pdfDoc.PageHeight
        Dim docWidth As Integer = pdfDoc.PageWidth
        Dim dummyPicBox As New PictureBox
        dummyPicBox.Size = oSize
        If (picWidth > picHeight And docWidth < docHeight) Or (picWidth < picHeight And docWidth > docHeight) Then
          dummyPicBox.Width = picHeight
          dummyPicBox.Height = picWidth
        End If
        Dim HScale As Single = dummyPicBox.Width / (pdfDoc.PageWidth * DPIScalePercent)
        Dim VScale As Single = dummyPicBox.Height / (pdfDoc.PageHeight * DPIScalePercent)
        dummyPicBox.Dispose()
        If VScale > HScale Then
          GetOptimalDPI = Math.Floor(72 * HScale)
        Else
          GetOptimalDPI = Math.Floor(72 * VScale)
        End If
      End If
    End If
  End Function

  Public Shared Function GetImageFromPDF(ByRef pdfDoc As PDFLibNet.PDFWrapper, ByVal PageNumber As Integer, Optional ByVal DPI As Integer = RENDER_DPI) As System.Drawing.Image
    GetImageFromPDF = Nothing
    Try
      If pdfDoc IsNot Nothing Then
        pdfDoc.CurrentPage = PageNumber
        pdfDoc.CurrentX = 0
        pdfDoc.CurrentY = 0
        If DPI < 1 Then DPI = RENDER_DPI
        pdfDoc.RenderDPI = DPI
        Dim oPictureBox As New PictureBox
        pdfDoc.RenderPage(oPictureBox.Handle)
        GetImageFromPDF = Render(pdfDoc)
        oPictureBox.Dispose()
      End If
    Catch ex As Exception
      Throw ex
    End Try
  End Function

  Public Shared Function Render(ByRef pdfDoc As PDFLibNet.PDFWrapper) As System.Drawing.Bitmap
    Try
      If pdfDoc IsNot Nothing Then
        Dim backbuffer As System.Drawing.Bitmap = New Bitmap(pdfDoc.PageWidth, pdfDoc.PageHeight)
        pdfDoc.ClientBounds = New Rectangle(0, 0, pdfDoc.PageWidth, pdfDoc.PageHeight)
        Dim g As Graphics = Graphics.FromImage(backbuffer)
        Using g
          Dim hdc As IntPtr = g.GetHdc()
          pdfDoc.DrawPageHDC(hdc)
          g.ReleaseHdc()
        End Using
        g.Dispose()
        Return backbuffer
      End If
    Catch ex As Exception
      Throw ex
      Return Nothing
    End Try
    Return Nothing
  End Function

  Public Shared Function BuildHTMLBookmarks(ByRef pdfDoc As PDFLibNet.PDFWrapper, Optional ByVal pageNumberOnly As Boolean = False) As String

    If pageNumberOnly = True Then
      GoTo StartPageList
    End If

    If pdfDoc.Outline.Count <= 0 Then
StartPageList:
      BuildHTMLBookmarks = "<!--PageNumberOnly--><ul>"
      For i As Integer = 1 To pdfDoc.PageCount
        BuildHTMLBookmarks &= "<li><a href=""javascript:changePage('" & i & "')"">Page " & i & "</a></li>"
      Next
      BuildHTMLBookmarks &= "</ul>"
      Exit Function
    Else
      BuildHTMLBookmarks = ""
      FillHTMLTreeRecursive(pdfDoc.Outline, BuildHTMLBookmarks, pdfDoc)
      If Regex.IsMatch(BuildHTMLBookmarks, "\d") = False Then
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

End Class

