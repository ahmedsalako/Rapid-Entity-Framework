<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CreateCustomer.aspx.cs" Inherits="RapidWebTest.CreateCustomer" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
    .style8
    {
        width: 239px;
        height: 30px;
    }
    .style9
    {
        height: 30px;
        width: 109px;
    }
    .style10
    {
        height: 23px;
        width: 109px;
    }
    .style11
    {
        width: 239px;
        height: 23px;
    }
        .style12
        {
        width: 239px;
    }
    .style13
    {
        width: 109px;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">&nbsp;x vvvvx zxv z&nbsp;&nbsp;&nbsp; vcscvcc<table style="width: 106%;">
    <tr>
        <td class="style1" colspan="2">
            <b>Enter Customer Detail</b></td>
    </tr>
    <tr>
        <td class="style13">
                    &nbsp;</td>
        <td class="style12">
            <asp:DropDownList ID="ddlCustomer" runat="server" Width="286px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="style13">
                    &nbsp;</td>
        <td class="style12">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style13">
                    First Name</td>
        <td class="style12">
            <asp:TextBox ID="txtFirstName" runat="server" Width="257px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style13">
                    Last name</td>
        <td class="style12">
            <asp:TextBox ID="txtLastName" runat="server" Width="288px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style13">
                    Country</td>
        <td class="style12">
            <asp:TextBox ID="txtCountry" runat="server" Width="173px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style9">
                    Street Name</td>
        <td class="style8">
            <asp:TextBox ID="txtStreetName" runat="server" Width="173px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style9">
                    House Num</td>
        <td class="style8">
            <asp:TextBox ID="txtHouseNo" runat="server" Width="173px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style9">
                    PostCode</td>
        <td class="style8">
            <asp:TextBox ID="txtPostCode" runat="server" Width="173px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style10">
        </td>
        <td class="style11">
            <asp:LinkButton ID="cmdItem" runat="server" onclick="cmdItem_Click">Create 
            Customer</asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td class="style2" colspan="2">
                    &nbsp;</td>
    </tr>
</table>
</asp:Content>

