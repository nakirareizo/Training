<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="EventList.ascx.cs" Inherits="TrainingRequisition.Assessments.UserControls.EventList" %>
<asp:Panel ID="pnlSelectEvent" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Select Training"></asp:Label>
    <asp:DropDownList ID="dlEvents" runat="server" DataTextField="DisplayName" 
    DataValueField="ID" Width="400px" >
    </asp:DropDownList>
    <asp:Button ID="btnSelect" runat="server" Text="Select" 
    onclick="btnSelect_Click" />
</asp:Panel>


