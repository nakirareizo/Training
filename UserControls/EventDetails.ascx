<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventDetails.ascx.cs" Inherits="TrainingRequisition.UserControls.EventDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<style type="text/css">
    .style1
    {
        background-color: #DDDDDD;
        width: 500px;
    }
    </style>
<table cellpadding="5" cellspacing="0" class="style1" >
    <tr>
        <td>
            Title</td>
        <td>
            <asp:TextBox ID="txtTitle" runat="server" Width="400px" ReadOnly="true" 
                Enabled="false"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            Provider</td>
        <td>
            <asp:TextBox ID="txtProvider" runat="server" Width="400px" ReadOnly="true" 
                Enabled="false"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            Price</td>
        <td>
            <asp:DropDownList ID="dlCurrencies" runat="server" ReadOnly="true" Enabled="false">
            </asp:DropDownList>
            <asp:TextBox ID="txtPrice" runat="server" ReadOnly="true" Enabled="false"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            Dates</td>
        <td>
            From
                <asp:TextBox ID="txtDateFrom" runat="server" ReadOnly="true" Enabled="false" ></asp:TextBox>
&nbsp;To
              <asp:TextBox ID="txtDateTo" runat="server" ReadOnly="true"  Enabled="false"></asp:TextBox>
            <br />
        </td>
    </tr>
    <tr>
        <td>
            Training Type</td>
        <td>
            <asp:DropDownList ID="dlTrainingTypes" runat="server" DataTextField="Name" Enabled="false" DataValueField="Name" >
            </asp:DropDownList>
        </td>
    </tr>
</table>
