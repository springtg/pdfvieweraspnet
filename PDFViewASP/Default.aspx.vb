Partial Public Class _Default
  Inherits System.Web.UI.Page

  Protected mWidth As Integer
  Protected mHeight As Integer

  Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    If Session("ScreenResolution") Is Nothing Then
      If Request.QueryString("password") IsNot Nothing Then
        Session("password") = Request.QueryString("password").ToString
      End If
      If Request.QueryString("filename") IsNot Nothing Then
        Session("filename") = Request.QueryString("filename").ToString
      End If
      Response.Redirect("detectsize.aspx")
    Else
      Dim myArray() As String = Session("ScreenResolution").ToString().Split(",")
      PDFViewer1.Width = myArray(0)
      PDFViewer1.Height = myArray(1)
      If IsPostBack = False Then
        'Here you could fetch filenames/passwords from a DB based on query string parameters
        'This is just a simple example of how to set them via query string
        If Request.QueryString("password") IsNot Nothing Then
          PDFViewer1.Password = Request.QueryString("password").ToString
        ElseIf Session("password") IsNot Nothing Then
          PDFViewer1.Password = Session("password")
          Session.Remove("password")
        End If
        If Request.QueryString("filename") IsNot Nothing Then
          PDFViewer1.FileName = Request.QueryString("filename").ToString
        ElseIf Session("filename") IsNot Nothing Then
          PDFViewer1.FileName = Session("filename")
          Session.Remove("filename")
        End If
      End If
    End If
  End Sub


  'Current Upload limit is 25 MB (25000 k)
  'Change maxRequestLength in Web.config to set the upload limit

  'Current Upload timeout is 5 minutes (300 seconds)
  'Change executionTimeout in Web.config to set the upload timeout

  Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
    If FileUpload1.HasFile Then
      If ImageUtil.IsPDF(FileUpload1.FileName) Then
        ErrorLabel.Visible = False
        Dim savePath As String = Request.MapPath("PDF") & "\" & FileUpload1.FileName
        FileUpload1.SaveAs(savePath)
        If PDFViewer1.IsPasswordValid(savePath, tbPass.Text) = False Then
          ShowBadPasswordMessage()
          Exit Sub
        End If
        'Password must be set before the filename is set because the filename property activates the user control
        PDFViewer1.Password = tbPass.Text
        PDFViewer1.FileName = savePath
      Else
        ErrorLabel.Text = "Only PDF files (*.pdf) are allowed to be uploaded."
        ErrorLabel.Visible = True
      End If
    End If
  End Sub

  Protected Sub ShowBadPasswordMessage()
    ErrorLabel.Text = "Password is invalid. Please try again."
    ErrorLabel.Visible = True
  End Sub


End Class