<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffList.ascx.cs" Inherits="TrainingRequisition.UserControls.StaffList" %>
<asp:Panel ID="pnlSelectStaff" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Select Staff"></asp:Label>
    <asp:DropDownList ID="dlStaff" runat="server" DataTextField="Name" DataValueField="Username" Width="400px">
    </asp:DropDownList>
    <asp:Button ID="btnSelect" runat="server" Text="View" onclick="btnSelect_Click" />
</asp:Panel>