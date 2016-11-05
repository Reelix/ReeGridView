<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestPage.aspx.cs" Inherits="TestPage" %>
<%@ Register Src="~/ReeGridView.ascx" TagName="GridView" TagPrefix="Ree" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .rgvtable
        {
            border-collapse: collapse;
            border: 1px solid black;
            font-family: Verdana;
        }
        .rgvheader
        {
            font-weight: bold;
            color: blue;
            border: 1px solid black;
        }
        .rgvdata
        {
            border: 1px solid black;
            color: red;
            text-align: right;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Just a test page for the grid</div>
        <Ree:GridView ID="rgvPeople" runat="server" />
        <asp:Button ID="btnParseData" runat="server" OnClick="btnParseData_Click" Text="Parse data as custom object" />
    </form>
</body>
</html>
