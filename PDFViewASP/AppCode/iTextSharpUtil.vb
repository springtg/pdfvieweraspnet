Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Drawing.Imaging

Public Class iTextSharpUtil

  Public Shared mBookmarkList As New ArrayList

  Public Shared Function GetPDFPageCount(ByVal filepath As String, Optional ByVal userPassword As String = "") As Integer

    Dim oPdfReader As PdfReader
    If userPassword <> "" Then
      Dim encoding As New System.Text.ASCIIEncoding()
      oPdfReader = New PdfReader(filepath, encoding.GetBytes(userPassword))
    Else
      oPdfReader = New PdfReader(filepath)
    End If
    Dim page_count As Integer = oPdfReader.NumberOfPages
    oPdfReader.Close()
    Return page_count
  End Function

  Public Shared Function GetPDFPageSize(ByVal filepath As String, ByVal pageNumber As Integer, Optional ByVal userPassword As String = "") As Drawing.Size
    Dim oPdfReader As PdfReader
    If userPassword <> "" Then
      Dim encoding As New System.Text.ASCIIEncoding()
      oPdfReader = New PdfReader(filepath, encoding.GetBytes(userPassword))
    Else
      oPdfReader = New PdfReader(filepath)
    End If
    Dim page_count As Integer = oPdfReader.NumberOfPages
    If pageNumber >= 1 And pageNumber <= page_count Then
      Dim rect As iTextSharp.text.Rectangle = oPdfReader.GetPageSize(pageNumber)
      GetPDFPageSize.Height = rect.Height
      GetPDFPageSize.Width = rect.Width
    End If
    oPdfReader.Close()
  End Function

  Public Shared Function GetOptimalDPI(ByVal filepath As String, ByVal pageNumber As Integer, ByRef oSize As Drawing.Size, Optional ByVal userPassword As String = "") As Integer
    GetOptimalDPI = 0
    Dim pageSize As Drawing.Size = GetPDFPageSize(filepath, pageNumber, userPassword)
    If pageSize.Width > 0 And pageSize.Height > 0 Then
      Dim picHeight As Integer = oSize.Height
      Dim picWidth As Integer = oSize.Width
      Dim dummyPicBox As New PictureBox
      dummyPicBox.Size = oSize
      'If (picWidth > picHeight And pageSize.Width < pageSize.Height) Or (picWidth < picHeight And pageSize.Width > pageSize.Height) Then
      '  dummyPicBox.Width = picHeight
      '  dummyPicBox.Height = picWidth
      'End If
      Dim HScale As Single = dummyPicBox.Width / pageSize.Width
      Dim VScale As Single = dummyPicBox.Height / pageSize.Height
      dummyPicBox.Dispose()
      If HScale < VScale Then
        GetOptimalDPI = Math.Floor(72 * HScale)
      Else
        GetOptimalDPI = Math.Floor(72 * VScale)
      End If
    End If
  End Function

  Public Shared Sub FillTreeRecursive(ByVal arList As ArrayList, ByVal treeNodes As TreeNode)
    For Each item As Object In arList
      Dim tn As New TreeNode(item("Title"))
      Dim ol As iTextOutline
      ol.Title = item("Title")
      ol.Action = item("Action")
      ol.Named = item("Named")
      ol.Page = item("Page")
      tn.Tag = ol
      treeNodes.Nodes.Add(tn)
      If Not Nothing Is item("Kids") Then
        FillTreeRecursive(item("Kids"), tn)
      End If
    Next
  End Sub

  Public Shared Function BuildBookmarkTreeFromPDF(ByVal FileName As String, ByVal TreeNodes As TreeNodeCollection, Optional ByVal userPassword As String = "") As Boolean
    TreeNodes.Clear()
    Dim oPdfReader As PdfReader
    If userPassword <> "" Then
      Dim encoding As New System.Text.ASCIIEncoding()
      oPdfReader = New PdfReader(FileName, encoding.GetBytes(userPassword))
    Else
      oPdfReader = New PdfReader(FileName)
    End If

    Dim pageCount As Integer = oPdfReader.NumberOfPages
    Dim arList As ArrayList = New ArrayList()
    arList = SimpleBookmark.GetBookmark(oPdfReader)

    oPdfReader.Close()
    If Nothing Is arList Then
      Return False
    End If
    Dim CurrentNode As New TreeNode
    CurrentNode = TreeNodes.Add("Bookmarks")
    FillTreeRecursive(arList, CurrentNode)
    Return True
  End Function

  'Dim format As String = "<li><a href=""javascript:changeImage('images/page{0}.png')"">{1}</a></li>"

  Public Shared Function BuildHTMLBookmarks(ByVal FileName As String, Optional ByVal userPassword As String = "", Optional ByVal pageNumberOnly As Boolean = False, Optional ByVal pageCount As Integer = 0) As String

    If pageNumberOnly = True And pageCount > 0 Then
      GoTo StartPageList
    End If

    Dim oPdfReader As PdfReader

    If userPassword <> "" Then
      Dim encoding As New System.Text.ASCIIEncoding()
      oPdfReader = New PdfReader(FileName, encoding.GetBytes(userPassword))
    Else
      oPdfReader = New PdfReader(FileName)
    End If

    pageCount = oPdfReader.NumberOfPages
    Dim arList As ArrayList = New ArrayList()
    arList = SimpleBookmark.GetBookmark(oPdfReader)
    oPdfReader.Close()

    If Nothing Is arList Then
StartPageList:
      BuildHTMLBookmarks = "<!--PageNumberOnly--><ul>"
      For i As Integer = 1 To pageCount
        BuildHTMLBookmarks &= "<li><a href=""javascript:changePage('" & i & "')"">Page " & i & "</a></li>"
      Next
      BuildHTMLBookmarks &= "</ul>"
      Exit Function
    Else
      BuildHTMLBookmarks = ""
      fillRecursiveHTMLTree(arList, BuildHTMLBookmarks)
      If Regex.IsMatch(BuildHTMLBookmarks, "\d") = False Then
        BuildHTMLBookmarks = ""
        GoTo StartPageList
      End If
      Exit Function
    End If
  End Function

  Public Shared Sub fillRecursiveHTMLTree(ByVal arList As ArrayList, ByRef strHtml As String)
    strHtml &= "<ul>"
    For Each item As Object In arList
      If Not Nothing Is item("Page") Then
        Dim i As String = Regex.Replace(item("Page"), "(^\d+).+$", "$1")
        strHtml &= "<li><a href=""javascript:changePage('" & i & "')"">" & Web.HttpUtility.HtmlEncode(item("Title")) & "</a></li>"
      End If
      If Not Nothing Is item("Kids") Then
        fillRecursiveHTMLTree(item("Kids"), strHtml)
      End If
    Next
    strHtml &= "</ul>"
  End Sub

  Public Shared Function IsEncrypted(ByVal pdfFileName As String) As Boolean
    IsEncrypted = False
    Try
      Dim oPDFReader As New PdfReader(pdfFileName)
      oPDFReader.Close()
    Catch ex As BadPasswordException
      IsEncrypted = True
    End Try
  End Function

  Public Shared Function IsPasswordValid(ByVal pdfFileName As String, ByVal Password As String) As Boolean
    IsPasswordValid = False
    Try
      Dim encoding As New System.Text.ASCIIEncoding()
      Dim oPDFReader As New PdfReader(pdfFileName, encoding.GetBytes(Password))
      oPDFReader.Close()
      IsPasswordValid = True
    Catch ex As BadPasswordException
      'Authentication Failed
    End Try
  End Function

#Region "ASP Web routines"

  Public Shared Sub FillTreeRecursiveASP(ByVal arList As ArrayList, ByVal treeNodes As Web.UI.WebControls.TreeNode)
    For Each item As Object In arList
      Dim tn As New Web.UI.WebControls.TreeNode(item("Title"))
      tn.Value = Regex.Replace(item("Page"), "(^\d+).+$", "$1")
      treeNodes.ChildNodes.Add(tn)
      If Not Nothing Is item("Kids") Then
        FillTreeRecursiveASP(item("Kids"), tn)
      End If
    Next
  End Sub

  Public Shared Function BuildASPBookmarkTreeFromPDF(ByVal FileName As String, ByVal treeViewCol As Web.UI.WebControls.TreeNodeCollection, Optional ByVal userPassword As String = "") As Boolean
    treeViewCol.Clear()
    Dim oPdfReader As PdfReader
    If userPassword <> "" Then
      Dim encoding As New System.Text.ASCIIEncoding()
      oPdfReader = New PdfReader(FileName, encoding.GetBytes(userPassword))
    Else
      oPdfReader = New PdfReader(FileName)
    End If

    Dim pageCount As Integer = oPdfReader.NumberOfPages
    Dim arList As ArrayList = New ArrayList()
    arList = SimpleBookmark.GetBookmark(oPdfReader)

    oPdfReader.Close()
    If Nothing Is arList Then
      Return Nothing
    End If
    Dim CurrentNode As New TreeNode
    Dim tn As New Web.UI.WebControls.TreeNode("Bookmarks")
    FillTreeRecursiveASP(arList, tn)
    treeViewCol.Add(tn)
  End Function

#End Region

End Class

Public Structure iTextOutline
  Dim Title As String
  Dim Action As String
  Dim Page As String
  Dim Named As String
  Dim Position As Drawing.Point
End Structure
