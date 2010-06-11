Partial Public Class _Default
  Inherits System.Web.UI.Page

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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