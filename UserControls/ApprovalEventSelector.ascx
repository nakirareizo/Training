<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalEventSelector.ascx.cs" Inherits="TrainingRequisition.UserControls.ApprovalEventSelector" %>
<style type="text/css">
    .style1
    {
        width: 500px;
        background-color: #CCCCCC;
    }
</style>
<table cellpadding="5" cellspacing="0" class="style1">
    <tr>
        <td>
        Select a Training to approve:<br />
            <asp:DropDownList ID="dlEventDates" runat="server" Width="500px">
            </asp:DropDownList>
            <asp:Button ID="btnSelect" runat="server" onclick="btnSelect_Click" 
                Text="Select" />
        </td>
    </tr>
</table>
