<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="detectsize.aspx.vb" Inherits="PDFViewASP.detectsize" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<script language="javascript"> 
   if (typeof(window.innerWidth) == 'number') 
   {
      //Non-IE
      browserWidth = window.innerWidth;
      browserHeight = window.innerHeight;
   } else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
      //IE 6+ in 'standards compliant mode'
      browserWidth = document.documentElement.clientWidth;
      browserHeight = document.documentElement.clientHeight;
   } else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
      //IE 4 compatible
      browserWidth = document.body.clientWidth;
      browserHeight = document.body.clientHeight;
   }
res = "&res="+browserWidth+","+browserHeight 
top.location.href="detectsize.aspx?action=set"+res 
</script>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
