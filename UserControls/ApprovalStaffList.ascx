<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalStaffList.ascx.cs" Inherits="TrainingRequisition.UserControls.ApprovalStaffList" %>
<style type="text/css">
    .style1
    {
        width: 500px;
        background-color: #E7E7FF;
    }
</style>
<p>
    <table cellpadding="5" cellspacing="0" class="style1">
        <tr>
            <td>
                <asp:GridView ID="gvStaff" runat="server" AutoGenerateColumns="False" 
                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
                    CellPadding="3" GridLines="Horizontal" Width="500px" 
                    DataKeyNames="Username" onrowdatabound="gvStaff_RowDataBound">
                    <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" VerticalAlign="Top" />
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Username" HeaderText="Username" />
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Department" HeaderText="Department" />
                        <asp:TemplateField HeaderText="Supervisor's Justification">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblJustification" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ratings">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRatings" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                    <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                    <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                    <AlternatingRowStyle BackColor="#F7F7F7" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlButtons" runat="server" Visible="False">
                    <asp:Button ID="btnSelectAll" runat="server" onclick="btnSelectAll_Click" 
    Text="Select All" />
                    <asp:Button ID="btnClearAll" runat="server" onclick="btnClearAll_Click" 
                        Text="Clear All" />
                    <asp:Button ID="btnApprove" runat="server" onclick="btnApprove_Click" 
                        Text="Approve" />
                    <asp:Button ID="btnReject" runat="server" onclick="btnReject_Click" 
                        Text="Reject" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <br />
</p>



