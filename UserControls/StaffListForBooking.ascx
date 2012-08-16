<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffListForBooking.ascx.cs" Inherits="TrainingRequisition.UserControls.StaffListForBooking" %>
<asp:Panel ID="pnlSelectStaff" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Select Staff"></asp:Label>
    <asp:DropDownList ID="dlStaff" runat="server" DataTextField="Name" 
    DataValueField="Username" Width="350px">
    </asp:DropDownList>
    <asp:Button ID="btnSelect" runat="server" Text="View" 
    onclick="btnSelect_Click" />
    <asp:CheckBox ID="chkHide" runat="server" 
    Text="Hide staff with no pending approvals" 
        oncheckedchanged="chkHide_CheckedChanged" AutoPostBack="True" 
        Checked="True" />
</asp:Panel>


