<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="setSessionValue.aspx.cs" Inherits="MongoDBWeb.Provider" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label Text="text" runat="server" ID="IDSESSION" />
        <br />
        <a href="ASP/test.asp"> IR A LA PAGINA DE TEST</a>
        <br />
        <a href="getSessionValue.aspx"> IR A LA PAGINA DE GET ASP.NET</a>
    </div>
    </form>
</body>
</html>
