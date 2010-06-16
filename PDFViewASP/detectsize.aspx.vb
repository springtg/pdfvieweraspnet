Public Partial Class detectsize
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    If (Request.QueryString("action") IsNot Nothing) Then
      Session("ScreenResolution") = Request.QueryString("res").ToString()
      Response.Redirect("Default.aspx")
    End If
  End Sub

End Class