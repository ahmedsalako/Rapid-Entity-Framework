<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" CodeFile="CreateProduct.aspx.cs" AutoEventWireup="true"  Inherits="RapidWebTest.CreateProduct" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
    .style4
    {
        width: 79px;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <table style="width: 82%;">
    <tr>
        <td class="style1" colspan="2">
            <b>Enter Product Details</b></td>
    </tr>
    <tr>
        <td class="style4">
                    &nbsp;</td>
        <td class="style3">
            <asp:DropDownList ID="ddlProducts" runat="server" Width="266px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="style4">
                    &nbsp;</td>
        <td class="style3">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style4">
                    Item Name</td>
        <td class="style3">
            <asp:TextBox ID="txtItemName" runat="server" Width="284px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style4">
                    Price</td>
        <td class="style3">
            <asp:TextBox ID="txtPrice" runat="server" ReadOnly="True">10</asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style4">
        </td>
        <td class="style3">
            <asp:LinkButton ID="cmdItem" runat="server" onclick="cmdItem_Click">Create 
            Product</asp:LinkButton>
        </td>
    </tr>
</table>
</asp:Content>

