<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="PDFViewASP._Default" %>

<%@ Register src="PDFViewer.ascx" tagname="PDFViewer" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height: 509px; width: 100%; margin-left: 0px; background-color: #FFFFFF;">
        <asp:FileUpload ID="FileUpload1" runat="server" Width="224px" />
        <asp:Button ID="Button1" runat="server" Text="View PDF" />
        &nbsp;&nbsp;<asp:Label ID="ErrorLabel" runat="server" Text="" ForeColor="#CC0000" Visible="False"></asp:Label>
        <uc1:PDFViewer ID="PDFViewer1" runat="server" />
    </div>
    </form>
</body>
</html>
