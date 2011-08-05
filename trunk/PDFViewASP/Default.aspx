<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="PDFViewASP._Default" %>

<%@ Register src="PDFViewer.ascx" tagname="PDFViewer" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ASP.NET PDF Viewer</title>   
</head>
<%--<body  onload="loadFile();">--%>
<body>
    <form id="form1" runat="server">

    <table style="height: 509px; width: 100%; margin-left: 0px; background-color: #FFFFFF;">
    <tr>
        <td valign="middle" align="left"><asp:Label ID="Label1" runat="server" Text="File Name:" CssClass="LeftLabelColumn" />&nbsp;<asp:FileUpload ID="FileUpload1" runat="server"  CssClass="LeftValueColumn" />&nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Password:" CssClass="LeftLabelColumn" />&nbsp;<asp:TextBox id="tbPass" TextMode="password" runat="server" />&nbsp;&nbsp;&nbsp;<asp:Button ID="Button1" runat="server" Text="View PDF" />&nbsp;&nbsp;&nbsp;<asp:Label ID="ErrorLabel" runat="server" Text="" ForeColor="#CC0000" Visible="False" /></td>
    </tr>
    <tr>
    <td><uc1:PDFViewer ID="PDFViewer1" runat="server" /></td>
    </tr>
    </table>
    </form>
</body>
</html>
