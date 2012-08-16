<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="StaffList.ascx.cs" Inherits="TrainingRequisition.Assessments.UserControls.StaffList" %>
<asp:Panel ID="pnlSelectStaff" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Select Staff"></asp:Label>
    <asp:DropDownList ID="dlStaff" runat="server" DataTextField="Name" 
    DataValueField="Username" Width="350px">
    </asp:DropDownList>
    <asp:Button ID="btnSelect" runat="server" Text="Select" 
    onclick="btnSelect_Click" />
</asp:Panel>


