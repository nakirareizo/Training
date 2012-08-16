<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookingOptions.ascx.cs" Inherits="TrainingRequisition.UserControls.BookingOptions" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<style type="text/css">
    .style1
    {
        width: 500px;
        border-left-style: solid;
        border-left-width: 1px;
        border-right: 1px solid #C0C0C0;
        border-top-style: solid;
        border-top-width: 1px;
        border-bottom: 1px solid #C0C0C0;
    }
</style>
<table class="style1">
    <tr>
        <td colspan="2">
            <b>Booking Options</b></td>
    </tr>
    <tr>
        <td>
            After how many days of an event is its Post-Training Assessment due?</td>
        <td>
            <asp:TextBox ID="txtPTABeginDaysAfterEvent" runat="server"></asp:TextBox>
        </td>
    </tr>
      <tr>
        <td>
            After how many days of an event is its Post-Training Assessment past due?</td>
        <td>
            <asp:TextBox ID="txtPTAEndDaysAfterEvent" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            Allow staff to book events even if event assessments are not yet done.</td>
        <td>
            <asp:CheckBox ID="chkBypassPTACheck" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Button ID="btnApply" runat="server" onclick="btnApply_Click" 
                Text="Apply" />
            <asp:Button ID="btnRevert" runat="server" onclick="btnRevert_Click" 
                Text="Revert" />
            <br />
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
            <asp:CustomValidator ID="CustomValidator1" runat="server" 
                onservervalidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
        </td>
        <td>
            &nbsp;</td>
    </tr>
</table>
